// Helpers/Settings.cs
using System;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace Clockwise.Helpers
{
  /// <summary>
  /// This is the Settings static class that can be used in your Core solution or in any
  /// of your client applications. All settings are laid out the same exact way with getters
  /// and setters. 
  /// </summary>
  public static class Settings
  {
    private static ISettings AppSettings
    {
      get
      {
        return CrossSettings.Current;
      }
    }

    #region Setting Constants

	private const string AlarmsKey = "alarms_key";
	private static readonly string AlarmsDef = string.Empty;

		private const string SongKey = "song_key";
		private static readonly string SongDef = string.Empty;

		private const string FirstTimeKey = "first_time_key";
		private static readonly string FirstTimeDef = string.Empty;

		private const string RepeatDaysKey = "repeat_days_key";
		private static readonly string RepeatDaysDef = "0";

		private const string ModuleOrderKey = "module_order_key";
		private static readonly string ModuleOrderDef = 
			"notifications|weather|news|reddit|twitter|countdown|reminders|traffic|fact|quote|tdih";
		
		private const string SnoozeKey = "snooze_key";
		private static readonly string SnoozeDef = string.Empty;

		private const string WeatherKey = "weather_key";
		private static readonly string WeatherDef = string.Empty;

		private const string RedditKey = "reddit_key";
		private static readonly string RedditDef = string.Empty;

		private const string TwitterKey = "twitter_key";
		private static readonly string TwitterDef = string.Empty;

		private const string NewsKey = "news_key";
		private static readonly string NewsDef = string.Empty;

		private const string RemindersKey = "reminders_key";
		private static readonly string RemindersDef = string.Empty;

		private const string CountdownKey = "countdown_key";
		private static readonly string CountdownDef = string.Empty;

		private const string TrafficKey = "traffic_key";
		private static readonly string TrafficDef = string.Empty;

		private const string QuoteKey = "quote_key";
		private static readonly string QuoteDef = string.Empty;

		private const string TDIHKey = "tdih_key";
		private static readonly string TDIHDef = string.Empty;

		private const string FactKey = "fact_key";
		private static readonly string FactDef = string.Empty;

		private const string AndroidFileAccessKey = "android_file_access_key";
		private static readonly string AndroidFileAccessDef = string.Empty;

		public enum AlarmStatus { ALARM_ON, ALARM_OFF };

		#endregion


		public static string Alarms
    {
			get {return AppSettings.GetValueOrDefault<string>(AlarmsKey, AlarmsDef);}
			set {AppSettings.AddOrUpdateValue<string>(AlarmsKey, value);}
	}

		public static string Song
		{
			get { return AppSettings.GetValueOrDefault<string>(SongKey, SongDef); }
			set { AppSettings.AddOrUpdateValue<string>(SongKey, value); }
		}

		public static string FirstTime
		{
			get { return AppSettings.GetValueOrDefault<string>(FirstTimeKey, FirstTimeDef); }
			set { AppSettings.AddOrUpdateValue<string>(FirstTimeKey, value); }
		}

		public static string RepeatDays
		{
			get { return AppSettings.GetValueOrDefault<string>(RepeatDaysKey, RepeatDaysDef); }
			set { AppSettings.AddOrUpdateValue<string>(RepeatDaysKey, value); }
		}

		public static string ModuleOrder
		{
			get { return AppSettings.GetValueOrDefault<string>(ModuleOrderKey, ModuleOrderDef); }
			set { AppSettings.AddOrUpdateValue<string>(ModuleOrderKey, value); }
		}

		public static string Snooze
		{
			get { return AppSettings.GetValueOrDefault<string>(SnoozeKey, SnoozeDef); }
			set { AppSettings.AddOrUpdateValue<string>(SnoozeKey, value); }
		}

		public static string Weather
		{
			get { return AppSettings.GetValueOrDefault<string>(WeatherKey, WeatherDef); }
			set { AppSettings.AddOrUpdateValue<string>(WeatherKey, value); }
		}

		public static string Reddit
		{
			get { return AppSettings.GetValueOrDefault<string>(RedditKey, RedditDef); }
			set { AppSettings.AddOrUpdateValue<string>(RedditKey, value); }
		}

		public static string Twitter
		{
			get { return AppSettings.GetValueOrDefault<string>(TwitterKey, TwitterDef); }
			set { AppSettings.AddOrUpdateValue<string>(TwitterKey, value); }
		}

		public static string News
		{
			get { return AppSettings.GetValueOrDefault<string>(NewsKey, NewsDef); }
			set { AppSettings.AddOrUpdateValue<string>(NewsKey, value); }
		}

		public static string Reminders
		{
			get { return AppSettings.GetValueOrDefault<string>(RemindersKey, RemindersDef); }
			set { AppSettings.AddOrUpdateValue<string>(RemindersKey, value); }
		}

		public static string Countdown
		{
			get { return AppSettings.GetValueOrDefault<string>(CountdownKey, CountdownDef); }
			set { AppSettings.AddOrUpdateValue<string>(CountdownKey, value); }
		}

		public static string Traffic
		{
			get { return AppSettings.GetValueOrDefault<string>(TrafficKey, TrafficDef); }
			set { AppSettings.AddOrUpdateValue<string>(TrafficKey, value); }
		}

		public static string Quote
		{
			get { return AppSettings.GetValueOrDefault<string>(QuoteKey, QuoteDef); }
			set { AppSettings.AddOrUpdateValue<string>(QuoteKey, value); }
		}

		public static string TDIH
		{
			get { return AppSettings.GetValueOrDefault<string>(TDIHKey, TDIHDef); }
			set { AppSettings.AddOrUpdateValue<string>(TDIHKey, value); }
		}

		public static string Fact
		{
			get { return AppSettings.GetValueOrDefault<string>(FactKey, FactDef); }
			set { AppSettings.AddOrUpdateValue<string>(FactKey, value); }
		}

		public static string AndroidFileAccess
		{
			get { return AppSettings.GetValueOrDefault<string>(AndroidFileAccessKey, AndroidFileAccessDef); }
			set { AppSettings.AddOrUpdateValue<string>(AndroidFileAccessKey, value); }
		}

		public static bool IsAlarmOn(int index)
		{
			if (Alarms == string.Empty) return false;
			else {
				string[] alarmSettings = Alarms.Split('|')[index].Split(':');
				return Int32.Parse(alarmSettings[4]) == (int) AlarmStatus.ALARM_ON;
			}
		}

		public static int GetAlarmHour(int index)
		{
			if (Alarms == string.Empty) return -1;
			else {
				string[] alarmSettings = Alarms.Split('|')[index].Split(':');
				return Int32.Parse(alarmSettings[1]);
			}
		}

		public static void SetAlarmHour(int index, int hour)
		{
			string[] alarms = Alarms.Split('|');
			string[] alarmSettings = alarms[index].Split(':');
			alarmSettings[1] = "" + hour;
			alarms[index] = alarmSettings[0] + ":"
				+ alarmSettings[1] + ":"
				+ alarmSettings[2] + ":"
				+ alarmSettings[3] + ":"
				+ alarmSettings[4];
			string newSettings = string.Empty;
			foreach (String s in alarms)
			{
				newSettings += s + "|";
			}
			Alarms = newSettings.TrimEnd('|');
		}

		public static void SetAlarmMinute(int index, int minute)
		{
			string[] alarms = Alarms.Split('|');
			string[] alarmSettings = alarms[index].Split(':');
			alarmSettings[2] = "" + minute;
			alarms[index] = alarmSettings[0] + ":"
				+ alarmSettings[1] + ":"
				+ alarmSettings[2] + ":"
				+ alarmSettings[3] + ":"
				+ alarmSettings[4];
			string newSettings = string.Empty;
			foreach (String s in alarms)
			{
				newSettings += s + "|";
			}
			Alarms = newSettings.TrimEnd('|');
		}

		public static int GetAlarmMinute(int index)
		{
			if (Alarms == string.Empty) return -1;
			else {
				string[] alarmSettings = Alarms.Split('|')[index].Split(':');
				return Int32.Parse(alarmSettings[2]);
			}
		}

		public static int GetAlarmRepeatDays(int index)
		{
			if (Alarms == string.Empty) return -1;
			else {
				string[] alarmSettings = Alarms.Split('|')[index].Split(':');
				return Int32.Parse(alarmSettings[3]);
			}
		}

		public static void ToggleAlarm(int index, bool alarmOn)
		{
			string[] alarms = Alarms.Split('|');
			string[] alarmSettings = alarms[index].Split(':');
			alarmSettings[4] = "" + (alarmOn ? (int)AlarmStatus.ALARM_ON : (int)AlarmStatus.ALARM_OFF);
			alarms[index] = alarmSettings[0] + ":" 
				+ alarmSettings[1] + ":"
				+ alarmSettings[2] + ":"
				+ alarmSettings[3] + ":"
				+ alarmSettings[4];
			string newSettings = string.Empty;
			foreach (String s in alarms)
			{
				newSettings += s + "|";
			}
			Alarms = newSettings.TrimEnd('|');
		}

		public static void SetRepeatDays(int index, int repeatDays)
		{
			string[] alarms = Alarms.Split('|');
			string[] alarmSettings = alarms[index].Split(':');
			alarmSettings[3] = "" + repeatDays;

			alarms[index] = alarmSettings[0] + ":"
				+ alarmSettings[1] + ":"
				+ alarmSettings[2] + ":"
				+ alarmSettings[3] + ":"
				+ alarmSettings[4];
			string newSettings = string.Empty;
			foreach (String s in alarms)
			{
				newSettings += s + "|";
			}
			Alarms = newSettings.TrimEnd('|');
		}



		public static void DeleteAlarm(int index)
		{
			string newSetting = string.Empty;
			string[] alarms = Alarms.Split('|');
			for (int i = 0; i < alarms.Length; i++)
			{
				if (i != index)
				{
					newSetting += alarms[i] + "|";
				}
			}

			if (newSetting != string.Empty) newSetting = newSetting.TrimEnd('|');
		}
  }
}