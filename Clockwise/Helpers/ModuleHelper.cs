using System;
using System.Collections.Generic;
using Clockwise.Helpers;
namespace Clockwise
{
	public class ModuleHelper
	{
		public static string[] GetActiveModules()
		{
			string moduleOrder = Settings.ModuleOrder;
			string[] order = moduleOrder.Split('|');
			List<string> final = new List<string>();
			foreach (string m in order)
			{
				switch (m)
				{
					case "weather":
						final.Add(Settings.Weather);
						break;
					case "reddit":
						final.Add(Settings.Reddit);
						break;
					case "news":
						final.Add(Settings.News);
						break;
					case "twitter":
						final.Add(Settings.Twitter);
						break;
					case "traffic":
						final.Add(Settings.Traffic);
						break;
					case "countdown":
						final.Add(Settings.Countdown);
						break;
					case "reminders":
						final.Add(Settings.Reminders);
						break;
					case "fact":
						final.Add(Settings.Fact);
						break;
					case "quote":
						final.Add(Settings.Quote);
						break;
					case "tdih":
						final.Add(Settings.TDIH);
						break;
				}
			}
			return final.ToArray();
		}
	}
}
