using Android.App;
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
using Android.Content;

namespace Clockwise.Droid
{
	[Activity(Label = "Clockwise")]
	public class MainActivity : Activity
	{
		private int repeatDaysResult = 0;
		RelativeLayout settingsContainer;
		LinearLayout snoozeRow;
		LinearLayout buttonRow;
		Typeface fontLight;
		View parent;
		bool moduleSettingOpen = false;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			fontLight = Typeface.CreateFromAsset(Resources.Assets, "HelveticaNeueLight.ttf");
			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);
			parent = FindViewById<LinearLayout>(Resource.Id.set_alarm_window);
			int alarm_number = Intent.GetIntExtra("alarm_number", 0); //CHANGE

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

			hourpicker.ValueChanged += delegate {
				int hourSet = (hourpicker.Value + ampmpicker.Value * 12);
				if (hourSet == 12 && ampmpicker.Value == 0) hourSet = 0; 
				Settings.SetAlarmField(alarm_number, Settings.AlarmField.Hour, 
				                       "" + hourSet);
			};


			minutepicker.ValueChanged += delegate
			{
				Settings.SetAlarmField(alarm_number, Settings.AlarmField.Minute, "" + minutepicker.Value);
			};

			ampmpicker.ValueChanged += delegate
			{
				Settings.SetAlarmField(alarm_number, Settings.AlarmField.Hour,
									   "" + (hourpicker.Value + ampmpicker.Value * 12));
			};



			ImageSwitcher alarm_toggle = FindViewById<ImageSwitcher>(Resource.Id.alarm_toggle);
			alarm_toggle.SetFactory(new Toggle());

			string[] alarmSettings = Settings.Alarms.Split('|')[alarm_number].Split('#');

			//Load settings
			int status = int.Parse(Settings.GetAlarmField(alarm_number, Settings.AlarmField.Status));
			if (status == (int)Settings.AlarmStatus.ALARM_ON)
			{
				alarm_toggle.SetImageResource(Resource.Drawable.on_toggle);
			}
			else {
				alarm_toggle.SetImageResource(Resource.Drawable.off_toggle);
			}

			int hour = System.Int32.Parse(alarmSettings[1]);
			int minute = System.Int32.Parse(alarmSettings[2]);
			bool am = hour < 12;

			hourpicker.Value = (am ? hour : hour - 12);
			minutepicker.Value = (minute);
			ampmpicker.Value = (am ? 0 : 1);

			alarm_toggle.Click += delegate {
				int temp = int.Parse(Settings.GetAlarmField(alarm_number, Settings.AlarmField.Status));

				if (temp == (int)Settings.AlarmStatus.ALARM_OFF) //if alarm is off
				{
					//Turn alarm on
					//int hourSet = hourpicker.Value + ampmpicker.Value * 12;
					//if (hourSet == 12 && ampmpicker.Value == 0) hourSet = 0;
					//if (hourSet == 24) hourSet = 12;
					Settings.SetAlarmField(alarm_number, Settings.AlarmField.Status, "" + (int)Settings.AlarmStatus.ALARM_ON);
					alarm_toggle.SetImageResource(Resource.Drawable.on_toggle);
					alarm_toggle.Activated = true;
					int hourSet = int.Parse(Settings.GetAlarmField(alarm_number, Settings.AlarmField.Hour));
					int minuteSet = int.Parse(Settings.GetAlarmField(alarm_number, Settings.AlarmField.Minute));
					int days = int.Parse(Settings.GetAlarmField(alarm_number, Settings.AlarmField.RepeatDays));
					AlarmUtils.SetTime(Application.Context, hourSet, minuteSet, alarm_number, days, false);
					Console.WriteLine("set time: " + hourSet + ":" + minuteSet);
				}
				else {
					//Turn alarm off
					Settings.SetAlarmField(alarm_number, Settings.AlarmField.Status, "" + (int)Settings.AlarmStatus.ALARM_OFF);
					alarm_toggle.SetImageResource(Resource.Drawable.off_toggle);
					alarm_toggle.Activated = false;
					AlarmUtils.Cancel(Application.Context, alarm_number, false);
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
					//Settings.RepeatDays = repeatDaysResult.ToString();
					Settings.SetAlarmField(alarm_number, Settings.AlarmField.RepeatDays, "" + repeatDaysResult);

					//
					//LOOK INTO
					//
					if (repeatDaysResult == 0 && alarm_toggle.Activated)
					{
						//Turn alarm off
						Settings.SetAlarmField(alarm_number, Settings.AlarmField.Status, 
						                       "" + (int)Settings.AlarmStatus.ALARM_OFF);
						alarm_toggle.SetImageResource(Resource.Drawable.off_toggle);
						alarm_toggle.Activated = false;
						AlarmUtils.Cancel(Application.Context, alarm_number, false);
					}
				};

			}

