using System;
using System.Collections.Generic;
using Clockwise.Helpers;
using Newtonsoft.Json;

namespace Clockwise
{
	public class JSONRequestSerializer
	{
		public string time;
		public double lat, lon;
		public Weather weather;
		public List<News> news = new List<News>();
		public List<Reddit> reddit = new List<Reddit>();
		public List<Twitter> twitter = new List<Twitter>();
		public List<Traffic> traffic = new List<Traffic>();
		public List<Reminders> reminders = new List<Reminders>();
		public List<Countdown> countdown = new List<Countdown>();

		public bool tdih = false, quote = false, fact = false;
		private static JSONRequestSerializer instance;
		private JSONRequestSerializer()
		{

		}

		public static JSONRequestSerializer GetInstance()
		{
			if (instance == null)
			{
				instance = new JSONRequestSerializer();
			}
			return instance;
		}

		public string GetJsonRequest(int index, double lat, double lon)
		{
			string weatherString = Settings.GetWeather(index);
			if(weatherString != Settings.EMPTY_MODULE)
				weather = new Weather(Settings.GetWeather(index));

			int i = 0;
			string subnews = Settings.GetNews(index, i);
			while (subnews != string.Empty)
			{
				news.Add(new News(subnews));
				subnews = Settings.GetNews(index, ++i);
			}

			i = 0;
			string subreddit = Settings.GetReddit(index, i);
			while (subreddit != string.Empty)
			{
				reddit.Add(new Reddit(subreddit));
				subreddit = Settings.GetReddit(index, ++i);
			}

			i = 0;
			string subtwitter = Settings.GetTwitter(index, i);
			while (subtwitter != string.Empty)
			{
				twitter.Add(new Twitter(subtwitter));
				subtwitter = Settings.GetTwitter(index, ++i);
			}

			i = 0;
			string subreminders = Settings.GetReminders(index, i);
			while (subreminders != string.Empty)
			{
				reminders.Add(new Reminders(subreminders));
				subreminders = Settings.GetReminders(index, ++i);
			}

			i = 0;
			string subtraffic = Settings.GetTraffic(index, i);
			while (subtraffic != string.Empty)
			{
				traffic.Add(new Traffic(subtraffic));
				subtraffic = Settings.GetTraffic(index, ++i);
			}

			i = 0;
			string subcountdown = Settings.GetCountdown(index, i);
			while (subcountdown != string.Empty)
			{
				countdown.Add(new Countdown(subcountdown));
				subcountdown = Settings.GetCountdown(index, ++i);
			}

			fact = Settings.GetFact(index);
			quote = Settings.GetQuote(index);
			tdih = Settings.GetTDIH(index);

			time = DateTime.Now.Hour + ":" + DateTime.Now.Minute;
			this.lat = lat;
			this.lon = lon;

			return Newtonsoft.Json.JsonConvert.SerializeObject(instance, Formatting.Indented);
		}


		#region Containers
		public struct Weather
		{
			public bool fahrenheit, maxTemp, description, currentTemp;
			public Weather(string weather)
			{
				string[] settings = weather.Split(':');
				description = settings[1] == "0";
				currentTemp = settings[2] == "0";
				maxTemp = settings[3] == "0";
				fahrenheit = settings[4] == "0";
			}
		}

		public struct News
		{
			public string category;
			public UInt16 amount;
			public News(string news)
			{
				string[] settings = news.Split(':');
				category = settings[0];
				amount = UInt16.Parse(settings[1]);
			}
		}

		public struct Reddit
		{
			public string subreddit;
			public UInt16 amount;
			public Reddit(string reddit)
			{
				string[] settings = reddit.Split(':');
				subreddit = settings[0];
				amount = UInt16.Parse(settings[1]);
			}
		}

		public struct Twitter
		{
			public string username;
			public UInt16 amount;
			public Twitter(string twitter)
			{
				string[] settings = twitter.Split(':');
				username = settings[0];
				amount = UInt16.Parse(settings[1]);
			}
		}

		public struct Reminders
		{
			public string name;
			public List<String> list;
			public Reminders(string reminders)
			{
				string[] settings = reminders.Split(':');
				name = settings[0];
				string[] l = settings[1].Split(';');
				list = new List<String>(l);
			}
		}

		public struct Traffic
		{
			public string name, startUrl, destUrl, mode;
			public Traffic(string traffic)
			{
				string[] settings = traffic.Split(':');
				name = settings[0];
				startUrl = settings[1];
				destUrl = settings[2];
				mode = settings[3];
			}
		}

		public struct Countdown
		{
			public string eventName;
			public string date;
			public Countdown(string countdown)
			{
				string[] settings = countdown.Split(':');
				eventName = settings[0];
				date = settings[1];
			}
		}
		#endregion
	}
}
