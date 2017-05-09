using System;
using System.Net;
using Newtonsoft.Json;

namespace Clockwise.Droid
{
	public class ApiConnector
	{
		private static ApiConnector instance = new ApiConnector();
		public static ApiConnector Instance()
		{
			return instance;
		}

		private ApiConnector() { }

		string URI = "http://phplaravel-43928-259989.cloudwaysapps.com/get/moduleData";

		public string getAlarmString()
		{
			string alarmString = "";
			//string myParameters = "&email=" + user.Email + "&password=" + user.Password;
			string myParameters = "";
			using (WebClient wc = new WebClient())
			{
				wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
				string result = wc.UploadString(URI, myParameters);
				if (result.Equals("false"))
				{

				}
				else
				{
					dynamic json = JsonConvert.DeserializeObject(result);
				}
			}
			return alarmString;
		}


	}
}
