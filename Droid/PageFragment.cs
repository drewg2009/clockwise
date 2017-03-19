
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Provider;
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
			Typeface font = Typeface.CreateFromAsset(Context.Resources.Assets, "HelveticaNeueLight.ttf");
			List<Song> songList = ManageAlarms.songList;
			List<Song> defaultList = ManageAlarms.defaultList;

			SongManager sm = ManageAlarms.sm;

			var metrics = Resources.DisplayMetrics;

			if (mPage == 0)
			{
				for (int i = 0; i < defaultList.Count; i++)
				{
					RadioButton rb = new RadioButton(Context);
					rb.Text = defaultList[i].Title;
					rb.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
					rb.SetTextColor(Color.Gray);
					rb.Typeface = font;
					rb.TextSize = 20;
					rb.SetPadding(0, (int)(4 * metrics.Density), 0, (int)(4 * metrics.Density));
					rb.LetterSpacing = .1f;

					int temp = i;
					rb.Click += delegate
					{
						//play
						if (sm.isPlaying()) sm.stop();
						sm.play(defaultList[temp].getUri().ToString());
						sm.playingIndex = temp;

						//save
						Helpers.Settings.SetAlarmField(index, Helpers.Settings.AlarmField.Song,
						                               defaultList[temp].getUri().ToString());
					};

					//Space
					View space = new View(Context);
					space.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, 1);
					space.SetBackgroundColor(Color.Gray);
					space.SetPadding((int)(10 * metrics.Density), 0, 0, 0);

					group.AddView(rb);
					group.AddView(space);
				}
			}
			else
			{
				if (songList != null)
				{

					for (int i = 0; i < songList.Count; i++)
					{
						RadioButton rb = new RadioButton(Context);
						rb.Text = songList[i].Title;
						rb.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
						rb.SetTextColor(Color.Gray);


						rb.Typeface = font;
						rb.TextSize = 20;
						rb.SetPadding(0, (int)(12 * metrics.Density), 0, (int)(12 * metrics.Density));
						rb.LetterSpacing = .1f;

						int temp = i;
						rb.Click += delegate {
							//play
							if (sm.isPlaying()) sm.stop();
							sm.play(songList[temp].getUri().ToString());
							sm.playingIndex = temp;

							//save
							Helpers.Settings.SetAlarmField(index, Helpers.Settings.AlarmField.Song, 
							                       songList[temp].getUri().ToString());
						};

						//Space
						View space = new View(Context);
						space.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, 1);
						space.SetBackgroundColor(Color.Gray);
						space.SetPadding((int)(10 * metrics.Density), 0, 0, 0);

						group.AddView(rb);
						group.AddView(space);
					}

				}
			}
			return view;
		}
	}
}
