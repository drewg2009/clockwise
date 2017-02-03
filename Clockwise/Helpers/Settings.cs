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
		private static readonly string RepeatDaysDef = string.Empty;

		private const string ModuleOrderKey = "module_order_key";
		private static readonly string ModuleOrderDef = string.Empty;

		private const string SnoozeKey = "snooze_key";
		private static readonly string SnoozeDef = string.Empty;



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

  }
}