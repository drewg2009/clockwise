using System.Collections.Generic;
using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace Clockwise.Droid
{
	public class TonesRecyclerViewAdapter : RecyclerView.Adapter
	{
		Context context;
		List<Song> songList;
		List<Song> defaultsList;
		SongManager sm;
		static RadioButton selectedRadio;
		int page, alarmIndex;
		public TonesRecyclerViewAdapter(Context c, int p, SongManager sm, int alarmIndex)
		{
			context = c;
			songList = SongManager.getInstance(c).getSongList();
			defaultsList = SongManager.getInstance(c).getDefaultList();

			page = p;
			this.sm = sm;
			this.alarmIndex = alarmIndex;
		}

		public override int ItemCount
		{
			get
			{
				if (page == 0)
				{
					return defaultsList != null ? defaultsList.Count : 0;
				}
				else
				{
					return songList != null ? songList.Count : 0;
				}
			}
		}

		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
			if (page == 0)
			{
				Song s = defaultsList[position];
				var row = holder as CustomViewHolder;
				row.Title.Text = s.Title;
				row.ItemView.Click += delegate
                {
                    //play
                    if (sm.isPlaying()) sm.stop();
					sm.play(defaultsList[position].getUri().ToString());
					sm.playingIndex = position;
					if (alarmIndex != -1)
						Helpers.Settings.SetAlarmField(alarmIndex, Helpers.Settings.AlarmField.Song,
												   defaultsList[position].getUri().ToString());
					else SongSelect.result = defaultsList[position].Title + "|" + defaultsList[position].getUri();
					if (selectedRadio != null) selectedRadio.Checked = false;
					row.RadioButton.Checked = true;
					selectedRadio = row.RadioButton;
                };

				if (alarmIndex != -1 && defaultsList[position].getUri().ToString()
				   == Helpers.Settings.GetAlarmField(alarmIndex, Helpers.Settings.AlarmField.Song))
				{
					row.RadioButton.Checked = true;
					selectedRadio = row.RadioButton;
				}
			}
			else
			{
				Song s = songList[position];
				var row = holder as CustomViewHolder;
				row.Title.Text = s.Title;
				row.ItemView.Click += delegate
                {
                    //play
                    if (sm.isPlaying()) sm.stop();
					sm.play(songList[position].getUri().ToString());
					sm.playingIndex = position;
					if(alarmIndex != -1)
						Helpers.Settings.SetAlarmField(alarmIndex, Helpers.Settings.AlarmField.Song,
					                               songList[position].getUri().ToString());
					else SongSelect.result = songList[position].Title + "|" + songList[position].getUri();
					
					if (selectedRadio != null) selectedRadio.Checked = false;
					row.RadioButton.Checked = true;
					selectedRadio = row.RadioButton;
                };

				if (alarmIndex != -1 && songList[position].getUri().ToString()
				   == Helpers.Settings.GetAlarmField(alarmIndex, Helpers.Settings.AlarmField.Song))
				{
					row.RadioButton.Checked = true;
					selectedRadio = row.RadioButton;
				}
				
				row.Image.SetImageBitmap(s.getAlbumArt());
			}
		}



		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			View v = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.tones_page_row, null);
			CustomViewHolder holder = new CustomViewHolder(v);
			return holder;
		}

		public class CustomViewHolder : RecyclerView.ViewHolder
		{
			public ImageView Image { get; set;}
			public TextView Title { get; set;}
			public RadioButton RadioButton { get; set; }
			public CustomViewHolder(View v) : base (v)
			{
				ItemView = v;
				Image = v.FindViewById<ImageView>(Resource.Id.tone_image);
				Title = v.FindViewById<TextView>(Resource.Id.tone_title);
				RadioButton = v.FindViewById<RadioButton>(Resource.Id.tone_radio_button);
			}
		}
	}
}
