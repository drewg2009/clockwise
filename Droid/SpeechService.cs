using System;
using Android.App;
using Android.OS;
using Android.Content;
using Clockwise.Helpers;
using Android.Widget;
using Java.Util;
using System.Threading.Tasks;
using System.Net;
using Android.Speech.Tts;
using Android.Locations;
using Android.Gms.Common.Apis;
using Android.Gms.Common;
using Android.Gms.Location;
using Android.Runtime;
using Android.Media;
using System.IO;

namespace Clockwise.Droid
{
    [Service]
    public class SpeechService : Service, TextToSpeech.IOnInitListener
	{
        TextToSpeech textToSpeech;
        Locale lang;
        int alarm_index;
        bool initFinished = false;
        Location location;

		public override void OnCreate()
        {
            base.OnCreate();
            textToSpeech = new TextToSpeech(this, this, "com.google.android.tts");
            lang = Java.Util.Locale.Default;
            textToSpeech.SetLanguage(lang);
        }


        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            alarm_index = intent.GetIntExtra("alarm_index", -1);

            if (alarm_index >= 0)
            {
                AlarmUtils.ClearNotification(alarm_index);

                //Turn of alarm tone
                //if(ServiceHelper.isMyServiceRunning(SongService.class, this)
                if (SongService.intent != null)
                    StopService(SongService.intent);
                if (AlarmReceiver.player != null && AlarmReceiver.player.IsPlaying)
                {
                    AlarmReceiver.player.Stop();
                    AlarmReceiver.player = null;
                }

                //Reschedule
                Calendar calendar = Calendar.Instance;
                calendar.TimeInMillis = Java.Lang.JavaSystem.CurrentTimeMillis();

                int temp = int.Parse(Settings.GetAlarmField(alarm_index, Settings.AlarmField.Status));
                if (temp == (int)Settings.AlarmStatus.ALARM_ON)
                {
                    if (int.Parse(Settings.GetAlarmField(alarm_index, Settings.AlarmField.RepeatDays)) > 0)
                    {
                        AlarmUtils.SetTime(this, int.Parse(Settings.GetAlarmField(alarm_index, Settings.AlarmField.Hour)),
                                           int.Parse(Settings.GetAlarmField(alarm_index, Settings.AlarmField.Minute)),
                                           alarm_index, int.Parse(Settings.GetAlarmField(alarm_index, Settings.AlarmField.RepeatDays)), false);
                    }
                    else
                    {
                        Settings.SetAlarmField(alarm_index, Settings.AlarmField.Status, "" + (int)Settings.AlarmStatus.ALARM_OFF);
                        if (ManageAlarms.instance != null)
                            ManageAlarms.instance.RefreshAlarms();
                    }
                }

            }
            else
            {
                Toast.MakeText(this, "bad index: " + alarm_index, ToastLength.Long).Show();
            }

            initFinished = true;
            return base.OnStartCommand(intent, flags, startId);
        }

        public async void MakeRequest(int index)
        {

            await Task.Factory.StartNew(() =>
            {
                string introMsg = "Hello. Clockwise is collecting your module info. Please wait one moment.";
                textToSpeech.Speak(introMsg, QueueMode.Add, null, null);
            });

            string request = string.Empty;
            string parameters = "moduleInfo=";
            string errorMsg = "Clockwise could not load your module information at this time. Please try again later.";
            string URI = "http://phplaravel-43928-259989.cloudwaysapps.com/get/moduleData";

            LocationManager lm = GetSystemService(Context.LocationService) as LocationManager;
            Criteria locationCriteria = new Criteria();
            locationCriteria.Accuracy = Accuracy.Coarse;
            locationCriteria.PowerRequirement = Power.Low;
            string provider = lm.GetBestProvider(locationCriteria, true);
            if (lm.IsProviderEnabled(LocationManager.NetworkProvider)
            || lm.IsProviderEnabled(LocationManager.GpsProvider))
            {
                location = lm.GetLastKnownLocation(provider);
                Console.WriteLine("Lat:{0}, Lon:{1}", location.Latitude, location.Longitude);
                request = JSONRequestSerializer.GetInstance().GetJsonRequest(index, location.Latitude, location.Longitude);

            }
            else
            {
                request = JSONRequestSerializer.GetInstance().GetJsonRequest(index, 0, 0);
            }
            parameters += request;
            var task = Task.Factory.StartNew(() =>
            {
                using (WebClient wc = new WebClient())
                {
                    wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    string result = wc.UploadString(new Uri(URI), parameters);
                    if (!string.IsNullOrEmpty(result))
                    {
                        
						MediaPlayer _player;
                        byte[] data = Convert.FromBase64String(Base64Decode(result));
						File.WriteAllBytes("file.mp3", data);
                        _player = MediaPlayer.Create(this, Android.Net.Uri.Parse("file.mp3"));
                        _player.Start();
						//textToSpeech.Speak(result, QueueMode.Add, null, null);
                    }
                    else
                    {
                        textToSpeech.Speak(errorMsg, QueueMode.Add, null, null);
                    }
                }
            });

            Console.WriteLine("\nrequest: " + request);
        }

		public static string Base64Decode(string base64EncodedData)
		{
			var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
			return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
		}


        void TextToSpeech.IOnInitListener.OnInit(OperationResult status)
        {
            // if we get an error, default to the default language
            if (status == OperationResult.Error)
                textToSpeech.SetLanguage(Java.Util.Locale.Default);
            // if the listener is ok, set the lang
            if (status == OperationResult.Success)
                textToSpeech.SetLanguage(lang);

            double interval = 1000;
            double timeout = 5000;
            double elapsedTime = 0;
            System.Timers.Timer timer = new System.Timers.Timer(interval);
            timer.Elapsed += async (sender, e) => await Task.Factory.StartNew(() =>
            {
                if (elapsedTime > timeout)
                {
                    string errorMsg = "Clockwise timed out with your current connection. Please try again later.";
                    textToSpeech.Speak(errorMsg, QueueMode.Add, null, null);
                    timer.Stop();
                    StopSelf();
                }
                else
                {
                    if (initFinished)
                    {
                        MakeRequest(alarm_index);
                        timer.Stop();
                        StopSelf();
                    }
                }
                elapsedTime += interval;

            });
            timer.Start();


		

        }


        public override IBinder OnBind(Intent intent)
        {
            return null;
        }


    }
}
