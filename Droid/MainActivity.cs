﻿using Android.App;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using System.Linq;
using Android.Content.PM;
using Android.Graphics;
using Android.Views;
using Android.Animation;
using System;
using Clockwise.Helpers;

namespace Clockwise.Droid
{
	[Activity(Label = "Clockwise", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : Activity
	{
		bool alarmOn = true;
		private int repeatDaysResult = 0;
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);

			// Setup pickers
			NumberPicker hourpicker = FindViewById<NumberPicker>(Resource.Id.hour);
			NumberPicker minutepicker = FindViewById<NumberPicker>(Resource.Id.minute);
			NumberPicker ampmpicker = FindViewById<NumberPicker>(Resource.Id.ampm);

			//string[] hours = 
			//hourpicker.SetDisplayedValues(Enumerable.Range(1, 12).Select(n => n.ToString()).ToArray());
			hourpicker.MinValue = 1;
			hourpicker.MaxValue = 12;

			minutepicker.MinValue = 0;
			minutepicker.MaxValue = 59;
			minutepicker.SetFormatter(new TwoDigitFormatter());

			ampmpicker.MaxValue = 1;
			ampmpicker.MinValue = 0;
			ampmpicker.SetDisplayedValues(new string[] { "am", "pm" });

			ImageSwitcher alarm_toggle = FindViewById<ImageSwitcher>(Resource.Id.alarm_toggle);
			alarm_toggle.SetFactory(new Toggle());

			if (Settings.AlarmTime == string.Empty)
			{
				alarm_toggle.SetImageResource(Resource.Drawable.off_toggle);

				//SET CLOCK TO CURRENT TIME
				System.DateTime currentTime = System.DateTime.Now;
				int hour = currentTime.Hour;
				int minute = currentTime.Minute;
				bool am = hour < 12;

				hourpicker.Value = (am ? hour : hour - 12);
				minutepicker.Value = (minute);
				ampmpicker.Value = (am ? 0 : 1);
			}
			else {
				alarm_toggle.SetImageResource(Resource.Drawable.on_toggle);
				string alarmTime = Settings.AlarmTime;
				int hour = System.Int32.Parse(alarmTime.Split(':')[0]);
				int minute = System.Int32.Parse(alarmTime.Split(':')[1]);
				bool am = hour < 12;

				hourpicker.Value = (am ? hour : hour - 12);
				minutepicker.Value = (minute);
				ampmpicker.Value = (am ? 0 : 1);
			}
			

			//Animation animation = null;
			//alarm_toggle.SetInAnimation(Android.App.Application.Context, Resource.Drawable.off_toggle);
			//alarm_toggle.SetOutAnimation(Android.App.Application.Context, Resource.Drawable.on_toggle);

			alarm_toggle.Click += delegate {
				if (Settings.AlarmTime == string.Empty)
				{
					if (repeatDaysResult != 0)
					{
						//Turn alarm on
						int hourSet = hourpicker.Value + ampmpicker.Value * 12;
						if (hourSet == 12 && ampmpicker.Value == 0) hourSet = 0;
						if (hourSet == 24) hourSet = 12;
						alarm_toggle.SetImageResource(Resource.Drawable.on_toggle);
						alarm_toggle.Activated = true;
						AlarmUtils.SetTime(Android.App.Application.Context, hourSet, minutepicker.Value);
					}
					else {
						Toast.MakeText(Application.Context, "Select a repeat day first", ToastLength.Long).Show();
					}
				}
				else {
					//Turn alarm off
					Settings.AlarmTime = string.Empty;
					alarm_toggle.SetImageResource(Resource.Drawable.off_toggle);
					alarm_toggle.Activated = false;
					AlarmUtils.Cancel(Application.Context);
				}
			};

			TextView[] repeatDays = new TextView[7];
			repeatDays[0] = FindViewById<TextView>(Resource.Id.sundayInput);
			repeatDays[1] = FindViewById<TextView>(Resource.Id.mondayInput);
			repeatDays[2] = FindViewById<TextView>(Resource.Id.tuesdayInput);
			repeatDays[3] = FindViewById<TextView>(Resource.Id.wednesdayInput);
			repeatDays[4] = FindViewById<TextView>(Resource.Id.thursdayInput);
			repeatDays[5] = FindViewById<TextView>(Resource.Id.fridayInput);
			repeatDays[6] = FindViewById<TextView>(Resource.Id.saturdayInput);

			for (int i = 0; i < 7; i++)
			{
				TextView tv = repeatDays[i];
				tv.SetTextColor(Color.ParseColor("#CCCCCC"));
				int temp = 1 << i;
				tv.Click += delegate {
					//Turn on
					if (tv.CurrentTextColor == Color.ParseColor("#CCCCCC"))
					{
						//tv.setTextColor(Color.parseColor("#7BD678"));
						ObjectAnimator colorAnim = ObjectAnimator.OfInt(tv, "textColor",
								Color.ParseColor("#CCCCCC"), Color.ParseColor("#7BD678"));
						colorAnim.SetEvaluator(new ArgbEvaluator());
						colorAnim.Start();
						tv.SetTypeface(null, Android.Graphics.TypefaceStyle.Bold);
					}
					//Turn off
					else {
						ObjectAnimator colorAnim = ObjectAnimator.OfInt(tv, "textColor",
									Color.ParseColor("#7BD678"), Color.ParseColor("#CCCCCC"));
						colorAnim.SetEvaluator(new ArgbEvaluator());
						colorAnim.Start();
						tv.SetTypeface(null, TypefaceStyle.Normal);
					}

					repeatDaysResult = repeatDaysResult ^ temp;
					Settings.RepeatDays = repeatDaysResult.ToString();
					if (repeatDaysResult == 0 && alarm_toggle.Activated)
					{
						//Turn alarm off
						alarm_toggle.SetImageResource(Resource.Drawable.off_toggle);
						alarm_toggle.Activated = false;
						AlarmUtils.Cancel(Application.Context);
					}
				};

			}

			//Load saved days
			int savedModule = Int32.Parse(Settings.RepeatDays);
			if (savedModule != 0)
			{
				repeatDaysResult = savedModule;
				for (int i = 0; i < repeatDays.Length; i++)
				{
					if ((repeatDaysResult & (1 << i)) == (1 << i))
					{
						repeatDays[i].SetTextColor(Color.ParseColor("#7BD678"));
						repeatDays[i].SetTypeface(null, TypefaceStyle.Bold);
					}
				}
			}

			//Settings expansion
			RelativeLayout clock_settings_layout = FindViewById<RelativeLayout>(Resource.Id.clock_settings);
			ImageView settingsBtn = FindViewById<ImageView>(Resource.Id.settings);
			settingsBtn.Click += delegate
			{

				//clock_settings_layout.Animate().SetInterpolator(
				//clock_settings_layout.Measure(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent)
				int initialHeight = clock_settings_layout.Height;
				System.Console.WriteLine("clock height: " + initialHeight);
			};

			//Align test module
			FrameLayout testLayout = FindViewById<FrameLayout>(Resource.Id.moduleTest);
			RelativeLayout.LayoutParams testParams = (RelativeLayout.LayoutParams)testLayout.LayoutParameters;
			testParams.AddRule(LayoutRules.Below, Resource.Id.pulldown);

			var metrics = Resources.DisplayMetrics;
			var widthInDp = (int)((metrics.WidthPixels) / Resources.DisplayMetrics.Density);

			FrameLayout module1Holder = FindViewById<FrameLayout>(Resource.Id.module1Holder);
			module1Holder.LayoutParameters = new LinearLayout.LayoutParams((int)(metrics.WidthPixels * .9), LinearLayout.LayoutParams.MatchParent);

			FrameLayout module2Holder = FindViewById<FrameLayout>(Resource.Id.module2Holder);
			module2Holder.LayoutParameters = new LinearLayout.LayoutParams((int)(metrics.WidthPixels * .9), LinearLayout.LayoutParams.MatchParent);

			//holderParams.Width = (int)(widthInDp * .75f);


		}


		public class TwoDigitFormatter : Java.Lang.Object, NumberPicker.IFormatter
		{
			public string Format(int value)
			{
				return string.Format("{0:D2}", value);
			}
		}

		public class Toggle : Java.Lang.Object, ViewSwitcher.IViewFactory
		{
			public View MakeView()
			{
				ImageView myView = new ImageView(Android.App.Application.Context);
				return myView;
			}
		}

		//public class SettingsAnimation : Java.Lang.Object, Android.Animation.ITimeInterpolator

		//{
		//	public View MakeView()
		//	{
		//		ImageView myView = new ImageView(Android.App.Application.Context);
		//		return myView;
		//	}
		//}
		public override async void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
		{
			switch (requestCode)
			{
				case 1:
					{

						// If request is cancelled, the result arrays are empty.
						if (grantResults.Count() > 0
						    && grantResults[0] == Permission.Granted)
						{

							// permission was granted, yay! Do the
							// contacts-related task you need to do.
							Settings.AndroidFileAccess = "true";
							Toast.MakeText(Android.App.Application.Context, "Permission granted to access song files", ToastLength.Short).Show();
						}
						else {

							// permission denied, boo! Disable the
							// functionality that depends on this permission.
							Toast.MakeText(Android.App.Application.Context, "Permission denied to access song files", ToastLength.Short).Show();

						}
						break;
					}
			}
		}
			                        
	}
}

