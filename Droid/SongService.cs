using System;
using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;

namespace Clockwise.Droid
{
	[Service]
	public class SongService : Service
	{
		public static Intent intent;
		public static MediaPlayer player;

		public override void OnDestroy()
		{
			base.OnDestroy();
			if (player != null && player.IsPlaying) player.Stop();
		}

		public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
		{
			player = MediaPlayer.Create(this, Android.Net.Uri.Parse(intent.GetStringExtra("songUri")));
			player.Looping = true;
			player.Start();
			return base.OnStartCommand(intent, flags, startId);
		}

		static void setVolume(float l, float r, Context c)
		{
			if (player != null) player.SetVolume(l, r);
		}

		public override IBinder OnBind(Intent intent)
		{
			return null;
		}
	}
}
