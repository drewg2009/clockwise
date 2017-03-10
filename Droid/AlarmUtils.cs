using System;
using Android.Content;
using Android.App;
using Android.Graphics;
using Clockwise.Helpers;
using Android.OS;
using Android.Widget;
using Android.Content.PM;
using Java.Util;
using System.Collections.Generic;

namespace Clockwise.Droid
{
	public class AlarmUtils
	{
		public static string ALARM_NAME = "CLOCKWISE";

		private static List<PendingIntent> pendingIntents = new List<PendingIntent>();
		private static List<PendingIntent> notificationClickIntents = new List<PendingIntent>();
		private static AlarmManager am = null;

		public static void Init(Context context, bool addingAlarm)
		{
			String[] currentAlarms = Settings.AlarmTime.Split('|');
			ComponentName receiver = new ComponentName(context, Java.Lang.Class.FromType(typeof(AlarmReceiver)));
        	PackageManager pm = context.PackageManager;
			pm.SetComponentEnabledSetting(receiver, ComponentEnabledState.Enabled, ComponentEnableOption.DontKillApp);

			pendingIntents = new List<PendingIntent>();
			notificationClickIntents = new List<PendingIntent>();

			Intent alarmIntent = new Intent(context, typeof(AlarmReceiver));

			for (int i = 0; i < currentAlarms.Length; i++)
			{
				notificationClickIntents.Add(PendingIntent.GetActivity(context, i, new Intent(), 0));
				pendingIntents.Add(PendingIntent.GetBroadcast(context, i, alarmIntent, PendingIntentFlags.UpdateCurrent));

			}

			if (addingAlarm)
			{
				notificationClickIntents.Add(PendingIntent.GetActivity(context, notificationClickIntents.Count, new Intent(), 0));
				pendingIntents.Add(PendingIntent.GetBroadcast(context, pendingIntents.Count, alarmIntent, PendingIntentFlags.UpdateCurrent));
			}


			am = (AlarmManager)Application.Context.GetSystemService(Context.AlarmService);
		}

		public static void SetTime(Context context, int hour, int minute, int alarmIndex, int repeatDays, bool addingAlarm)
		{
			Init(context, addingAlarm);
			String[] currentAlarms = Settings.AlarmTime.Split('|');

			//Create new alarm
			if (Settings.AlarmTime == string.Empty)
			{
				Settings.AlarmTime = "" + alarmIndex + ":" + hour + ":" + minute;
			}
			else if (alarmIndex == currentAlarms.Length)
			{
				Console.Write("creating alarm");
				string newAlarm = "" + alarmIndex + ":" + hour + ":" + minute;
				string newAlarmSetting = "";
				foreach (String s in currentAlarms)
				{
					newAlarmSetting += s + "|";
				}
				newAlarmSetting += newAlarm;
				Settings.AlarmTime = newAlarmSetting;
			}
			//Change old alarm
			else {
				Console.Write("editing alarm");

				currentAlarms[alarmIndex] = "" + alarmIndex + ":" + hour + ":" + minute;
				string newAlarmSetting = "";
				foreach (String s in currentAlarms)
				{
					newAlarmSetting += s + "|";
				}
				Settings.AlarmTime = newAlarmSetting.TrimEnd('|');
			}

			am.Cancel(pendingIntents[alarmIndex]);


			bool[] daySelection = new bool[7];

			//Load saved says into array
			for (int j = 0; j < 7; j++)
			{
				daySelection[j] = (repeatDays & (1 << j)) == (1 << j);
			}

			Calendar calendar = Calendar.Instance;
			calendar.TimeInMillis = Java.Lang.JavaSystem.CurrentTimeMillis();
				        
			int currentDay = calendar.Get(CalendarField.DayOfWeek) - 1;
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

			long offset = 0;

			calendar.Set(CalendarField.HourOfDay, hour);
			calendar.Set(CalendarField.Minute, minute);
			calendar.Set(CalendarField.Second, 0);

			Java.Util.Date current = new Java.Util.Date(Java.Lang.JavaSystem.CurrentTimeMillis());

			//set time is before current time or current day isn't selected
			if (calendar.Time.Before(current) || !daySelection[currentDay])
			{
				offset = daysUntilNextAlarm * 1000 * 60 * 60 * 24;
			}

			Console.WriteLine("offset: " + offset);
			calendar.TimeInMillis = calendar.TimeInMillis + offset;
			if ((int)Build.VERSION.SdkInt >= 21)
			{
				AlarmManager.AlarmClockInfo info = new AlarmManager.AlarmClockInfo(calendar.TimeInMillis, notificationClickIntents[alarmIndex]);
				am.SetAlarmClock(info, pendingIntents[alarmIndex]);
			}
			else {
				am.SetExact(AlarmType.RtcWakeup, calendar.TimeInMillis, notificationClickIntents[alarmIndex]);
			}

			//Toast
			string toast = "Set time: " + (calendar.Get(CalendarField.Month) + 1) + "/" + calendar.Get(CalendarField.DayOfMonth) + "/" + calendar.Get(CalendarField.Year)
			                                                                                      + ", " + calendar.Get(CalendarField.HourOfDay) + ":" + calendar.Get(CalendarField.Minute);
			Toast.MakeText(context, toast, ToastLength.Long).Show();
		}

		public static void Cancel(Context context, int alarmIndex, bool addingAlarm)
		{
			//Helpers.Settings.AlarmTime = string.Empty;
			String[] currentAlarms = Settings.AlarmTime.Split('|');
			List<String> currentAlarmsList = new List<String>(currentAlarms);
			currentAlarmsList.RemoveAt(alarmIndex);
			string newAlarmSetting = "";
			for (int i = 0; i < currentAlarmsList.Count; i++)
			{
				newAlarmSetting += "" + i + ":" + currentAlarmsList[i].Substring(currentAlarmsList[i].IndexOf(':')+1);
			}
			if (am == null) Init(context, addingAlarm);
			am.Cancel(pendingIntents[alarmIndex]);
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