			//Load saved days
			int savedModule = int.Parse(Settings.GetAlarmField(alarm_number, Settings.AlarmField.RepeatDays));

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
			settingsContainer = FindViewById<RelativeLayout>(Resource.Id.settings_container);
			snoozeRow = FindViewById<LinearLayout>(Resource.Id.snooze_row);
			buttonRow = FindViewById<LinearLayout>(Resource.Id.settings_row);
			ImageView settingsBtn = FindViewById<ImageView>(Resource.Id.settings);
			AnimationManager settingsAnimationManager = new AnimationManager(false);
			AnimationHelper settingsHelper = new AnimationHelper(settingsContainer, settingsAnimationManager);

			settingsBtn.Click += delegate
			{
				if (!settingsAnimationManager.Animating)
				{
					if (settingsContainer.LayoutParameters.Height == 0)
					{
						//Expand
						int targetHeight = (int)(80 * settingsContainer.Context.Resources.DisplayMetrics.Density);
						settingsHelper.expand((int)(200), targetHeight);
					}
					else {
						//Collapse
						settingsHelper.collapse((int)(200), 0);

						if (buttonRow.Alpha < .5f)
						{
							//Fade in settings
							buttonRow.Enabled = true;
							buttonRow.Clickable = true;
							AnimationHelper buttonHolderFade = new AnimationHelper(buttonRow, null);

							//Fade out snooze
							snoozeRow.Enabled = false;
							snoozeRow.Clickable = false;
							AnimationHelper snoozeRowFade = new AnimationHelper(snoozeRow, null);
							buttonHolderFade.Fade(100, 1.0f);
							snoozeRowFade.Fade(100, 0f);
						}
					}
				}
			};

			ImageView addModuleBtn = FindViewById<ImageView>(Resource.Id.add_module_button);
			addModuleBtn.Click += delegate {
				Intent creatModule = new Intent(Application.Context, typeof(CreateModule));
				creatModule.PutExtra("alarm_index", alarm_number);
				StartActivity(creatModule);
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
			pulldownParams.AddRule(LayoutRules.Below, Resource.Id.settings_container);

			//Snooze
			FindViewById<ImageView>(Resource.Id.snooze).Click += delegate {
				Console.WriteLine("clicked snooze");
				if (buttonRow.Alpha > .5f)
				{
					Console.WriteLine("fading in");
					//Fade in snooze
					buttonRow.Enabled = false;
					buttonRow.Clickable = false;

					AnimationHelper buttonHolderFade = new AnimationHelper(buttonRow, null);
					snoozeRow.Enabled = true;
					snoozeRow.Clickable = true;

					AnimationHelper snoozeRowFade = new AnimationHelper(snoozeRow, null);
					buttonHolderFade.Fade(100, 0f);
					snoozeRowFade.Fade(100, 1.0f);
				}
			};

			SeekBar snoozeBar = FindViewById<SeekBar>(Resource.Id.snoozeBar);
			TextView snoozeOutput = FindViewById<TextView>(Resource.Id.snoozeOutput);
			snoozeOutput.Typeface = fontLight;
			string[] snoozeValues = Resources.GetStringArray(Resource.Array.snooze_values);
			snoozeBar.Max = snoozeValues.Length-1;
			snoozeBar.ProgressChanged += delegate
			{
				Console.WriteLine("seek: " + snoozeBar.Progress);
				snoozeOutput.Text = snoozeValues[snoozeBar.Progress];
				Settings.SetAlarmField(alarm_number, Settings.AlarmField.Snooze, snoozeValues[snoozeBar.Progress]);
			};

			int snoozeVal = int.Parse(Settings.GetAlarmField(alarm_number, Settings.AlarmField.Snooze));
			for (int i = 0; i < snoozeValues.Length; i++)
			{
				if (int.Parse(snoozeValues[i]) == snoozeVal)
				{
					snoozeBar.Progress = i;
				}
			}

			//Song select
			FindViewById<ImageView>(Resource.Id.tone).Click += delegate {
				Intent i = new Intent(ApplicationContext, typeof(SongSelect));
				i.PutExtra("alarm_index", alarm_number);
				StartActivity(i);
				//Close settings
				//settingsBtn.PerformClick();
			};

			//Scrollview
			HorizontalScrollView scrollView = FindViewById<HorizontalScrollView>(Resource.Id.module_scroller);
			LinearLayout scroll_layout = FindViewById<LinearLayout>(Resource.Id.module_layout);

			scrollView.ScrollChange += delegate {
				Console.WriteLine("scrollview x: " + scrollView.ScrollX);
				//Console.WriteLine("scrollview width: " + scrollView.Width);

				Console.WriteLine("percent scrolled: " + (double)scrollView.ScrollX/(double)scroll_layout.Width);

			};
		}

