
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Clockwise.Helpers;

namespace Clockwise.Droid
{
	public class PageFragment : Android.Support.V4.App.Fragment
	{
		public static String ARG_PAGE = "ARG_PAGE";
		public static String INDEX = "INDEX";

		private int mPage;
		private int index;
		public static PageFragment newInstance(int page, int alarmIndex)
		{
			Bundle args = new Bundle();
			args.PutInt(ARG_PAGE, page);
			args.PutInt(INDEX, alarmIndex);

			PageFragment fragment = new PageFragment();
			fragment.Arguments = args;
			return fragment;
		}

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			mPage = Arguments.GetInt(ARG_PAGE);
			index = Arguments.GetInt(INDEX);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			// Use this to return your custom view for this Fragment
			// return inflater.Inflate(Resource.Layout.YourFragment, container, false);
			View view = inflater.Inflate(Resource.Layout.fragment_page, container, false);
			RadioGroup group = view.FindViewById<RadioGroup>(Resource.Id.tone_radio_group);
			if (mPage == 1)
			{
				List<Song> songList = ManageAlarms.songList;
				SongManager sm = ManageAlarms.sm;
				if (songList != null)
				{
					Typeface font = Typeface.CreateFromAsset(Context.Resources.Assets, "HelveticaNeueLight.ttf");

					for (int i = 0; i < songList.Count; i++)
					{
						RadioButton rb = new RadioButton(Context);
						rb.Text = songList[i].Title;
						rb.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
						rb.SetTextColor(Color.Gray);

						//Space
						View v = new View(Context);
						v.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent,
																		1);
						v.SetBackgroundColor(Color.Gray);
						v.SetPadding(50, 0, 0, 0);
						rb.Typeface = font;
						rb.TextSize = 22;
						rb.SetPadding(0, 3, 0, 3);

						int temp = i;
						rb.Click += delegate {
							//play
							if (sm.isPlaying()) sm.stop();
							sm.play(songList[temp].getUri().ToString());
							sm.playingIndex = temp;

							//save
							Settings.SetAlarmField(index, Settings.AlarmField.Song, 
							                       songList[temp].getUri().ToString());
						};

						group.AddView(rb);
						group.AddView(v);
					}

				}
			}
			return view;
		}
	}
}
