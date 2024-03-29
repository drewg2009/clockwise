﻿
using System;
using Android.Content;
using Android.Support.V4.App;

namespace Clockwise.Droid
{
	public class SongPageAdapter : FragmentPagerAdapter
	{
		public override int Count
		{
			get
			{
				return PAGE_COUNT;
			}
		}

		int alarmIndex;

		public override Android.Support.V4.App.Fragment GetItem(int position)
		{
			return AlarmTonesPageFragment.newInstance(position, alarmIndex);
		}

		int PAGE_COUNT = 2;
		public String[] tabTitles = new String[] { "Default Tones", "Device Music"};
		private Context context;

		public SongPageAdapter(Android.Support.V4.App.FragmentManager fm, Context context, int alarmIndex) : base(fm)
		{
			this.alarmIndex = alarmIndex;
			this.context = context;
		}

		public override Java.Lang.ICharSequence GetPageTitleFormatted(int position)
		{
			
			return new Java.Lang.String(tabTitles[position]);
		}
	}
}
