using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Clockwise.Helpers;
using Java.Util;

namespace Clockwise.Droid
{
	[Service]
	public class CloseService : Service
	{
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

				if (Settings.IsAlarmOn(alarm_index))
				{
					if (Settings.GetAlarmRepeatDays(alarm_index) > 0)
					{
						AlarmUtils.SetTime(this, Settings.GetAlarmHour(alarm_index), Settings.GetAlarmMinute(alarm_index),
						                   alarm_index, Settings.GetAlarmRepeatDays(alarm_index), Settings.GetAlarmSnooze(alarm_index), false);
					}
					else {
						Settings.ToggleAlarm(alarm_index, false);
						if (ManageAlarms.instance != null)
							ManageAlarms.instance.RefreshAlarms();
					}
				}

			}
			else {
				Toast.MakeText(this, "bad index: " + alarm_index, ToastLength.Long).Show();
			}

			StopSelf();

			return base.OnStartCommand(intent, flags, startId);
		}
		public override void OnCreate()
		{
			base.OnCreate();

		}

		public override IBinder OnBind(Intent intent)
		{
			return null;
		}
	}
}