		private void RefreshModules(int index)
		{
			string[] modules = Settings.GetActiveModules(index);
			LinearLayout moduleLayout = FindViewById<LinearLayout>(Resource.Id.module_layout);
			var metrics = Resources.DisplayMetrics;

			foreach(string m in modules)
			{
				if (m != string.Empty)
				{
					string type = m.Substring(0, m.IndexOf(':'));
					//RelativeLayout rl = null;
					switch (type)
					{
						case "fact":
							moduleLayout.AddView(CreateModuleDisplay(type, "Fun Fact", index, -1));
							break;
						case "quote":
							moduleLayout.AddView(CreateModuleDisplay(type, "Quote of the Day", index, -1));
							break;
						case "tdih":
							moduleLayout.AddView(CreateModuleDisplay(type, "This Day in History", index, -1));
							break;
						case "weather":
							moduleLayout.AddView(CreateModuleDisplay(type, "Weather", index, -1));
							break;
						case "news":
						case "reddit":
						case "twitter":
						case "countdown":
						case "reminders":
						{ //news:cat:count,cat:count, ...
							string[] moduleList = m.Substring(m.IndexOf(':') + 1).Split(',');
								for (int i = 0; i < moduleList.Length; i++)
							{
									string[] settings = moduleList[i].Split(':');
									moduleLayout.AddView(CreateModuleDisplay(type, settings[0], index, i));
							}
							break;
						}
					}
				}
			}
		}

