using System;
using Android.App;
using Android.OS;
using Android.Content;
namespace Clockwise.Droid
{
	[Service]
	public class SpeechService : Service
	{
		public override void OnCreate()
		{
			base.OnCreate();
			//Cancel notification

			//Stop song


		}

		public override IBinder OnBind(Intent intent)
		{
			return null;
		}
	}
}
