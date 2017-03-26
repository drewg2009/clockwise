using System;
using System.Collections.Generic;
using Android.Content;
using Android.Media;
using Android.Provider;

namespace Clockwise.Droid
{
	public class SongManager
	{
		List<Song> songList;
		List<Song> defaultList;

		Context c;
		public int playingIndex = -1;
		private static SongManager instance;

		private SongManager(Context c)
		{
			this.songList = new List<Song>();
			this.defaultList = new List<Song>();
			this.c = c;
		}

		public static SongManager getInstance(Context context)
		{
			if (instance == null)
			{
				instance = new SongManager(context);
			}
			return instance;
		}

		public List<Song> getSongList()
		{
			songList = new List<Song>();
			//retrieve song info
			ContentResolver musicResolver = c.ContentResolver;
			var musicUri = MediaStore.Audio.Media.ExternalContentUri;
			var musicCursor = musicResolver.Query(musicUri, null, null, null, null);

			if (musicCursor != null && musicCursor.MoveToFirst())
			{
				//get columns
				int titleColumn = musicCursor.GetColumnIndex
                         (MediaStore.Audio.Media.InterfaceConsts.Title);
				int idColumn = musicCursor.GetColumnIndex
						(MediaStore.Audio.Media.InterfaceConsts.Id);
				int artistColumn = musicCursor.GetColumnIndex
						(MediaStore.Audio.Media.InterfaceConsts.ArtistId);
				int albumIdColumn = musicCursor.GetColumnIndex(MediaStore.Audio.Media.InterfaceConsts.AlbumId);
				//add songs to list
				do
				{
					long thisId = musicCursor.GetLong(idColumn);
					String thisTitle = musicCursor.GetString(titleColumn);
					String thisArtist = musicCursor.GetString(artistColumn);					Android.Net.Uri contentUri = ContentUris.WithAppendedId(
						MediaStore.Audio.Media.ExternalContentUri, thisId);
					long thisAlbumId = musicCursor.GetLong(albumIdColumn);
					songList.Add(new Song(c, thisId, thisTitle, thisArtist, contentUri, thisAlbumId));
				}
				while (musicCursor.MoveToNext());
				musicCursor.Close();
			}

			songList.Sort((x, y) => string.Compare(x.Title, y.Title, StringComparison.Ordinal));
			return songList;
		}

		public List<Song> getDefaultList()
		{
			defaultList = new List<Song>();
			RingtoneManager manager = new RingtoneManager(c);
			manager.SetType(RingtoneType.Alarm);
			var cursor = manager.Cursor;
			int titleColumn = cursor.GetColumnIndex
					 (MediaStore.Audio.Media.InterfaceConsts.Title);
			int idColumn = cursor.GetColumnIndex
						(MediaStore.Audio.Media.InterfaceConsts.Id);
			if (cursor != null && cursor.MoveToFirst())
			{
				do
				{
					String title = cursor.GetString(titleColumn);
					Android.Net.Uri ringtoneURI = manager.GetRingtoneUri(cursor.Position);
					long thisId = cursor.GetLong(idColumn);
					defaultList.Add(new Song(c, thisId, title, null, ringtoneURI, -1));

				}while (cursor.MoveToNext());
				cursor.Close();
			}

			defaultList.Sort((x, y) => string.Compare(x.Title, y.Title, StringComparison.Ordinal));
			return defaultList;
		}

		public void play(String songUri)
		{
			Intent i = new Intent(c, typeof(SongService));
        	i.PutExtra("songUri", songUri);
        	SongService.intent=i;
        	c.StartService(i);
    	}

		public bool isPlaying()
		{
			return SongService.player != null && SongService.player.IsPlaying;
		}

		public void setVolume(float i)
		{
			if(SongService.player != null) SongService.player.SetVolume(i, i);
		}

		public void stop()
		{
			//if (ServiceHelper.IsMyServiceRunning(typeof(SongService), c)) {
			//          c.StopService(SongService.intent);
			//      }
			if(SongService.intent != null)
				c.StopService(SongService.intent);
	    }
	}
}
