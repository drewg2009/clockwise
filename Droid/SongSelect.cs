
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
using Android.Graphics;
namespace Clockwise.Droid
{
	[Activity(Label = "SongSelect")]
	public class SongSelect : Activity
	{
		List<Song> songList;
		RadioGroup radioList;
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.SongSelect);

			radioList = FindViewById<RadioGroup>(Resource.Id.song_radio_group);
			songList = ManageAlarms.songList;
			if (songList != null)
			{
				Typeface font = Typeface.CreateFromAsset(ApplicationContext.Resources.Assets, "HelveticaNeueLight.ttf");

				foreach (Song s in songList)
				{
					RadioButton rb = new RadioButton(this);
					rb.Text = s.Title;
					rb.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
					rb.SetTextColor(Color.Gray);
					rb.Typeface = font;
					radioList.AddView(rb);
				}

			}

		}
	}
}
