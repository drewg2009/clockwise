using System;
using Android.App;
using Android.OS;
using Android.Content;
using Clockwise.Helpers;
using Android.Widget;
using Java.Util;
using System.Threading.Tasks;
using System.Net;
using Android.Locations;
using Android.Gms.Common.Apis;
using Android.Gms.Common;
using Android.Gms.Location;
using Android.Runtime;
using Android.Media;
using System.IO;
using Java.IO;
using Newtonsoft.Json;

namespace Clockwise.Droid
{
    [Service]
    public class SpeechService : Service
    {
        Location location;


        public override void OnCreate()
        {
            base.OnCreate();
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            int alarm_index = intent.GetIntExtra("alarm_index", -1);

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


            MakeRequest(alarm_index);

            return base.OnStartCommand(intent, flags, startId);
        }


        public void MakeRequest(int index)
        {

            string request = string.Empty;
            string parameters = "moduleInfo=";
            string getModuleDataURI = "http://phplaravel-43928-259989.cloudwaysapps.com/get/moduleData";

            LocationManager lm = GetSystemService(Context.LocationService) as LocationManager;
            Criteria locationCriteria = new Criteria();
            locationCriteria.Accuracy = Accuracy.Coarse;
            locationCriteria.PowerRequirement = Power.Low;
            string provider = lm.GetBestProvider(locationCriteria, true);
            if (lm.IsProviderEnabled(LocationManager.NetworkProvider)
            || lm.IsProviderEnabled(LocationManager.GpsProvider))
            {
                location = lm.GetLastKnownLocation(provider);
                System.Console.WriteLine("Lat:{0}, Lon:{1}", location.Latitude, location.Longitude);
                request = JSONRequestSerializer.GetInstance().GetJsonRequest(index, location.Latitude, location.Longitude);

            }
            else
            {
                request = JSONRequestSerializer.GetInstance().GetJsonRequest(index, 0, 0);
            }

            Task.Factory.StartNew(() =>
            {
                string introURI = "http://phplaravel-43928-259989.cloudwaysapps.com/get/introMessage";
                string introMessage = GetApiData(introURI, "");
                byte[] introData = System.Convert.FromBase64String(introMessage);
                MediaPlayer m = PlayMp3(introData);

                m.Completion += delegate
                {
					parameters += request;
					string content = GetApiData(getModuleDataURI, parameters);
					string errorURI = "http://phplaravel-43928-259989.cloudwaysapps.com/get/errorMessage";
                    if (!string.IsNullOrEmpty(content))
                    {
                        
                        byte[] data = System.Convert.FromBase64String(content);
                        MediaPlayer m2 = PlayMp3(data);
                    }
                    else
                    {

						string errorParams = "";
						string errorMessage = GetApiData(errorURI, "");
                        byte[] data = System.Convert.FromBase64String(errorMessage);
                        MediaPlayer m2 = PlayMp3(data);
                    }
                    StopSelf();

                };

            });


            //Console.WriteLine("\nrequest: " + request);
        }

        private string GetApiData(string URI, string reqParams)
        {
            string content = "";
            using (WebClient wc = new WebClient())
            {
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                string result = wc.UploadString(new Uri(URI), reqParams);
                dynamic decodedResult = JsonConvert.DeserializeObject(result);
                content = (string)decodedResult;

            }

            return content;

        }


        private MediaPlayer PlayMp3(byte[] mp3SoundByteArray)
        {

            MediaPlayer mediaPlayer = null;
            try
            {
                // create temp file that will hold byte array
                Java.IO.File tempMp3 = Java.IO.File.CreateTempFile("file.mp3", "mp3", CacheDir);
                tempMp3.DeleteOnExit();
                FileOutputStream fos = new FileOutputStream(tempMp3);
                fos.Write(mp3SoundByteArray);
                fos.Close();

                // resetting mediaplayer instance to evade problems
                //_player.Reset();

                // In case you run into issues with threading consider new instance like:
                mediaPlayer = new MediaPlayer();

                // Tried passing path directly, but kept getting 
                // "Prepare failed.: status=0x1"
                // so using file descriptor instead
                FileInputStream fis = new FileInputStream(tempMp3);
                mediaPlayer.SetDataSource(fis.FD);

                mediaPlayer.Prepare();
                mediaPlayer.Start();

            }
            catch (Java.IO.IOException ex)
            {
                string s = ex.ToString();
                ex.PrintStackTrace();
            }
            return mediaPlayer;
        }


        public override IBinder OnBind(Intent intent)
        {
            return null;
        }


    }
}
