
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
using Android.Media;
using Android.Provider;
using Clockwise.Helpers;
namespace Clockwise.Droid
{
	[BroadcastReceiver (Enabled = false)]
	public class AlarmReceiver : BroadcastReceiver
	{
		public static SongManager sm;
		public static MediaPlayer player;

		public override void OnReceive(Context context, Intent intent)
		{
			int alarm_index = intent.GetIntExtra("alarm_index", -1);
			if (context.CheckSelfPermission(Android.Manifest.Permission.ReadExternalStorage)
				== Permission.Granted)
			{
				//Check if song exists
				bool songExists = false;
				String startSongUri = Helpers.Settings.GetAlarmField(alarm_index, Helpers.Settings.AlarmField.Song);

				Android.Net.Uri musicUri = MediaStore.Audio.Media.ExternalContentUri;
				ContentResolver musicResolver = context.ContentResolver;

				var musicCursor = musicResolver.Query(musicUri, null, null, null, null);
				if (musicCursor != null && musicCursor.MoveToFirst())
				{
					int idColumn = musicCursor.GetColumnIndex
							(MediaStore.Audio.Media.InterfaceConsts.Id);
					int titleColumn = musicCursor.GetColumnIndex
                         (MediaStore.Audio.Media.InterfaceConsts.Title);
					do
					{
						long thisId = musicCursor.GetLong(idColumn);
						Android.Net.Uri contentUri = ContentUris.WithAppendedId(
							MediaStore.Audio.Media.ExternalContentUri, thisId);
						String title = musicCursor.GetString(titleColumn);

						if (contentUri.ToString().Equals(startSongUri))
						{
							songExists = true;
							break;
						}
					}
					while (musicCursor.MoveToNext());
					musicCursor.Close();
				}

				if (songExists)
				{
					sm = SongManager.getInstance(context);
					//sm.getSongList();
					sm.play(startSongUri);
				}
				else {
					Clockwise.Helpers.Settings.AndroidStartSong = string.Empty;
					Clockwise.Helpers.Settings.AndroidStartSongUri = string.Empty;
					Helpers.Settings.SetAlarmField(alarm_index, Helpers.Settings.AlarmField.Song,
					                               Helpers.Settings.EMPTY_MODULE);

					//Play default alarm tone
					if (player == null)
					{
						AudioManager audioManager = (AudioManager)context.GetSystemService(Context.AudioService);
						player = MediaPlayer.Create(context, RingtoneManager.GetDefaultUri(RingtoneType.Alarm));
						//player = MediaPlayer.Create(context, AlarmManager.GetDefaultUri(RingtoneType.Ringtone));

						try
						{
							player.SetVolume((float)(audioManager.GetStreamVolume(Stream.Notification) / 7.0),
							                 (float)(audioManager.GetStreamVolume(Stream.Notification) / 7.0));
						}
						catch (Exception e)
						{
							Console.WriteLine(e.StackTrace);
						}
					}

					player.Looping = true;
					player.Start();
				}
			}
			else {
				//Play default alarm tone
				Clockwise.Helpers.Settings.AndroidStartSong = string.Empty;
				Clockwise.Helpers.Settings.AndroidStartSongUri = string.Empty;
				Helpers.Settings.SetAlarmField(alarm_index, Helpers.Settings.AlarmField.Song,
												   Helpers.Settings.EMPTY_MODULE);

				if (player == null)
				{
					AudioManager audioManager = (AudioManager)context.GetSystemService(Context.AudioService);
					player = MediaPlayer.Create(context, RingtoneManager.GetDefaultUri(RingtoneType.Alarm));

					try
					{
						player.SetVolume((float)(audioManager.GetStreamVolume(Stream.Notification) / 7.0),
										 (float)(audioManager.GetStreamVolume(Stream.Notification) / 7.0));
					}
					catch (Exception e)
					{
						Console.WriteLine(e.StackTrace);
					}
				}

				player.Looping = true;
				player.Start();
			}


			AlarmUtils.PostNotification(context, alarm_index);
		}
	}
}
