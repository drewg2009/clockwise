using System;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Java.IO;

namespace Clockwise.Droid
{
	public class Song
	{
		private long id;
		public String Title;
		private String artist;
		private Android.Net.Uri songUri;
		private Context context;
		private long albumId;
		public Song(Context c, long songID, String songTitle, String songArtist, Android.Net.Uri songUri, long albumId)
		{
			id = songID;
			Title = songTitle;
			artist = songArtist;
			this.songUri = songUri;
			context = c;
			this.albumId = albumId;
		}


		public long getID()
		{
			return id;
		}


		public String getArtist()
		{
			return artist;
		}

		public Android.Net.Uri getUri()
		{
			return songUri;
		}

		public Bitmap getAlbumArt()
		{
			Bitmap bm = null;
			try
			{
				BitmapFactory.Options opts = new BitmapFactory.Options();
				opts.InSampleSize = 8;
				Android.Net.Uri sArtworkUri = Android.Net.Uri.Parse("content://media/external/audio/albumart");
				Android.Net.Uri uri = ContentUris.WithAppendedId(sArtworkUri, albumId);
				ParcelFileDescriptor pfd = context.ContentResolver.OpenFileDescriptor(uri, "r");

				if (pfd != null)
				{
					FileDescriptor fd = pfd.FileDescriptor;
					//bm = BitmapFactory.DecodeFileDescriptor(fd);
					bm = BitmapFactory.DecodeFileDescriptor(fd, null, opts);
				}
			}
			catch (Exception e)
			{
				//Console.WriteLine(e.StackTrace);
			}
			return bm;
		}
	}
}
