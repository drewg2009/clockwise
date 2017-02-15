using System;
using Android.Content;
using Android.App;
using Android.Graphics;
using Clockwise.Helpers;
using Android.OS;
using Android.Widget;
using Android.Content.PM;
using Android.Provider;

namespace Clockwise.Droid
{
	public class AlarmUtils
	{
		public static string ALARM_NAME = "CLOCKWISE";

		private static PendingIntent pendingIntent = null;
		private static PendingIntent notificationClickIntent = null;
		private static AlarmManager am = null;

		public static void Init(Context context)
		{
			ComponentName receiver = new ComponentName(context, Java.Lang.Class.FromType(typeof(AlarmReceiver)));
        	PackageManager pm = context.PackageManager;
			pm.SetComponentEnabledSetting(receiver, ComponentEnabledState.Enabled, ComponentEnableOption.DontKillApp);
			
			Intent alarmIntent = new Intent(context, typeof(AlarmReceiver));
			notificationClickIntent = PendingIntent.GetActivity(context, 0, new Intent(), 0);
			pendingIntent = PendingIntent.GetBroadcast(context, 0, alarmIntent, PendingIntentFlags.UpdateCurrent);
			am = (AlarmManager)Android.App.Application.Context.GetSystemService(Context.AlarmService);
		}

		public static void SetTime(Context context, int hour, int minute)
		{
			if (am == null) Init(context);
			Helpers.Settings.AlarmTime = hour + ":" + minute;

			am.Cancel(pendingIntent);

			int repeatDays = Int32.Parse(Helpers.Settings.RepeatDays);
			bool[] daySelection = new bool[7];

			//Load saved says into array
			for (int j = 0; j < 7; j++)
			{
				daySelection[j] = (repeatDays & (1 << j)) == (1 << j);
				Console.WriteLine("daySelection " + j + ": " + daySelection[j].ToString());

			}

			int currentDay = (int) DateTime.Now.DayOfWeek;
			Console.WriteLine("currentDay: " + currentDay);


			int daysUntilNextAlarm = currentDay < 6 ? currentDay + 1 : 0;
			while (daysUntilNextAlarm != currentDay && !daySelection[daysUntilNextAlarm])
			{
				daysUntilNextAlarm = daysUntilNextAlarm < 6 ? daysUntilNextAlarm + 1 : 0;
			}

			daysUntilNextAlarm -= currentDay;
			if (daysUntilNextAlarm <= 0)
			{
				daysUntilNextAlarm += 7;
			}

			Console.WriteLine("daysUntilNextAlarm: " + daysUntilNextAlarm);

			long offset = 0;
			DateTime temp = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hour, minute, 0);
			Console.WriteLine("this minute: " +temp.ToLongDateString() + ", " + temp.ToLongTimeString());

			if (DateTime.Compare(temp, DateTime.Now) < 0 || !daySelection[currentDay])
				offset = daysUntilNextAlarm * TimeSpan.TicksPerDay;

			Console.WriteLine("offset: " + offset);

			DateTime setTime = new DateTime(temp.Ticks + offset);


			if ((int)Build.VERSION.SdkInt >= 21)
			{
				AlarmManager.AlarmClockInfo info = new AlarmManager.AlarmClockInfo(setTime.Ticks, notificationClickIntent);
				am.SetAlarmClock(info, pendingIntent);
			}
			else {
				am.SetExact(AlarmType.RtcWakeup, setTime.Ticks, notificationClickIntent);
			}

			//Toast
			string toast = "Set time: " + setTime.Month + "/" + setTime.Day + "/" + setTime.Year
												 + ", " + setTime.Hour + ":" + setTime.Minute;
			DateTime dt = new DateTime(am.NextAlarmClock.TriggerTime);
			string nextAlarm2 = dt.ToLongDateString() + ", " + dt.ToLongTimeString();
 			Console.WriteLine("next alarm: " + nextAlarm2);

			Console.WriteLine(toast);
			Toast.MakeText(context, toast, ToastLength.Long).Show();
		}

		public static void Cancel(Context context)
		{
			Helpers.Settings.AlarmTime = string.Empty;
			if (am == null) Init(context);
			am.Cancel(pendingIntent);
		}

		public static void PostNotification(Context context)
		{
			//Intent speechIntent = new Intent(context, null);

			//Intent snoozeIntent = new Intent(context, null);
			//Intent closeIntent = new Intent(context, null);

			Notification.Builder builder = new Notification.Builder(context)
				.SetLargeIcon(BitmapFactory.DecodeResource(context.Resources, Resource.Drawable.circle_high_res))
				.SetSmallIcon(Resource.Drawable.ic_alarm_white_48dp)
				.SetContentTitle("Clockwise")
				.SetDefaults(NotificationDefaults.All)
				.SetPriority((int)NotificationPriority.Max)
				.SetStyle(new Notification.BigTextStyle().BigText("Click Play to hear your modules."));

			Notification notification = builder.Build();
			NotificationManager nm = (NotificationManager)Application.Context.GetSystemService(Context.NotificationService);
			nm.Notify("ClockWise", 0, notification);
		}

		public static void ClearNotification()
		{
			NotificationManager nm = (NotificationManager) Android.App.Application.Context.GetSystemService("notification");
			nm.Cancel(ALARM_NAME, 0);
		}
	}
}
