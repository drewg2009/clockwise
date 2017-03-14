using System;
using System.Collections.Generic;
using Android.Content;
using Android.Provider;

namespace Clockwise.Droid
{
	public class SongManager
	{
		List<Song> songList;
		Context c;
		public int playingIndex = -1;
		private static SongManager instance;

		private SongManager(Context c)
		{
			this.songList = new List<Song>();
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
					String thisArtist = musicCursor.GetString(artistColumn);
					Android.Net.Uri contentUri = ContentUris.WithAppendedId(
						MediaStore.Audio.Media.ExternalContentUri, thisId);
					long thisAlbumId = musicCursor.GetLong(albumIdColumn);
					songList.Add(new Song(c, thisId, thisTitle, thisArtist, contentUri, thisAlbumId));
				}
				while (musicCursor.MoveToNext());
				musicCursor.Close();
			}
			return songList;
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

		void stop()
		{
			if (ServiceHelper.IsMyServiceRunning(typeof(SongService), c)) {
	            c.StopService(SongService.intent);
	        }
	    }
	}
}
