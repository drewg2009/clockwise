
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
		static RadioButton defaultRadioButton = null;
		static RadioButton songRadioButton = null;
		private LinearLayout groupHolder;
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
				if ((int)Build.VERSION.SdkInt >= 23)
				{
					if (Activity.CheckSelfPermission(
							Android.Manifest.Permission.ReadExternalStorage)
								== Permission.Granted && ManageAlarms.songList == null)
					{
						ManageAlarms.songList = ManageAlarms.sm.getSongList();
					}
				}
				else if (ManageAlarms.songList == null)
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
			groupHolder = view.FindViewById<LinearLayout>(Resource.Id.scrollList);
			//RadioGroup group = view.FindViewById<RadioGroup>(Resource.Id.tone_radio_group);
			Typeface font = Typeface.CreateFromAsset(Context.Resources.Assets, "HelveticaNeueLight.ttf");
			List<Song> songList = ManageAlarms.songList;
			List<Song> defaultList = ManageAlarms.defaultList;

			SongManager sm = ManageAlarms.sm;

			var metrics = Resources.DisplayMetrics;
			while (groupHolder.ChildCount != 0)
				groupHolder.RemoveViewAt(0);
			
			if (mPage == 0)
			{
				if (ManageAlarms.defaultsRadioGroup.Parent != null)
				{
					LinearLayout defaultsParent = (LinearLayout)ManageAlarms.defaultsRadioGroup.Parent;
					defaultsParent.RemoveView(ManageAlarms.defaultsRadioGroup);
				}

				groupHolder.AddView(ManageAlarms.defaultsRadioGroup);
				RadioGroup group = (RadioGroup)groupHolder.GetChildAt(0);
				for (int i = 0; i < group.ChildCount; i+=2)
				{
					Console.WriteLine("casting: " + i/2);
					RadioButton rb = (RadioButton)group.GetChildAt(i);
					int temp = i/2;

					if (defaultList[temp].getUri().ToString()
						== Helpers.Settings.GetAlarmField(index, Helpers.Settings.AlarmField.Song))
					{
						rb.Checked = true;
					}

					rb.Click += delegate
					{
						//save
						Helpers.Settings.SetAlarmField(index, Helpers.Settings.AlarmField.Song,
													   defaultList[temp].getUri().ToString());
						if (songRadioButton != null)
						{
							songRadioButton.Checked = false;
							songRadioButton = null;
						}
						defaultRadioButton = rb;
					};
				}
			}
			else
			{
				if (ManageAlarms.songsRadioGroup.Parent != null)
				{
					LinearLayout songsParent = (LinearLayout)ManageAlarms.songsRadioGroup.Parent;
					songsParent.RemoveView(ManageAlarms.songsRadioGroup);
				}
				groupHolder.AddView(ManageAlarms.songsRadioGroup);
				RadioGroup group = (RadioGroup)groupHolder.GetChildAt(0);

				if (group.ChildCount > 0)
				{
					for (int i = 0; i < group.ChildCount; i += 2)
					{
						RadioButton rb = (RadioButton)group.GetChildAt(i);
						int temp = i / 2;

						if (songList[temp].getUri().ToString()
							== Helpers.Settings.GetAlarmField(index, Helpers.Settings.AlarmField.Song))
						{
							rb.Checked = true;
						}

						rb.Click += delegate
						{
							//save
							Helpers.Settings.SetAlarmField(index, Helpers.Settings.AlarmField.Song,
															   songList[temp].getUri().ToString());
							if (defaultRadioButton != null)
							{
								defaultRadioButton.Checked = false;
								defaultRadioButton = null;
							}
							songRadioButton = rb;
						};
					}
				}
				else
				{
					TextView tv = new TextView(Context);
					if (Activity.CheckSelfPermission(
						Android.Manifest.Permission.ReadExternalStorage)
							!= Permission.Granted)
					{
						tv.Text = "You must give Clockwise file access to play device music. Click here to go to Settings.";
						tv.Click += delegate {
							Activity.Finish();
							if (MainActivity.instance != null)
							{
								MainActivity.instance.Finish();
							}
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