		private RelativeLayout CreateModuleDisplay(string type, string title, int index, int subindex)
		{
			var metrics = Resources.DisplayMetrics;
			RelativeLayout rl = (RelativeLayout)LayoutInflater.Inflate(Resource.Layout.module_holder, null);
			rl.LayoutParameters = new LinearLayout.LayoutParams((int)(metrics.WidthPixels), LinearLayout.LayoutParams.MatchParent);

			RelativeLayout module = rl.FindViewById<RelativeLayout>(Resource.Id.module);
			RelativeLayout.LayoutParams moduleParams = new RelativeLayout.LayoutParams((int)(metrics.WidthPixels * .85), RelativeLayout.LayoutParams.MatchParent);
			moduleParams.AddRule(LayoutRules.CenterHorizontal);
			module.LayoutParameters = moduleParams;
			module.Measure(RelativeLayout.LayoutParams.MatchParent, RelativeLayout.LayoutParams.MatchParent);

			TextView settingTitle = rl.FindViewById<TextView>(Resource.Id.setting_title);
			ImageView settingImage = rl.FindViewById<ImageView>(Resource.Id.moduleImage);
			ImageView navButton = rl.FindViewById<ImageView>(Resource.Id.navButton);

			settingTitle.Typeface = Typeface.CreateFromAsset(Resources.Assets, "HelveticaNeueLight.ttf");
			settingTitle.TextSize = (int)((metrics.HeightPixels / metrics.Density) * .06);
			//RelativeLayout.LayoutParams titleParams = (RelativeLayout.LayoutParams)settingTitle.LayoutParameters;
			//titleParams.Width = 
			settingTitle.LayoutParameters.Width = RelativeLayout.LayoutParams.MatchParent;

			parent.Measure(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent);

			settingTitle.Text = title;

			//Add Editing
			RelativeLayout displayRow = module.FindViewById<RelativeLayout>(Resource.Id.display_row);
			LinearLayout editor = null;

			switch (type)
			{
				case "fact":
					break;
				case "quote":
					break;
				case "tdih":
					break;
				case "weather":
					settingImage.SetImageResource(Resource.Drawable.weather_icon);
					editor = (LinearLayout)LayoutInflater.Inflate(Resource.Layout.weather, null);
					Weather.Setup(index, editor, null, ApplicationContext);
					break;
				case "news":
					settingImage.SetImageResource(Resource.Drawable.news_icon);
					editor = (LinearLayout)LayoutInflater.Inflate(Resource.Layout.news, null);
					News.Setup(index, subindex, editor, null, this);

					break;
				case "reddit":
					settingImage.SetImageResource(Resource.Drawable.reddit_icon);
					editor = (LinearLayout)LayoutInflater.Inflate(Resource.Layout.reddit, null);
					Reddit.Setup(index, subindex, editor, null, this);

					break;
				case "twitter":
					settingImage.SetImageResource(Resource.Drawable.twitter_icon);
					editor = (LinearLayout)LayoutInflater.Inflate(Resource.Layout.twitter, null);
					Twitter.Setup(index, subindex, editor, null, this);

					break;
				case "reminders":
					settingImage.SetImageResource(Resource.Drawable.todo_icon);
					break;
			}

			settingImage.LayoutParameters.Width = (int)((double)parent.MeasuredHeight * .4);
			settingImage.LayoutParameters.Height = (int)((double)parent.MeasuredHeight * .4);

			if (editor != null)
			{
				FrameLayout.LayoutParams editorParams = new FrameLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent,
				                                                                     ViewGroup.LayoutParams.WrapContent);
				editor.LayoutParameters = editorParams;
				editor.Alpha = 0;
				editor.Visibility = ViewStates.Invisible;
				displayRow.AddView(editor);
			}

			navButton.Click += delegate {
				//if(
				AnimationHelper editorFade = new AnimationHelper(editor, null);
				AnimationHelper imageFade = new AnimationHelper(settingImage, null);
				if (editor.Alpha < .5f)
				{
					editorFade.Fade(200, 1f);
					imageFade.Fade(200, 0f);
					navButton.SetImageResource(Resource.Drawable.back_button);
					moduleSettingOpen = true;
				}
				else {
					editorFade.Fade(200, 0f);
					imageFade.Fade(200, 1f);
					navButton.SetImageResource(Resource.Drawable.edit_button);
					moduleSettingOpen = false;

				}
			};

			return rl;
		}

		public override void OnBackPressed()
		{
			if (buttonRow.Alpha < .5f)
			{
				Console.WriteLine("fading out");
				//Fade in settings
				buttonRow.Enabled = true;
				buttonRow.Clickable = true;
				AnimationHelper buttonHolderFade = new AnimationHelper(buttonRow, null);

				//Fade out snooze
				snoozeRow.Enabled = false;
				snoozeRow.Clickable = false;
				AnimationHelper snoozeRowFade = new AnimationHelper(snoozeRow, null);
				buttonHolderFade.Fade(100, 1.0f);
				snoozeRowFade.Fade(100, 0f);
			}
			else {
				base.OnBackPressed();
			}
		}

		protected override void OnResume()
		{
			base.OnResume();
			LinearLayout moduleLayout = FindViewById<LinearLayout>(Resource.Id.module_layout);
			while(moduleLayout.ChildCount > 0) moduleLayout.RemoveViewAt(0);
			RefreshModules(Intent.GetIntExtra("alarm_number", 0));

			if (CheckSelfPermission(Android.Manifest.Permission.ReadExternalStorage)
			   == Permission.Granted && ManageAlarms.songsRadioGroup == null){
				new ManageAlarms.GetSongs().Execute();
			}
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

		public class Hou : Java.Lang.Object, View.IOnDragListener
		{
			public bool OnDrag(View v, DragEvent e)
			{
				return true;
			}
		}

		//public class Hou : Java.Lang.Object, View.IOnDragListener
		//{
		//	public bool OnDrag(View v, DragEvent e)
		//	{
		//		return true;
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

