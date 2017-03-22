
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
using Clockwise.Helpers;
using Android.Support.V7.App;
using Android.Support.V4.View;
using Android.Support.Design.Widget;

namespace Clockwise.Droid
{
	[Activity(Label = "SongSelect", Theme = "@style/Theme.AppCompat.Light.NoActionBar")]
	public class SongSelect : AppCompatActivity
	{
		List<Song> songList;
		SongManager sm;
		RadioGroup radioList;
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.SongSelect);
			int alarm_index = Intent.GetIntExtra("alarm_index", -1);

			// Get the ViewPager and set it's PagerAdapter so that it can display items
			ViewPager viewPager = FindViewById<ViewPager>(Resource.Id.viewpager);
			viewPager.Adapter = new SongPageAdapter(SupportFragmentManager,ApplicationContext, alarm_index);
			

			// Give the TabLayout the ViewPager
			TabLayout tabLayout = (TabLayout)FindViewById<TabLayout>(Resource.Id.sliding_tabs);
			tabLayout.SetupWithViewPager(viewPager);
			sm = SongManager.getInstance(this);

			SeekBar volume = FindViewById<SeekBar>(Resource.Id.volume_seek_bar);
			volume.Progress = int.Parse(Settings.GetAlarmField(alarm_index, Settings.AlarmField.Volume)) - 1;
			volume.ProgressChanged += delegate {
				Settings.SetAlarmField(alarm_index, Settings.AlarmField.Volume, "" + (volume.Progress + 1));
				sm.setVolume((float)volume.Progress / 14f);
			};

		}

		protected override void OnStop()
		{
			if (sm != null) { sm.stop(); }
			base.OnStop();
		}
	}
}
