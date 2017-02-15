
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Content.PM;

namespace Clockwise.Droid
{
	[BroadcastReceiver (Process = ":remote", Enabled = false)]
	public class AlarmReceiver : BroadcastReceiver
	{
		public override void OnReceive(Context context, Intent intent)
		{
			Console.WriteLine("alarm fired");
			if (context.CheckSelfPermission(Android.Manifest.Permission.ReadExternalStorage)
				== Permission.Granted)
			{

			}
			else {
				//Play default tone
			}
			Toast.MakeText(context, "Received intent!", ToastLength.Short).Show();
			AlarmUtils.PostNotification(context);
		}
	}
}
