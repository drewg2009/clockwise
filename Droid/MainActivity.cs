﻿using Android.App;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using System.Linq;
using Android.Content.PM;
using Android.Graphics;
using Android.Views;
using Android.Views.Animations;
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
						Console.WriteLine("passing time: " + hourSet + ":" + minutepicker.Value);
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
			LinearLayout settings_row = FindViewById<LinearLayout>(Resource.Id.settings_row);
			ImageView settingsBtn = FindViewById<ImageView>(Resource.Id.settings);
			AnimationManager settingsAnimationManager = new AnimationManager(false);
			AnimationHelper settingsHelper = new AnimationHelper(settings_row, settingsAnimationManager);
			settingsBtn.Click += delegate
			{
				int initialHeight = clock_settings_layout.Height;
				System.Console.WriteLine("clock height: " + initialHeight);

				if (!settingsAnimationManager.Animating)
				{
					if (settings_row.LayoutParameters.Height == 0)
					{
						//Expand
						int targetHeight = (int)(80 * settings_row.Context.Resources.DisplayMetrics.Density);
						int duration = (int)(200);
						settingsHelper.expand(duration, targetHeight);
					}
					else {
						//Collapse
						int targetHeight = 0;
						int duration = (int)(200);
						settingsHelper.collapse(duration, targetHeight);
					}
				}
			};

			ImageView addModuleBtn = FindViewById<ImageView>(Resource.Id.add_module_button);
			addModuleBtn.Click += delegate {
				StartActivity(typeof(CreateModule));
			};

			var metrics = Resources.DisplayMetrics;

			//Set clocksettings height
			RelativeLayout.LayoutParams clock_settings_layout_params = (RelativeLayout.LayoutParams)clock_settings_layout.LayoutParameters;
			clock_settings_layout_params.Height = (int)(metrics.HeightPixels * .4);
			clock_settings_layout.LayoutParameters = clock_settings_layout_params;
			//clock_settings_layout.LayoutParameters

			//Align test module
			FrameLayout testLayout = FindViewById<FrameLayout>(Resource.Id.moduleSection);
			RelativeLayout.LayoutParams testParams = (RelativeLayout.LayoutParams)testLayout.LayoutParameters;
			testParams.AddRule(LayoutRules.Below, Resource.Id.pulldown);
			testParams.Width = (int)metrics.WidthPixels;

			//Pulldown
			ImageView pulldown = FindViewById<ImageView>(Resource.Id.pulldown);
			RelativeLayout.LayoutParams pulldownParams = (RelativeLayout.LayoutParams)pulldown.LayoutParameters;
			pulldownParams.AddRule(LayoutRules.Below, Resource.Id.settings_row);

			//Scrollview
			HorizontalScrollView scrollView = FindViewById<HorizontalScrollView>(Resource.Id.module_scroller);
			LinearLayout scroll_layout = FindViewById<LinearLayout>(Resource.Id.module_layout);

			scrollView.ScrollChange += delegate {
				Console.WriteLine("scrollview x: " + scrollView.ScrollX);
				//Console.WriteLine("scrollview width: " + scrollView.Width);

				Console.WriteLine("percent scrolled: " + (double)scrollView.ScrollX/(double)scroll_layout.Width);

			};
		}

		private void addModules()
		{
			string[] modules = ModuleHelper.GetActiveModules();
			LinearLayout moduleLayout = FindViewById<LinearLayout>(Resource.Id.module_layout);
			var metrics = Resources.DisplayMetrics;

			foreach(string m in modules)
			{
				if (m != string.Empty)
				{
					RelativeLayout rl = (RelativeLayout)LayoutInflater.Inflate(Resource.Layout.module_holder, null);
					rl.LayoutParameters = new LinearLayout.LayoutParams((int)(metrics.WidthPixels), LinearLayout.LayoutParams.MatchParent);

					RelativeLayout module = rl.FindViewById<RelativeLayout>(Resource.Id.module);
					RelativeLayout.LayoutParams moduleParams = new RelativeLayout.LayoutParams((int)(metrics.WidthPixels * .85), RelativeLayout.LayoutParams.MatchParent);
					moduleParams.AddRule(LayoutRules.CenterHorizontal);
					module.LayoutParameters = moduleParams;
					module.Measure(RelativeLayout.LayoutParams.MatchParent, RelativeLayout.LayoutParams.MatchParent); 

					TextView settingTitle = rl.FindViewById<TextView>(Resource.Id.setting_title);
					ImageView settingImage = rl.FindViewById<ImageView>(Resource.Id.moduleImage);
					settingTitle.Typeface = Typeface.CreateFromAsset(Resources.Assets, "HelveticaNeueLight.ttf");
					settingTitle.TextSize = (int)((metrics.HeightPixels/metrics.Density) * .06);
					RelativeLayout.LayoutParams titleParams = (RelativeLayout.LayoutParams)settingTitle.LayoutParameters;
					titleParams.Width = (int)((metrics.WidthPixels * .85) / 2);
					settingTitle.LayoutParameters = titleParams;

					Console.WriteLine(module.MeasuredWidth + ", " + module.MeasuredHeight);
					settingImage.LayoutParameters.Width = (int)(module.MeasuredHeight * .6);
					settingImage.LayoutParameters.Height = (int)(module.MeasuredHeight *.6);


					string type = m.Substring(0, m.IndexOf(':'));
					switch (type)
					{
						case "fact":
							settingTitle.Text = "Fun Fact";
							moduleLayout.AddView(rl);
							break;
						case "quote":
							settingTitle.Text = "Quote of the Day";

							moduleLayout.AddView(rl);
							break;
						case "tdih":
							settingTitle.Text = "This Day in History";
							moduleLayout.AddView(rl);
							break;
						case "weather":
							settingTitle.Text = "Weather";
							settingImage.SetImageResource(Resource.Drawable.weather_icon);
							moduleLayout.AddView(rl);

							break;
					}
				}
			}
		}

		protected override void OnResume()
		{
			base.OnResume();
			LinearLayout moduleLayout = FindViewById<LinearLayout>(Resource.Id.module_layout);
			while(moduleLayout.ChildCount > 0) moduleLayout.RemoveViewAt(0);
			addModules();
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

		public class ScrollListener : Java.Lang.Object, View.IOnDragListener
		{
			public bool OnDrag(View v, DragEvent e)
			{
				return true;
			}
		}
				
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

