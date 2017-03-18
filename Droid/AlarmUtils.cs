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
		public static string ALARM_NAME = "Clockwise";

		private static List<PendingIntent> pendingIntents = new List<PendingIntent>();
		private static List<PendingIntent> notificationClickIntents = new List<PendingIntent>();
		private static AlarmManager am = null;
		private static PackageManager pm;
		public static void Init(Context context, bool addingAlarm)
		{
			String[] currentAlarms = Settings.Alarms.Split('|');
			ComponentName receiver = new ComponentName(context, Java.Lang.Class.FromType(typeof(AlarmReceiver)));
        	pm = context.PackageManager;
			pm.SetComponentEnabledSetting(receiver, ComponentEnabledState.Enabled, ComponentEnableOption.DontKillApp);

			pendingIntents = new List<PendingIntent>();
			notificationClickIntents = new List<PendingIntent>();


			for (int i = 0; i < currentAlarms.Length; i++)
			{
				notificationClickIntents.Add(PendingIntent.GetActivity(context, i, new Intent(), 0));
				Intent alarmIntent = new Intent(context, typeof(AlarmReceiver));
				alarmIntent.PutExtra("alarm_index", pendingIntents.Count);
				pendingIntents.Add(PendingIntent.GetBroadcast(context, i, alarmIntent, PendingIntentFlags.UpdateCurrent));
			}

			if (addingAlarm)
			{
				notificationClickIntents.Add(PendingIntent.GetActivity(context, notificationClickIntents.Count, new Intent(), 0));
				Intent alarmIntent = new Intent(context, typeof(AlarmReceiver));
				alarmIntent.PutExtra("alarm_index", pendingIntents.Count);
				pendingIntents.Add(PendingIntent.GetBroadcast(context, pendingIntents.Count, alarmIntent, PendingIntentFlags.UpdateCurrent));
			}


			am = (AlarmManager)Application.Context.GetSystemService(Context.AlarmService);
		}

		public static void SetTime(Context context, int hour, int minute, int alarmIndex, int repeatDays, bool addingAlarm)
		{
			Init(context, addingAlarm);

			Settings.EditAlarm(alarmIndex, hour, minute, repeatDays);
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

			if (repeatDays == 0)
			{
				//Set to current day and day after
				daySelection[currentDay] = true;
				daySelection[currentDay == 6 ? 0 : currentDay + 1] = true;
			}

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

		public static void Snooze(int index)
		{
			int interval = 1000 * 60 * int.Parse(Settings.GetAlarmField(index, Settings.AlarmField.Snooze));
			Calendar calendar = Calendar.Instance;
			calendar.TimeInMillis = Java.Lang.JavaSystem.CurrentTimeMillis() + interval;

			if ((int)Build.VERSION.SdkInt >= 21)
			{
				AlarmManager.AlarmClockInfo info = new AlarmManager.AlarmClockInfo(calendar.TimeInMillis, notificationClickIntents[index]);
				am.SetAlarmClock(info, pendingIntents[index]);
			}
			else {
				am.SetExact(AlarmType.RtcWakeup, calendar.TimeInMillis, notificationClickIntents[index]);
			}
		}

		public static void Cancel(Context context, int alarmIndex, bool addingAlarm)
		{
			//Helpers.Settings.AlarmTime = string.Empty;
			String[] currentAlarms = Settings.Alarms.Split('|');
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

		public static void PostNotification(Context context, int alarm_index)
		{
			context.StopService(new Intent(context, typeof(SpeechService)));

			Intent speechIntent = new Intent(context, typeof(SpeechService));
			Intent snoozeIntent = new Intent(context, typeof(SnoozeService));
			Intent closeIntent = new Intent(context, typeof(CloseService));
			speechIntent.PutExtra("alarm_index", alarm_index);
			snoozeIntent.PutExtra("alarm_index", alarm_index);
			closeIntent.PutExtra("alarm_index", alarm_index);

			PendingIntent piSpeech = PendingIntent.GetService(context, alarm_index, speechIntent, PendingIntentFlags.UpdateCurrent);
			PendingIntent piSnooze = PendingIntent.GetService(context, alarm_index, snoozeIntent, PendingIntentFlags.UpdateCurrent);
			PendingIntent piClose = PendingIntent.GetService(context, alarm_index, closeIntent, PendingIntentFlags.UpdateCurrent);

			Notification.Builder builder = new Notification.Builder(context)
				.SetLargeIcon(BitmapFactory.DecodeResource(context.Resources, Resource.Drawable.circle_high_res))
				.SetSmallIcon(Resource.Drawable.ic_alarm_white_48dp)
				.SetContentTitle("Clockwise: " + alarm_index)
				.SetDefaults(NotificationDefaults.All)
				.SetPriority((int)NotificationPriority.Max)
				.SetStyle(new Notification.BigTextStyle().BigText("Click Play to hear your modules."));

			//Add actions
			if ((int)Build.VERSION.SdkInt >= 23)
			{
				Notification.Action playAction = new Notification.Action.Builder(null, "Play", piSpeech).Build();
				Notification.Action snoozeAction = new Notification.Action.Builder(null, "Snooze", piSnooze).Build();
				Notification.Action closeAction = new Notification.Action.Builder(null, "Close", piClose).Build();

				builder.AddAction(playAction)
						.AddAction(snoozeAction)
						.AddAction(closeAction);

			}
			else if ((int)Build.VERSION.SdkInt >= 20)
			{ //api 20-22
				builder.AddAction(new Notification.Action.Builder(0, "Play", piSpeech).Build())
						.AddAction(new Notification.Action.Builder(0, "Snooze", piSnooze).Build())
						.AddAction(new Notification.Action.Builder(0, "Close", piClose).Build());
			}
			else { //for api 19
				builder.AddAction(0, "Play", piSpeech)
						.AddAction(0, "Snooze", piSnooze)
						.AddAction(0, "Close", piClose);
			}

			Notification notification = builder.Build();
			notification.Flags |= NotificationFlags.NoClear | NotificationFlags.OngoingEvent;

			NotificationManager nm = (NotificationManager)Application.Context.GetSystemService(Context.NotificationService);
			nm.Notify("Clockwise", alarm_index, notification);

		}

		public static void ClearNotification(int alarm_index)
		{
			NotificationManager nm = (NotificationManager) Application.Context.GetSystemService(Context.NotificationService);
			nm.Cancel(ALARM_NAME, alarm_index);
		}

	}
}
