
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
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

		public override void OnResume()
		{
			base.OnResume();
			if (mPage == 1)
			{


				//Update
				//ScrollView scroll = view.FindViewById<ScrollView>(Resource.Id.song_scrollview);
				if (Activity.CheckSelfPermission(
							Android.Manifest.Permission.ReadExternalStorage)
								== Permission.Granted && ManageAlarms.songList == null)
				{
					ManageAlarms.songList = ManageAlarms.sm.getSongList();
				}

			}
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			// Use this to return your custom view for this Fragment
			// return inflater.Inflate(Resource.Layout.YourFragment, container, false);
			LinearLayout view = (LinearLayout)inflater.Inflate(Resource.Layout.fragment_page, container, false);
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
				if (Activity.CheckSelfPermission(
							Android.Manifest.Permission.ReadExternalStorage)
								== Permission.Granted && songList == null)
				{
					ManageAlarms.songList = ManageAlarms.sm.getSongList();
					songList = ManageAlarms.songList;
				}

				if (songList != null && songList.Count > 0)
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
						rb.Click += delegate
						{
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
				else
				{
					//ScrollView scroll = view.FindViewById<ScrollView>(Resource.Id.song_scrollview);
					//view.RemoveView(scroll);
					TextView tv = new TextView(Context);
					if (Activity.CheckSelfPermission(
						Android.Manifest.Permission.ReadExternalStorage)
							!= Permission.Granted)
					{
						tv.Text = "You must give Clockwise file access to play device music. Click here to go to Settings.";
						tv.Click += delegate {
							Intent intent = new Intent();
							intent.SetAction(Android.Provider.Settings.ActionApplicationDetailsSettings);
							Android.Net.Uri uri = Android.Net.Uri.FromParts("package", Context.PackageName, null);
							intent.SetData(uri);
							StartActivity(intent);	
						};
					}
					else
					{
						tv.Text = "Oops, looks like your device doesn't have music.";
					}

					view.SetGravity(GravityFlags.Center);
					tv.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent,
					                                                 ViewGroup.LayoutParams.WrapContent);
					tv.TextAlignment = TextAlignment.Center;
					tv.Gravity = (GravityFlags.CenterVertical | GravityFlags.CenterHorizontal);
					tv.SetPadding((int)(10 * metrics.Density), 0, (int)(10 * metrics.Density), 0);
					view.AddView(tv);
				}
			}
			return view;
		}
	}
}
