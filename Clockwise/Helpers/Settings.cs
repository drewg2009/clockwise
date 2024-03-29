// Helpers/Settings.cs
using System;
using Newtonsoft.Json;
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

		private const string AdsHomeScreenKey = "ads_homescreen_key";
        private static readonly int AdsHomeScreenDef = 0;

		private const string AdsCreateModuleKey = "ads_create_module_key";
		private static readonly int AdsCreateModuleDef = 0;

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

		private const string AndroidStartSongKey = "android_start_song_key";
		private static readonly string AndroidStartSongDef = string.Empty;

		private const string AndroidStartSongUriKey = "android_start_song_uri_key";
		private static readonly string AndroidStartSongUriDef = string.Empty;

		private static string DefaultModuleOrder =
			"weather:news:reddit:twitter:countdown:reminders:traffic:fact:quote:tdih";
		public enum AlarmStatus { ALARM_ON, ALARM_OFF };

		public enum Modules { WEATHER, REDDIT, TWITTER, NEWS, COUNTDOWN, REMINDERS, TRAFFIC, FACT, QUOTE, TDIH };
		public enum AlarmField { Index, Hour, Minute, RepeatDays, Status, Snooze, Volume, Song, Name, Millis };
		public static string EMPTY_MODULE = "null";
		public static string SEPARATERS = "#:/;|";
		#endregion


		public static string Alarms
		{
			get { return AppSettings.GetValueOrDefault<string>(AlarmsKey, AlarmsDef); }
			set { AppSettings.AddOrUpdateValue<string>(AlarmsKey, value); }
		}

        public static int AdsHomeScreen{
            get { return AppSettings.GetValueOrDefault<int>(AdsHomeScreenKey, AdsHomeScreenDef); }
            set { AppSettings.AddOrUpdateValue<int>(AdsHomeScreenKey, value); }
        }

		public static int AdsCreateModule
		{
            get { return AppSettings.GetValueOrDefault<int>(AdsCreateModuleKey, AdsCreateModuleDef); }
            set { AppSettings.AddOrUpdateValue<int>(AdsCreateModuleKey, value); }
		}

		public static string AndroidStartSong
		{
			get { return AppSettings.GetValueOrDefault<string>(AndroidStartSongKey, AndroidStartSongDef); }
			set { AppSettings.AddOrUpdateValue<string>(AndroidStartSongKey, value); }
		}

		public static string AndroidStartSongUri
		{
			get { return AppSettings.GetValueOrDefault<string>(AndroidStartSongUriKey, AndroidStartSongUriDef); }
			set { AppSettings.AddOrUpdateValue<string>(AndroidStartSongUriKey, value); }
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

		public static void EditAlarm(int index, int hour, int minute, int repeatDays, long millis, int snooze = 10, int volume = 10, string name = "null", string song = "null")
		{
			String[] currentAlarms = Settings.Alarms.Split('|');
			if (name == string.Empty) name = EMPTY_MODULE;
			//Create new alarm
			AlarmStatus status = AlarmStatus.ALARM_ON;
			if (Alarms == string.Empty)
			{
				Alarms = "" + index + "#" + hour + "#" + minute + "#" + repeatDays + "#"
					+ (int)status + "#" + snooze + "#" + volume + "#" + song + "#" + name + "#" + millis;
				ModuleOrder = DefaultModuleOrder;
				AddModules();
			}
			else if (index == currentAlarms.Length)
			{
				string newAlarm = "" + index + "#" + hour + "#" + minute + "#" + repeatDays + "#"
					+ (int)status + "#" + snooze + "#" + volume + "#" + song + "#" + name + "#" + millis;
				Alarms += "|" + newAlarm;
				AddModules();
			}
			//Change old alarm
			else
			{
				currentAlarms[index] = "" + index + "#" + hour + "#" + minute + "#" + repeatDays +
					"#" + (int)status + "#" + GetAlarmField(index, AlarmField.Snooze) + "#" + GetAlarmField(index, AlarmField.Volume) + "#"
					+ GetAlarmField(index, AlarmField.Song) + "#"
					+ GetAlarmField(index, AlarmField.Name) + "#"
					+ GetAlarmField(index, AlarmField.Millis);
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

			Alarms = newSetting.TrimEnd('|'); ;
			RemoveModules(index);
		}

		public static void DeleteModule(int index, Modules type, int subindex = -1)
		{
			switch (type)
			{
				case Modules.COUNTDOWN:
					DeleteCountdown(index, subindex);
					break;
				case Modules.FACT:
					EditFact(index, false);
					break;
				case Modules.NEWS:
					DeleteNews(index, subindex);
					break;
				case Modules.QUOTE:
					EditQuote(index, false);
					break;
				case Modules.REDDIT:
                    DeleteReddit(index, subindex);
					break;
				case Modules.REMINDERS:
					DeleteReminders(index, subindex);
					break;
				case Modules.TDIH:
					EditTDIH(index, false);
					break;
				case Modules.TRAFFIC:
                    DeleteTraffic(index, subindex);
					break;
				case Modules.TWITTER:
                    DeleteTwitter(index, subindex);
					break;
				case Modules.WEATHER:
					DeleteWeather(index);
					break;
			}
		}

		public static void EditWeather(int index, bool fahren, bool currentTemp, bool maxTemp, bool description)
		{
			String[] weathers = Weather.Split('|');
			weathers[index] = "weather:"
				+ ((fahren) ? 0 : 1) + ":"
				+ ((currentTemp) ? 0 : 1) + ":"
				+ ((maxTemp) ? 0 : 1) + ":"
				+ ((description) ? 0 : 1);

			string newWeatherSetting = "";
			foreach (String s in weathers)
				newWeatherSetting += s + "|";
			Weather = newWeatherSetting.TrimEnd('|');
		}

		public static bool AddNews(int index, string category, int count)
		{
			String[] news = News.Split('|');
			string newNewsSetting = category + ":" + count;
			if (GetNews(index).Contains(category))
			{
				return false;
			}
			else
			{
				if (news[index] == EMPTY_MODULE)
					news[index] = "news:" + newNewsSetting;
				else
					news[index] += "," + newNewsSetting;

				string newSetting = string.Empty;
				foreach (String s in news)
					newSetting += s + "|";

				News = newSetting.TrimEnd('|');
				return true;
			}
		}

		public static bool AddTwitter(int index, string username, int count)
		{
			String[] twitters = Twitter.Split('|');
			string newTwitterSetting = username + ":" + count;
			if (GetTwitter(index).Contains(username))
			{
				return false;
			}
			else
			{
				if (twitters[index] == EMPTY_MODULE)
					twitters[index] = "twitter:" + newTwitterSetting;
				else
					twitters[index] += "," + newTwitterSetting;

				string newSetting = string.Empty;
				foreach (String s in twitters)
					newSetting += s + "|";

				Twitter = newSetting.TrimEnd('|');
				return true;
			}
		}

		public static bool AddReddit(int index, string subreddit, int count)
		{
			String[] reddits = Reddit.Split('|');
			string newRedditSetting = subreddit + ":" + count;

			if (GetReddit(index).Contains(subreddit))
			{
				return false;
			}
			else
			{
				if (reddits[index] == EMPTY_MODULE)
					reddits[index] = "reddit:" + newRedditSetting;
				else
					reddits[index] += "," + newRedditSetting;

				string newSetting = string.Empty;
				foreach (String s in reddits)
					newSetting += s + "|";

				Reddit = newSetting.TrimEnd('|');
				return true;
			}
		}

		public static bool AddCountdown(int index, string eventName, string date)
		{
			String[] countdowns = Countdown.Split('|');
			string newCountdownSetting = eventName + ":" + date;

			if (GetCountdown(index).Contains(eventName))
			{
				return false;
			}
			else
			{
				if (countdowns[index] == EMPTY_MODULE)
					countdowns[index] = "countdown:" + newCountdownSetting;
				else
					countdowns[index] += "," + newCountdownSetting;

				string newSetting = string.Empty;
				foreach (String s in countdowns)
					newSetting += s + "|";

				Countdown = newSetting.TrimEnd('|');
				return true;
			}
		}

		public static bool AddReminders(int index, string listName, string list)
		{
			String[] reminders = Reminders.Split('|');
			string newRemindersSeting = listName + ":" + list;
			string thisReminder = GetReminders(index);
			if(thisReminder != EMPTY_MODULE)
				thisReminder = thisReminder.Substring(10);
			if (GetReminders(index).Contains(listName + ":"))
			{
				return false;
			}
			else
			{
				if (reminders[index] == EMPTY_MODULE)
					reminders[index] = "reminders:" + newRemindersSeting;
				else
					reminders[index] += "," + newRemindersSeting;

				string newSetting = string.Empty;
				foreach (String s in reminders)
					newSetting += s + "|";

				Reminders = newSetting.TrimEnd('|');
				return true;
			}
		}

		public static bool AddTraffic(int index, string destName, float startLat, float startLon, float destLat, float destLon, string mode)
		{
			String[] traffics = Traffic.Split('|');
			string newTrafficSetting = destName + ":" + startLat + ":" + startLon + ":" + destLat + ":" +destLon + ":" + mode;

			if (GetTraffic(index).Contains(destName))
			{
				return false;
			}
			else
			{
				if (traffics[index] == EMPTY_MODULE)
					traffics[index] = "traffic:" + newTrafficSetting;
				else
					traffics[index] += "," + newTrafficSetting;

				string newSetting = string.Empty;
				foreach (String s in traffics)
					newSetting += s + "|";

				Traffic = newSetting.TrimEnd('|');
				return true;
			}
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

		public static bool EditReddit(int index, int subindex, string subreddit, int count)
		{
			string[] reddits = Reddit.Split('|');
			if (GetReddit(index).Contains(subreddit))
			   return false;
			string thisReddit = reddits[index]; //reddit:subreddit:count,subreddit:count, ...
			List<string> moduleList = new List<string>(thisReddit.Substring(thisReddit.IndexOf(':') + 1).Split(','));
			moduleList[subindex] = subreddit + ":" + count;

			string newList = string.Empty;
			foreach (String s in moduleList)
				newList += s + ",";

			reddits[index] = "reddit:" + newList.TrimEnd(',');

			string newSetting = string.Empty;
			foreach (String s in reddits)
				newSetting += s + "|";

			Reddit = newSetting.TrimEnd('|');
			return true;
		}

		public static bool EditTwitter(int index, int subindex, string username, int count)
		{
			if (GetTwitter(index).Contains(username))
				return false;
			string[] twitters = Twitter.Split('|');
			string thisTwitter = twitters[index]; //twitter:subreddit:count,subreddit:count, ...
			List<string> moduleList = new List<string>(thisTwitter.Substring(thisTwitter.IndexOf(':') + 1).Split(','));
			moduleList[subindex] = username + ":" + count;

			string newList = string.Empty;
			foreach (String s in moduleList)
				newList += s + ",";

			twitters[index] = "twitter:" + newList.TrimEnd(',');

			string newSetting = string.Empty;
			foreach (String s in twitters)
				newSetting += s + "|";

			Twitter = newSetting.TrimEnd('|');
			return true;
		}

		public static bool EditCountdown(int index, int subindex, string eventName, string date)
		{
			if (GetCountdown(index).Contains(eventName))
				return false;
			string[] countdowns = Countdown.Split('|');
			string thisCountdown = countdowns[index]; //countdown:subreddit:count,subreddit:count, ...
			List<string> moduleList = new List<string>(thisCountdown.Substring(thisCountdown.IndexOf(':') + 1).Split(','));
			moduleList[subindex] = eventName + ":" + date;

			string newList = string.Empty;
			foreach (String s in moduleList)
				newList += s + ",";

			countdowns[index] = "countdown:" + newList.TrimEnd(',');

			string newSetting = string.Empty;
			foreach (String s in countdowns)
				newSetting += s + "|";

			Countdown = newSetting.TrimEnd('|');
			return true;
		}

		public static bool EditTraffic(int index, int subindex, string destName, float startLat, float startLon, float destLat, float destLon, string mode)
		{
			if (GetTraffic(index).Contains(destName))
				return false;
			string[] traffics = Traffic.Split('|');
			string thisTraffic = traffics[index]; //traffic:subreddit:count,subreddit:count, ...
			List<string> moduleList = new List<string>(thisTraffic.Substring(thisTraffic.IndexOf(':') + 1).Split(','));
			moduleList[subindex] = destName + ":" + startLat + ":" + startLon + ":" + destLat + ":" +destLon + ":" + mode;

			string newList = string.Empty;
			foreach (String s in moduleList)
				newList += s + ",";

			traffics[index] = "traffic:" + newList.TrimEnd(',');

			string newSetting = string.Empty;
			foreach (String s in traffics)
				newSetting += s + "|";

			Traffic = newSetting.TrimEnd('|');
			return true;
		}

		public static bool EditReminders(int index, int subindex, string listName, string list)
		{
			string thisReminder = GetReminders(index);
			if(thisReminder != EMPTY_MODULE)
				thisReminder = thisReminder.Substring(10);
			if (GetReminders(index).Contains(listName + ":"))
			{
				return false;
			}
			string[] reminders = Reminders.Split('|');
			string thisReminders = reminders[index];
			List<string> moduleList = new List<string>(thisReminders.Substring(thisReminders.IndexOf(':') + 1).Split(','));
			moduleList[subindex] = listName + ":" + list;

			string newList = string.Empty;
			foreach (String s in moduleList)
				newList += s + ",";

			reminders[index] = "reminders:" + newList.TrimEnd(',');

			string newSetting = string.Empty;
			foreach (String s in reminders)
				newSetting += s + "|";

			Reminders = newSetting.TrimEnd('|');
			return true;
		}

		public static bool EditNews(int index, int subindex, string category, int count)
		{
			if (GetNews(index).Contains(category))
				return false;
			string[] news = News.Split('|');
			string thisNews = news[index]; //twitter:subreddit:count,subreddit:count, ...
			List<string> moduleList = new List<string>(thisNews.Substring(thisNews.IndexOf(':') + 1).Split(','));
			moduleList[subindex] = category + ":" + count;

			string newList = string.Empty;
			foreach (String s in moduleList)
				newList += s + ",";

			news[index] = "news:" + newList.TrimEnd(',');

			string newSetting = string.Empty;
			foreach (String s in news)
				newSetting += s + "|";

			News = newSetting.TrimEnd('|');
			return true;
		}

		public static bool GetFact(int index)
		{
			string fact = Fact.Split('|')[index];
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

		public static string GetWeather(int index)
		{
			return Weather.Split('|')[index];
		}

		public static string GetNews(int index)
		{
			return News.Split('|')[index];
		}

		public static string GetTwitter(int index)
		{
			return Twitter.Split('|')[index];
		}

		public static string GetCountdown(int index)
		{
			return Countdown.Split('|')[index];
		}

		public static string GetTraffic(int index)
		{
			return Traffic.Split('|')[index];
		}

		public static string GetReminders(int index)
		{
			return Reminders.Split('|')[index];
		}

		public static string GetReddit(int index)
		{
			return Reddit.Split('|')[index];
		}

		//------
		public static string GetNews(int index, int subindex)
		{
			if (News != EMPTY_MODULE)
			{
				string thisNews = News.Split('|')[index];
				if (thisNews != EMPTY_MODULE)
				{
					string[] moduleList = thisNews.Substring(thisNews.IndexOf(':') + 1).Split(',');
					if (subindex<moduleList.Length)
						return moduleList[subindex];
					else return string.Empty;
				}
				else return string.Empty;
			}
			else return string.Empty;
		}

		public static string GetTwitter(int index, int subindex)
		{
			if (Twitter != EMPTY_MODULE)
			{
				string thisTwitter = Twitter.Split('|')[index];
				if (thisTwitter != EMPTY_MODULE)
				{
					string[] moduleList = thisTwitter.Substring(thisTwitter.IndexOf(':') + 1).Split(',');
					if (subindex<moduleList.Length)
						return moduleList[subindex];
					else return string.Empty;
				}
				else return string.Empty;
			}
			else return string.Empty;
		}

		public static string GetCountdown(int index, int subindex)
		{
			if (Countdown != EMPTY_MODULE)
			{
				string thisCountdown = Countdown.Split('|')[index];
				if (thisCountdown != EMPTY_MODULE)
				{
					string[] moduleList = thisCountdown.Substring(thisCountdown.IndexOf(':') + 1).Split(',');
					if (subindex < moduleList.Length)
						return moduleList[subindex];
					else return string.Empty;
				}
				else return string.Empty;
			}
			else return string.Empty;
		}

		public static string GetTraffic(int index, int subindex)
		{
			if (Traffic != EMPTY_MODULE)
			{
				string thisTraffic = Traffic.Split('|')[index];
				if (thisTraffic != EMPTY_MODULE)
				{
					string[] moduleList = thisTraffic.Substring(thisTraffic.IndexOf(':') + 1).Split(',');
					if (subindex < moduleList.Length)
						return moduleList[subindex];
					else return string.Empty;
				}
				else return string.Empty;
			}
			else return string.Empty;
		}

		public static string GetReminders(int index, int subindex)
		{
			if (Reminders != EMPTY_MODULE)
			{
				string thisReminders = Reminders.Split('|')[index];
				if (thisReminders != EMPTY_MODULE)
				{
					string[] moduleList = thisReminders.Substring(thisReminders.IndexOf(':') + 1).Split(',');
					if (subindex < moduleList.Length)
						return moduleList[subindex];
					else return string.Empty;
				}
				else return string.Empty;

			}
			else return string.Empty;
		}

		public static string GetReddit(int index, int subindex)
		{
			if (Reddit != EMPTY_MODULE)
			{
				string thisReddit = Reddit.Split('|')[index];
				if (thisReddit != EMPTY_MODULE)
				{
					string[] moduleList = thisReddit.Substring(thisReddit.IndexOf(':') + 1).Split(',');
					if (subindex < moduleList.Length)
						return moduleList[subindex];
					else return string.Empty;
				}
				else return string.Empty;

			}
			else return string.Empty;
		}
		//------

		public static void DeleteWeather(int index)
		{
			string[] weathers = Weather.Split('|');
			weathers[index] = EMPTY_MODULE;

			string newSetting = string.Empty;
			foreach (String s in weathers)
				newSetting += s + "|";

			Weather = newSetting.TrimEnd('|');
		}

		public static void DeleteReddit(int index, int subindex)
		{
			string[] reddits = Reddit.Split('|');
			string thisReddit = reddits[index]; //reddit:subreddit:count,subreddit:count, ...
			List<string> moduleList = new List<string>(thisReddit.Substring(thisReddit.IndexOf(':') + 1).Split(','));
			moduleList.RemoveAt(subindex);

			string newList = string.Empty;
			foreach (String s in moduleList)
				newList += s + ",";

			if (newList != string.Empty)
				reddits[index] = "reddit:" + newList.TrimEnd(',');
			else
				reddits[index] = EMPTY_MODULE;

			string newSetting = string.Empty;
			foreach (String s in reddits)
				newSetting += s + "|";

			Reddit = newSetting.TrimEnd('|');
		}

		public static void DeleteTwitter(int index, int subindex)
		{
			string[] twitters = Twitter.Split('|');
			string thisTwitter = twitters[index]; //twitter:subreddit:count,subreddit:count, ...
			List<string> moduleList = new List<string>(thisTwitter.Substring(thisTwitter.IndexOf(':') + 1).Split(','));
			moduleList.RemoveAt(subindex);

			string newList = string.Empty;
			foreach (String s in moduleList)
				newList += s + ",";

			if (newList != string.Empty)
				twitters[index] = "twitter:" + newList.TrimEnd(',');
			else
				twitters[index] = EMPTY_MODULE;

			string newSetting = string.Empty;
			foreach (String s in twitters)
				newSetting += s + "|";

			Twitter = newSetting.TrimEnd('|');
		}

		public static void DeleteCountdown(int index, int subindex)
		{
			string[] countdowns = Twitter.Split('|');
			string thisCountdown = countdowns[index]; //countdown:subreddit:count,subreddit:count, ...
			List<string> moduleList = new List<string>(thisCountdown.Substring(thisCountdown.IndexOf(':') + 1).Split(','));
			moduleList.RemoveAt(subindex);

			string newList = string.Empty;
			foreach (String s in moduleList)
				newList += s + ",";

			if (newList != string.Empty)
				countdowns[index] = "countdown:" + newList.TrimEnd(',');
			else
				countdowns[index] = EMPTY_MODULE;

			string newSetting = string.Empty;
			foreach (String s in countdowns)
				newSetting += s + "|";

			Countdown = newSetting.TrimEnd('|');
		}

		public static void DeleteTraffic(int index, int subindex)
		{
			string[] traffics = Traffic.Split('|');
			string thisTraffic = traffics[index]; //traffic:subreddit:count,subreddit:count, ...
			List<string> moduleList = new List<string>(thisTraffic.Substring(thisTraffic.IndexOf(':') + 1).Split(','));
			moduleList.RemoveAt(subindex);

			string newList = string.Empty;
			foreach (String s in moduleList)
				newList += s + ",";

			if (newList != string.Empty)
				traffics[index] = "traffic:" + newList.TrimEnd(',');
			else
				traffics[index] = EMPTY_MODULE;

			string newSetting = string.Empty;
			foreach (String s in traffics)
				newSetting += s + "|";

			Traffic = newSetting.TrimEnd('|');
		}

		public static void DeleteReminders(int index, int subindex)
		{
			string[] reminders = Twitter.Split('|');
			string thisReminders = reminders[index]; //reminders:subreddit:count,subreddit:count, ...
			List<string> moduleList = new List<string>(thisReminders.Substring(thisReminders.IndexOf(':') + 1).Split(','));
			moduleList.RemoveAt(subindex);

			string newList = string.Empty;
			foreach (String s in moduleList)
				newList += s + ",";

			if (newList != string.Empty)
				reminders[index] = "reminders:" + newList.TrimEnd(',');
			else
				reminders[index] = EMPTY_MODULE;

			string newSetting = string.Empty;
			foreach (String s in reminders)
				newSetting += s + "|";

			Reminders = newSetting.TrimEnd('|');
		}

		public static void DeleteNews(int index, int subindex)
		{
			string[] news = Twitter.Split('|');
			string thisNews = news[index]; //twitter:subreddit:count,subreddit:count, ...
			List<string> moduleList = new List<string>(thisNews.Substring(thisNews.IndexOf(':') + 1).Split(','));
			moduleList.RemoveAt(subindex);

			string newList = string.Empty;
			foreach (String s in moduleList)
				newList += s + ",";

			if (newList != string.Empty)
				news[index] = "news:" + newList.TrimEnd(',');
			else
				news[index] = EMPTY_MODULE;

			string newSetting = string.Empty;
			foreach (String s in news)
				newSetting += s + "|";

			News = newSetting.TrimEnd('|');
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
			else
			{
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

		public static void EditModuleOrder(string newOrder)
		{
			ModuleOrder = newOrder;
		}

		public static void SetAlarmField(int index, AlarmField field, string newValue)
		{
			string[] alarms = Alarms.Split('|');
			string[] alarmSettings = alarms[index].Split('#');
			alarmSettings[(int)field] = newValue;

			string temp = string.Empty;
			for (int i = 0; i < alarmSettings.Length; i++)
			{
				temp += alarmSettings[i] + "#";
			}

			alarms[index] = temp.TrimEnd('#');

			string newSettings = string.Empty;
			foreach (String s in alarms)
			{
				newSettings += s + "|";
			}
			Alarms = newSettings.TrimEnd('|');
		}

		public static string GetAlarmField(int index, AlarmField field)
		{
			if (Alarms != string.Empty)
			{
				string[] alarms = Alarms.Split('|');
				if (index < alarms.Length)
					return Alarms.Split('|')[index].Split('#')[(int)field];
				else return null;
			}
			else return null;
		}

		public static string[] GetActiveModules(int index)
		{
			string moduleOrder = ModuleOrder;
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

		public static bool IsNewAlarmTime(long millis, int exceptionIndex = -1)
		{
			int alarmIndex = 0;
			string number = GetAlarmField(alarmIndex, AlarmField.Millis);
			while (number != null)
			{
				if (Math.Abs(Int64.Parse(number) - millis) < 20000 && alarmIndex != exceptionIndex)
				{
					
					return false;
				}
				alarmIndex++;
				number = GetAlarmField(alarmIndex, AlarmField.Millis);
			}
			return true;
		}

		public static bool IsAlarmOn(int index)
		{
			return int.Parse(Settings.GetAlarmField(index, Settings.AlarmField.Status))
				      == (int)Settings.AlarmStatus.ALARM_ON;
		}
	}
}