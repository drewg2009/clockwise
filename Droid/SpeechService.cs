﻿using System;
using Android.App;
using Android.OS;
using Android.Content;
using Geolocator.Plugin;
using Android.Content.PM;
using Clockwise.Helpers;
using Android.Widget;
using Java.Util;
using System.Threading.Tasks;
using System.Net;
using Android.Speech.Tts;

namespace Clockwise.Droid
{
	[Service]
    public class SpeechService : Service, TextToSpeech.IOnInitListener
	{
        TextToSpeech textToSpeech;
        Locale lang;
		public override void OnCreate()
		{
			base.OnCreate();
            textToSpeech = new TextToSpeech(this, this, "com.google.android.tts");
			lang = Java.Util.Locale.Default;
			textToSpeech.SetLanguage(lang);
			//Cancel notification

			//Stop song


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
					else {
						Settings.SetAlarmField(alarm_index, Settings.AlarmField.Status, "" + (int)Settings.AlarmStatus.ALARM_OFF);
						if (ManageAlarms.instance != null)
							ManageAlarms.instance.RefreshAlarms();
					}
				}

			}
			else {
				Toast.MakeText(this, "bad index: " + alarm_index, ToastLength.Long).Show();
			}

			MakeRequest(alarm_index);
			StopSelf();
			return base.OnStartCommand(intent, flags, startId);
		}

		public async void MakeRequest(int index)
		{
			string request;
            string parameters = "moduleInfo=";
            string errorMsg = "Clockwise could not load your module information at this time. Please try again later.";
			string URI = "http://phplaravel-43928-259989.cloudwaysapps.com/get/moduleData";
			if (CheckSelfPermission(Android.Manifest.Permission.ReadExternalStorage) == Permission.Granted)
			{
				var locator = CrossGeolocator.Current;
				locator.DesiredAccuracy = 50;
				var position = await locator.GetPositionAsync(timeoutMilliseconds: 10000);
				Console.WriteLine("Lat:{0}, Lon:{1}", position.Latitude, position.Longitude);
				request = JSONRequestSerializer.GetInstance().GetJsonRequest(index, position.Latitude, position.Longitude);

            }
			else
			{
				request = JSONRequestSerializer.GetInstance().GetJsonRequest(index, 0, 0);
			}
            parameters += request;
            await Task.Factory.StartNew( () => {
				using (WebClient wc = new WebClient())
				{
					wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    string result = wc.UploadString(new Uri(URI), parameters);
                    if(!string.IsNullOrEmpty(result)){
						textToSpeech.Speak(result, QueueMode.Flush, null, null);
					}
                    else{
                        textToSpeech.Speak(errorMsg, QueueMode.Flush, null, null);
					}
				}
            });


			Console.WriteLine("\nrequest: " + request);

		}


		void TextToSpeech.IOnInitListener.OnInit(OperationResult status)
		{
			// if we get an error, default to the default language
			if (status == OperationResult.Error)
				textToSpeech.SetLanguage(Java.Util.Locale.Default);
			// if the listener is ok, set the lang
			if (status == OperationResult.Success)
				textToSpeech.SetLanguage(lang);
		}

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }
	}
}
