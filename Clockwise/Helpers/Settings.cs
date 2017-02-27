// Helpers/Settings.cs
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

	private const string AlarmTimeKey = "alarm_time_key";
	private static readonly string AlarmTimeDef = string.Empty;

		private const string SongKey = "song_key";
		private static readonly string SongDef = string.Empty;

		private const string FirstTimeKey = "first_time_key";
		private static readonly string FirstTimeDef = string.Empty;

		private const string RepeatDaysKey = "repeat_days_key";
		private static readonly string RepeatDaysDef = "0";

		private const string ModuleOrderKey = "module_order_key";
		private static readonly string ModuleOrderDef = string.Empty;

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
    #endregion


    public static string AlarmTime
    {
			get {return AppSettings.GetValueOrDefault<string>(AlarmTimeKey, AlarmTimeDef);}
			set {AppSettings.AddOrUpdateValue<string>(AlarmTimeKey, value);}
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
  }
}