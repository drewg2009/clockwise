using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using Clockwise.Helpers;
using Java.Util;

namespace Clockwise.Droid
{
	[Service]
	public class SnoozeService : Service
	{
		public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
		{
			int alarm_index = intent.GetIntExtra("alarm_index", -1);
			AlarmUtils.ClearNotification(alarm_index);

			//Turn of alarm tone
			if (ServiceHelper.IsMyServiceRunning(typeof(SongService), this)) StopService(SongService.intent);
			if(AlarmReceiver.player != null && AlarmReceiver.player.IsPlaying)
			{
        		AlarmReceiver.player.Stop();
        		AlarmReceiver.player = null;
        	}

			AlarmUtils.Snooze(alarm_index);

			Toast.MakeText(this, "Snoozing for " + Settings.GetAlarmSnooze(alarm_index) + " minutes.", ToastLength.Short).Show();
			StopSelf();
			return base.OnStartCommand(intent, flags, startId);
		}

		public override IBinder OnBind(Intent intent)
		{
			return null;
		}
	}
}
