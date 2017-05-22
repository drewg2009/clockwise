using System;
using Android.App;
using Android.OS;
using Android.Content;
using Geolocator.Plugin;
using Android.Content.PM;
using Clockwise.Helpers;
using Android.Widget;
using Android.Locations;
using Java.Util;

namespace Clockwise.Droid
{
	[Service]
	public class SpeechService : Service
	{
		public override void OnCreate()
		{
			base.OnCreate();
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
			LocationManager lm = (LocationManager)ApplicationContext.GetSystemService(Context.LocationService);
			if ((int)Build.VERSION.SdkInt >= 23 && CheckSelfPermission(Android.Manifest.Permission.AccessFineLocation) == Permission.Granted
			    && CheckSelfPermission(Android.Manifest.Permission.AccessCoarseLocation) == Permission.Granted
			    && lm.IsProviderEnabled(LocationManager.GpsProvider))
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

			Console.WriteLine("\nrequest: " + request);

		}
		public override IBinder OnBind(Intent intent)
		{
			return null;
		}
	}
}
