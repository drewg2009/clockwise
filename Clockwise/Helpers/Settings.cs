// Helpers/Settings.cs
using System;
using System.Collections.Generic;
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

		private static string DefaultModuleOrder = 
			"notifications:weather:news:reddit:twitter:countdown:reminders:traffic:fact:quote:tdih";
		public enum AlarmStatus { ALARM_ON, ALARM_OFF };

		public enum Modules {WEATHER, REDDIT, TWITTER, NEWS, COUNTDOWN, REMINDERS, TRAFFIC, FACT, QUOTE, TDIH};

		public static string EMPTY_MODULE = "null";

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

		public static void EditAlarm(int index, int hour, int minute, int repeatDays)
		{
			String[] currentAlarms = Settings.Alarms.Split('|');

			//Create new alarm
			AlarmStatus status = AlarmStatus.ALARM_ON;
			if (Alarms == string.Empty)
			{
				Alarms = "" + index + ":" + hour + ":" + minute + ":" + repeatDays + ":" + (int)status;
				ModuleOrder = DefaultModuleOrder;
				AddModules();
			}
			else if (index == currentAlarms.Length)
			{
				string newAlarm = "" + index + ":" + hour + ":" + minute + ":" + repeatDays + ":" + (int)status;
				Alarms += "|" + newAlarm;
				ModuleOrder += "|" + DefaultModuleOrder;
				AddModules();
			}
			//Change old alarm
			else {
				currentAlarms[index] = "" + index + ":" + hour + ":" + minute + ":" + repeatDays + ":" + (int)status;
				string newAlarmSetting = "";
				foreach (String s in currentAlarms)
				{
					newAlarmSetting += s + "|";
				}
				Alarms = newAlarmSetting.TrimEnd('|');
			}
		}

		public static void AddModules()
		{
			Weather += (Weather == string.Empty) ? EMPTY_MODULE : "|" + EMPTY_MODULE;
			News += (News == string.Empty) ? EMPTY_MODULE : "|" + EMPTY_MODULE;
			Reddit += (Reddit == string.Empty) ? EMPTY_MODULE : "|" + EMPTY_MODULE;
			Twitter += (Twitter == string.Empty) ? EMPTY_MODULE : "|" + EMPTY_MODULE;
			Traffic += (Traffic == string.Empty) ? EMPTY_MODULE : "|" + EMPTY_MODULE;
			Countdown += (Countdown == string.Empty) ? EMPTY_MODULE : "|" + EMPTY_MODULE;
			Reminders += (Reminders == string.Empty) ? EMPTY_MODULE : "|" + EMPTY_MODULE;
			Quote += (Quote == string.Empty) ? EMPTY_MODULE : "|" + EMPTY_MODULE;
			Fact += (Fact == string.Empty) ? EMPTY_MODULE : "|" + EMPTY_MODULE;
			TDIH += (TDIH == string.Empty) ? EMPTY_MODULE : "|" + EMPTY_MODULE;
		}

		public static void RemoveModules(int index)
		{
			//Weather
			{
				string newWeatherSetting = string.Empty;
				string[] weathers = Weather.Split('|');
				for (int i = 0; i < weathers.Length; i++)
					if (i != index)
						newWeatherSetting += weathers[i] + "|";

				if (newWeatherSetting != string.Empty) newWeatherSetting = newWeatherSetting.TrimEnd('|');
				Weather = newWeatherSetting;
			}

			//News
			{
				string newNewsSetting = string.Empty;
				string[] news = News.Split('|');
				for (int i = 0; i < news.Length; i++)
					if (i != index)
						newNewsSetting += news[i] + "|";

				if (newNewsSetting != string.Empty) newNewsSetting = newNewsSetting.TrimEnd('|');
				News = newNewsSetting;
			}

			//Reddit
			{
				string newRedditSetting = string.Empty;
				string[] reddits = Reddit.Split('|');
				for (int i = 0; i < reddits.Length; i++)
					if (i != index)
						newRedditSetting += reddits[i] + "|";

				if (newRedditSetting != string.Empty) newRedditSetting = newRedditSetting.TrimEnd('|');
				Reddit = newRedditSetting;
			}

			//Twitter
			{
				string newTwitterSettings = string.Empty;
				string[] twitters = Twitter.Split('|');
				for (int i = 0; i < twitters.Length; i++)
					if (i != index)
						newTwitterSettings += twitters[i] + "|";

				if (newTwitterSettings != string.Empty) newTwitterSettings = newTwitterSettings.TrimEnd('|');
				Twitter = newTwitterSettings;
			}

			//Traffic
			{
				string newTrafficSettings = string.Empty;
				string[] traffics = Traffic.Split('|');
				for (int i = 0; i < traffics.Length; i++)
					if (i != index)
						newTrafficSettings += traffics[i] + "|";

				if (newTrafficSettings != string.Empty) newTrafficSettings = newTrafficSettings.TrimEnd('|');
				Traffic = newTrafficSettings;
			}

			//Countdown
			{
				string newCountdownSettings = string.Empty;
				string[] countdowns = Countdown.Split('|');
				for (int i = 0; i < countdowns.Length; i++)
					if (i != index)
						newCountdownSettings += countdowns[i] + "|";

				if (newCountdownSettings != string.Empty) newCountdownSettings = newCountdownSettings.TrimEnd('|');
				Countdown = newCountdownSettings;
			}

			//Reminders
			{
				string newRemindersSettings = string.Empty;
				string[] reminders = Reminders.Split('|');
				for (int i = 0; i < reminders.Length; i++)
					if (i != index)
						newRemindersSettings += reminders[i] + "|";

				if (newRemindersSettings != string.Empty) newRemindersSettings = newRemindersSettings.TrimEnd('|');
				Reminders = newRemindersSettings;
			}

			//Fact
			{
				string newFactSettings = string.Empty;
				string[] facts = Fact.Split('|');
				for (int i = 0; i < facts.Length; i++)
					if (i != index)
						newFactSettings += facts[i] + "|";

				if (newFactSettings != string.Empty) newFactSettings = newFactSettings.TrimEnd('|');
				Fact = newFactSettings;
			}

			//Quote
			{
				string newQuoteSettings = string.Empty;
				string[] quotes = Quote.Split('|');
				for (int i = 0; i < quotes.Length; i++)
					if (i != index)
						newQuoteSettings += quotes[i] + "|";

				if (newQuoteSettings != string.Empty) newQuoteSettings = newQuoteSettings.TrimEnd('|');
				Quote = newQuoteSettings;
			}

			//History
			{
				string newTDIHSettings = string.Empty;
				string[] TDIHs = TDIH.Split('|');
				for (int i = 0; i < TDIHs.Length; i++)
					if (i != index)
						newTDIHSettings += TDIHs[i] + "|";

				if (newTDIHSettings != string.Empty) newTDIHSettings = newTDIHSettings.TrimEnd('|');
				TDIH = newTDIHSettings;
			}
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
			Alarms = newSetting;
			RemoveModules(index);
		}

		public static void EditWeather(int index, bool description, bool currentTemp, bool maxTemp, bool celsius)
		{
			String[] weathers = Weather.Split('|');
			weathers[index] = "weather:"
				+ ((description) ? 0 : 1) + ":"
				+ ((currentTemp) ? 0 : 1) + ":"
				+ ((maxTemp) ? 0 : 1) + ":"
				+ ((celsius) ? 0 : 1);
			
			string newWeatherSetting = "";
			foreach (String s in weathers)
				newWeatherSetting += s + "|";
			Weather = newWeatherSetting.TrimEnd('|');
		}

		public static void EditNews(int index, string category, int count)
		{
			String[] news = News.Split('|');
			news[index] = "news:" + category + ":" + count;

			string newNewsSetting = "";
			foreach (String s in news)
				newNewsSetting += s + "|";
			News = newNewsSetting.TrimEnd('|');
		}

		public static void EditTwitter(int index, string username, int count)
		{
			String[] twitters = Twitter.Split('|');
			twitters[index] = "twitter:" + username + ":" + count;

			string newTwitterSetting = "";
			foreach (String s in twitters)
				newTwitterSetting += s + "|";
			Twitter = newTwitterSetting.TrimEnd('|');
		}

		public static void EditCountdown(int index, string eventName, string date)
		{
			String[] countdowns = Countdown.Split('|');
			countdowns[index] = "countdown:" + eventName + ":" + date;

			string newCountdownSetting = "";
			foreach (String s in countdowns)
				newCountdownSetting += s + "|";
			Countdown = newCountdownSetting.TrimEnd('|');
		}

		public static void EditReminders(int index, string listName, string list)
		{
			String[] reminders = Reminders.Split('|');
			reminders[index] = "reminder:" + listName + ":" + list;

			string newRemindersSetting = "";
			foreach (String s in reminders)
				newRemindersSetting += s + "|";
			Reminders = newRemindersSetting.TrimEnd('|');
		}

		public static void EditTraffic(int index, string destName, string destUrl)
		{
			String[] traffics = Traffic.Split('|');
			traffics[index] = "traffic:" + destName + ":" + destUrl;

			string newTrafficSetting = "";
			foreach (String s in traffics)
				newTrafficSetting += s + "|";
			Traffic = newTrafficSetting.TrimEnd('|');
		}

		public static void EditFact(int index, bool on)
		{
			String[] facts = Fact.Split('|');
			facts[index] = on ? "fact:" : EMPTY_MODULE;

			string newFactSetting = "";
			foreach (String s in facts)
				newFactSetting += s + "|";
			Fact = newFactSetting.TrimEnd('|');
		}

		public static void EditQuote(int index, bool on)
		{
			String[] quotes = Quote.Split('|');
			quotes[index] = on ? "quote:" : EMPTY_MODULE;

			string newQuoteSetting = "";
			foreach (String s in quotes)
				newQuoteSetting += s + "|";
			Quote = newQuoteSetting.TrimEnd('|');
		}

		public static void EditTDIH(int index, bool on)
		{
			String[] TDIHs = TDIH.Split('|');
			TDIHs[index] = on ? "tdih:" : EMPTY_MODULE;

			string newTDIHSetting = "";
			foreach (String s in TDIHs)
				newTDIHSetting += s + "|";
			TDIH = newTDIHSetting.TrimEnd('|');
		}

		public static bool GetFact(int index)
		{
			string Fact2 = Fact;
			string[] temp = Fact2.Split('|');
			string fact = temp[index];
			return fact != EMPTY_MODULE;
		}

		public static bool GetQuote(int index)
		{
			string quote = Quote.Split('|')[index];
			return quote != EMPTY_MODULE;
		}

		public static bool GetTDIH(int index)
		{
			string tdih = TDIH.Split('|')[index];
			return tdih != EMPTY_MODULE;
		}

		public static void DeleteModule(Modules type, int index)
		{
			string[] modules;
			switch (type)
			{
				case Modules.WEATHER:
					modules = Weather.Split('|');
					break;
				case Modules.REDDIT:
					modules = Reddit.Split('|');
					break;
				case Modules.TWITTER:
					modules = Twitter.Split('|');
					break;
				case Modules.COUNTDOWN:
					modules = Countdown.Split('|');
					break;
				case Modules.REMINDERS:
					modules = Reminders.Split('|');
					break;
				case Modules.TRAFFIC:
					modules = Traffic.Split('|');
					break;
				case Modules.NEWS:
					modules = News.Split('|');
					break;
				case Modules.FACT:
					modules = Fact.Split('|');
					break;
				case Modules.QUOTE:
					modules = Quote.Split('|');
					break;
				default:
					modules = TDIH.Split('|');
					break;
			}

			//Remove
			if (type == Modules.FACT || type == Modules.QUOTE || type == Modules.TDIH)
			{
				modules[index] = "0";
			}
			else {
				modules[index] = EMPTY_MODULE;
			}
			string newSetting = "";
			foreach (String s in modules)
				newSetting += s + "|";

			switch (type)
			{
				case Modules.WEATHER:
					Weather = newSetting.TrimEnd('|');
					break;
				case Modules.REDDIT:
					Reddit = newSetting.TrimEnd('|');
					break;
				case Modules.TWITTER:
					Twitter = newSetting.TrimEnd('|');
					break;
				case Modules.COUNTDOWN:
					Countdown = newSetting.TrimEnd('|');
					break;
				case Modules.REMINDERS:
					Reminders = newSetting.TrimEnd('|');
					break;
				case Modules.TRAFFIC:
					Traffic = newSetting.TrimEnd('|');
					break;
				case Modules.NEWS:
					News = newSetting.TrimEnd('|');
					break;
				case Modules.FACT:
					Fact = newSetting.TrimEnd('|');
					break;
				case Modules.QUOTE:
					Quote = newSetting.TrimEnd('|');
					break;
				default:
					TDIH = newSetting.TrimEnd('|');
					break;
			}
		}

		public static void EditModuleOrder(int index, string newOrder)
		{
			String[] moduleOrders = ModuleOrder.Split('|');
			moduleOrders[index] = newOrder;

			string newModuleOrder = "";
			foreach (String s in moduleOrders)
				newModuleOrder += s + "|";
			ModuleOrder = newModuleOrder.TrimEnd('|');
		}

		public static string[] GetActiveModules(int index)
		{
			string moduleOrder = ModuleOrder.Split('|')[index];
			string[] order = moduleOrder.Split(':');
			List<string> final = new List<string>();
			foreach (string m in order)
			{
				string setting = EMPTY_MODULE;
				switch (m)
				{
					case "weather":
						setting = Weather.Split('|')[index];
						break;
					case "reddit":
						setting = Reddit.Split('|')[index];
						break;
					case "news":
						setting = News.Split('|')[index];
						break;
					case "twitter":
						setting = Twitter.Split('|')[index];
						break;
					case "traffic":
						setting = Traffic.Split('|')[index];
						break;
					case "countdown":
						setting = Countdown.Split('|')[index];
						break;
					case "reminders":
						setting = Reminders.Split('|')[index];
						break;
					case "fact":
						setting = Fact.Split('|')[index];
						break;
					case "quote":
						setting = Quote.Split('|')[index];
						break;
					case "tdih":
						setting = TDIH.Split('|')[index];
						break;
				}
				if (setting != EMPTY_MODULE)
					final.Add(setting);
			}
			return final.ToArray();
		}
  }
}