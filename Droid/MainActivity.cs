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
using Android.Util;
using Android.Views.InputMethods;

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
		ImageView settingsBtn;
		//bool moduleSettingOpen = false;
		public static RelativeLayout clock_settings_layout;
		ImageView addModuleBtn;
		ImageSwitcher alarm_toggle;
		public static CustomHorizontalScrollView scrollView;
		LinearLayout scroll_layout;
		public static DisplayMetrics DisplayMetrics;
		static int currentModule = -1;
		private FrameLayout moduleFrame;
		ImageView currentModuleNavButton = null;
		public static MainActivity instance;
		static private List<View> moduleTabs = new List<View>();
		static private int selectedTab = -1;
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

			alarm_toggle = FindViewById<ImageSwitcher>(Resource.Id.alarm_toggle);
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

			//Align module frame
			DisplayMetrics = Resources.DisplayMetrics;
			moduleFrame = FindViewById<FrameLayout>(Resource.Id.moduleSection);
			RelativeLayout.LayoutParams testParams = (RelativeLayout.LayoutParams)moduleFrame.LayoutParameters;
			testParams.AddRule(LayoutRules.Below, Resource.Id.pulldown);
			testParams.Width = (int)DisplayMetrics.WidthPixels;
			testParams.Height = (int)(DisplayMetrics.HeightPixels * .6);

			//Settings expansion
			clock_settings_layout = FindViewById<RelativeLayout>(Resource.Id.clock_settings);
			settingsContainer = FindViewById<RelativeLayout>(Resource.Id.settings_container);
			snoozeRow = FindViewById<LinearLayout>(Resource.Id.snooze_row);
			buttonRow = FindViewById<LinearLayout>(Resource.Id.settings_row);
			settingsBtn = FindViewById<ImageView>(Resource.Id.settings);
			AnimationManager settingsAnimationManager = new AnimationManager(false);
			AnimationHelper settingsHelper = new AnimationHelper(settingsContainer, settingsAnimationManager);

			settingsBtn.Click += delegate
			{
				if (!settingsAnimationManager.Animating)
				{
					RelativeLayout currentModuleLayout = null;
					if (currentModule != -1)
					{
						currentModuleLayout = (RelativeLayout)scroll_layout.GetChildAt(currentModule);
					}
					if (settingsContainer.LayoutParameters.Height == 0)
					{
						//Expand
						int targetHeight = (int)(80 * settingsContainer.Context.Resources.DisplayMetrics.Density);
						settingsHelper.expand((int)(200), targetHeight);
						settingsBtn.SetImageResource(Resource.Drawable.settings_cancel);
						scrollView.SetOnTouchListener(new ScrollViewOffListener());

						//Fade nav button for current module
						if (currentModuleLayout != null)
						{
							AnimationHelper navButtonFader = new AnimationHelper(
								currentModuleLayout.FindViewById(Resource.Id.navButton), null);
							navButtonFader.Fade(100, 0);
						}


					}
					else {
						//Collapse
						settingsHelper.collapse((int)(200), 0);
						settingsBtn.SetImageResource(Resource.Drawable.settings_button);
						scrollView.SetOnTouchListener(new ScrollViewOnListener());

						//Fade nav button for current module
						if (currentModuleLayout != null)
						{
							AnimationHelper navButtonFader = new AnimationHelper(
								currentModuleLayout.FindViewById(Resource.Id.navButton), null);
							navButtonFader.Fade(100, 1f);
						}
						

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

			addModuleBtn = FindViewById<ImageView>(Resource.Id.add_module_button);
			addModuleBtn.Click += delegate {
				Intent creatModule = new Intent(Application.Context, typeof(CreateModule));
				creatModule.PutExtra("alarm_index", alarm_number);
				StartActivity(creatModule);
			};

			//Set clocksettings height
			RelativeLayout.LayoutParams clock_settings_layout_params = (RelativeLayout.LayoutParams)clock_settings_layout.LayoutParameters;
			clock_settings_layout_params.Height = (int)(DisplayMetrics.HeightPixels * .4);
			clock_settings_layout.LayoutParameters = clock_settings_layout_params;

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
				snoozeOutput.Text = snoozeValues[snoozeBar.Progress];
				Settings.SetAlarmField(alarm_number, Settings.AlarmField.Snooze, snoozeValues[snoozeBar.Progress]);
			};

			int snoozeVal = int.Parse(Settings.GetAlarmField(alarm_number, Settings.AlarmField.Snooze));
			for (int i = 0; i < snoozeValues.Length; i++)
			{
				if (int.Parse(snoozeValues[i]) == snoozeVal)
				{
					snoozeBar.Progress = i;
					break;
				}
			}

			//Song select
			FindViewById<ImageView>(Resource.Id.tone).Click += delegate {
				Intent i = new Intent(ApplicationContext, typeof(SongSelect));
				i.PutExtra("alarm_index", alarm_number);
				StartActivity(i);
			};

			//Module Order
			FindViewById<ImageView>(Resource.Id.moduleOrder).Click += delegate {
				Intent i = new Intent(ApplicationContext, typeof(ModuleOrder));
				i.PutExtra("alarm_index", alarm_number);
				StartActivity(i);
			};

			//Scrollview
			scrollView = FindViewById<CustomHorizontalScrollView>(Resource.Id.module_scroller);
			scroll_layout = FindViewById<LinearLayout>(Resource.Id.module_layout);
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
						case "traffic":
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

			//Tab holder
			RefreshTabs(moduleLayout.ChildCount);
		}

		private void RefreshTabs(int count)
		{
			//Tab holder
			moduleTabs.Clear();
			LinearLayout tabHolder = FindViewById<LinearLayout>(Resource.Id.tab_holder);
			tabHolder.LayoutParameters.Width = (int)(Resources.DisplayMetrics.WidthPixels* .75);
			int numModules = count;
			if (numModules > 0)
			{
				selectedTab = 0;
				int spaceWidth = (int)(tabHolder.Width * .05);
				int totalSpaceSize = spaceWidth * (numModules - 1);
				int tabWidth = (tabHolder.Width - totalSpaceSize) / numModules;

				Console.WriteLine("numModules: " + numModules);

				for (int i = 0; i<numModules; i++)
				{
					View tab = new View(ApplicationContext);
					LinearLayout.LayoutParams tabLp = new LinearLayout.LayoutParams(tabWidth, (int)(2 * Resources.DisplayMetrics.Density));
					tab.LayoutParameters = tabLp;
					tab.SetBackgroundColor(Color.Black);
					tab.Alpha = (i == 0) ? .4f : .15f;
					tabHolder.AddView(tab);
					moduleTabs.Add(tab);

					//Add space
					if (i != numModules - 1)
					{
						View space = new View(ApplicationContext);
						LinearLayout.LayoutParams spaceLp = new LinearLayout.LayoutParams(spaceWidth, (int)(2 * Resources.DisplayMetrics.Density));
						space.LayoutParameters = spaceLp;
						tabHolder.AddView(space);
					}
				}
			}
		}

		private RelativeLayout CreateModuleDisplay(string type, string title, int index, int subindex)
		{
			var metrics = Resources.DisplayMetrics;
			RelativeLayout rl = (RelativeLayout)LayoutInflater.Inflate(Resource.Layout.module_holder, null);
			rl.LayoutParameters = new LinearLayout.LayoutParams((int)(metrics.WidthPixels), LinearLayout.LayoutParams.WrapContent);

			RelativeLayout module = rl.FindViewById<RelativeLayout>(Resource.Id.module);
			RelativeLayout.LayoutParams moduleParams = new RelativeLayout.LayoutParams((int)(metrics.WidthPixels * .85), RelativeLayout.LayoutParams.WrapContent);
			moduleParams.AddRule(LayoutRules.CenterHorizontal);
			module.LayoutParameters = moduleParams;

			TextView settingTitle = rl.FindViewById<TextView>(Resource.Id.setting_title);
			ImageView settingImage = rl.FindViewById<ImageView>(Resource.Id.moduleImage);
			ImageView navButton = rl.FindViewById<ImageView>(Resource.Id.navButton);
			RelativeLayout titleRow = module.FindViewById<RelativeLayout>(Resource.Id.title_row);

			settingTitle.Typeface = Typeface.CreateFromAsset(Resources.Assets, "HelveticaNeueLight.ttf");
			settingTitle.TextSize = (int)((metrics.HeightPixels / metrics.Density) * .06);
			//RelativeLayout.LayoutParams titleParams = (RelativeLayout.LayoutParams)settingTitle.LayoutParameters;
			//titleParams.Width = 
			settingTitle.LayoutParameters.Width = RelativeLayout.LayoutParams.MatchParent;

			parent.Measure(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent);


			//Add Editing
			RelativeLayout displayRow = module.FindViewById<RelativeLayout>(Resource.Id.display_row);
			LinearLayout editor = null;
			bool isToggle = false;
			bool isSeparateActivity = false;
			Settings.Modules modType = 0;
			switch (type)
			{
				case "fact":
					modType = Settings.Modules.FACT;
					isToggle = true;
					break;
				case "quote":
					modType = Settings.Modules.QUOTE;
					isToggle = true;
					break;
				case "tdih":
					modType = Settings.Modules.TDIH;
					isToggle = true;
					break;
				case "weather":
					modType = Settings.Modules.WEATHER;
					settingImage.SetImageResource(Resource.Drawable.weather_icon);
					editor = (LinearLayout)LayoutInflater.Inflate(Resource.Layout.weather, null);
					Weather weather = new Weather(ApplicationContext, index, editor);
					weather.EditSetup(navButton);
					break;
				case "news":
					modType = Settings.Modules.NEWS;
					settingImage.SetImageResource(Resource.Drawable.news_icon);
					editor = (LinearLayout)LayoutInflater.Inflate(Resource.Layout.news, null);
					News news = new News(ApplicationContext, index, editor);
					news.EditSetup(subindex, navButton);
					break;
				case "reddit":
					modType = Settings.Modules.REDDIT;
					settingTitle.Text = "r/" + settingTitle.Text;
					settingImage.SetImageResource(Resource.Drawable.reddit_icon);
					editor = (LinearLayout)LayoutInflater.Inflate(Resource.Layout.reddit, null);
					Reddit reddit = new Reddit(ApplicationContext, index, editor);
					reddit.EditSetup(subindex, navButton);
					break;
				case "twitter":
					modType = Settings.Modules.TWITTER;
					settingImage.SetImageResource(Resource.Drawable.twitter_icon);
					editor = (LinearLayout)LayoutInflater.Inflate(Resource.Layout.twitter, null);
					Twitter twitter = new Twitter(ApplicationContext, index, editor);
					twitter.EditSetup(subindex, navButton);
					break;
				case "countdown":
					modType = Settings.Modules.COUNTDOWN;
					settingImage.SetImageResource(Resource.Drawable.countdown_icon);
					editor = (LinearLayout)LayoutInflater.Inflate(Resource.Layout.countdown, null);
					Countdown countdown = new Countdown(ApplicationContext, index, editor);
					countdown.EditSetup(subindex, navButton);
					break;
				case "reminders":
					modType = Settings.Modules.REMINDERS;
					settingImage.SetImageResource(Resource.Drawable.todo_icon);
					editor = (LinearLayout)LayoutInflater.Inflate(Resource.Layout.reminders, null);
					editor.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
					Reminders reminders = new Reminders(ApplicationContext, index, editor);
					reminders.EditSetup(subindex, navButton);
                    break;
                case "traffic":
                    modType = Settings.Modules.TRAFFIC;
                    settingImage.SetImageResource(Resource.Drawable.traffic_icon);
					isSeparateActivity = true;
					navButton.Click += delegate {
						Intent i = new Intent(ApplicationContext, typeof(TrafficActivity));
						i.PutExtra("alarm_index", index);
						i.PutExtra("subindex", subindex);
						StartActivity(i);
					};
                    break;
			}

			settingTitle.Text = title;

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

			if (isToggle)
			{
				navButton.SetImageResource(Resource.Drawable.delete2);
				navButton.Click += delegate
				{
					//Delete module
					Settings.DeleteModule(index, modType, subindex);
					//Update scrollview layout-----
					//1) Fade module
					rl.Animate().Alpha(0).SetDuration(50).Start();
					//2) Scroll to another module. Scroll left if possible
					if (scrollView.ScrollX > 0)
					{
						scrollView.AnimateScrollTo(0);
					}
					//scrollView.scr
					//3) remove module
					Handler handler = new Handler();
					handler.PostDelayed(delegate
					{
						FindViewById<LinearLayout>(Resource.Id.module_layout).RemoveView(rl);
					}, 200);
				};
			}
			else if(!isSeparateActivity)
			{
				navButton.Click += delegate
				{
					AnimationHelper editorFade = new AnimationHelper(editor, null);
					AnimationHelper imageFade = new AnimationHelper(settingImage, null);
					AnimationHelper alarmToggleFade = new AnimationHelper(alarm_toggle, null);
					AnimationHelper addModuleBtnFade = new AnimationHelper(addModuleBtn, null);
					AnimationHelper settingsBtnFade = new AnimationHelper(settingsBtn, null);

					AnimationHelper clockHeight = new AnimationHelper(clock_settings_layout, new AnimationManager(clock_settings_layout.Height > 0));
					AnimationHelper moduleFrameHeight = new AnimationHelper(moduleFrame, new AnimationManager(clock_settings_layout.Height == 0));
					int targetHeight = (int)(metrics.HeightPixels * .6);

					if (editor.Alpha < .5f
						&& settingsContainer.Height == 0) //open editor
					{
						scrollView.SetOnTouchListener(new ScrollViewOffListener());
						editorFade.Fade(200, 1f);
						imageFade.Fade(200, 0f);
						navButton.SetImageResource(Resource.Drawable.back_button);
						if (settingTitle.Text != "Weather")
						{
							clockHeight.collapse(200, 0);
							targetHeight = editor.Height + titleRow.Height + (int)(20 * metrics.Density);
							moduleFrameHeight.collapse(200, targetHeight);
							alarmToggleFade.Fade(100, 0);
							addModuleBtnFade.Fade(100, 0);
							settingsBtnFade.Fade(100, 0);
						}

						scrollView.HorizontalScrollBarEnabled = false;
						currentModuleNavButton = navButton;
					}
					else
					{ //close editor
						scrollView.SetOnTouchListener(new ScrollViewOnListener());
						editorFade.Fade(200, 0f);
						imageFade.Fade(200, 1f);
						navButton.SetImageResource(Resource.Drawable.edit_button);
						if (settingTitle.Text != "Weather")
						{
							clockHeight.expand(200, (int)(metrics.HeightPixels * .4));
							moduleFrameHeight.expand(200, (int)(metrics.HeightPixels * .6));
							alarmToggleFade.Fade(100, 1f);
							addModuleBtnFade.Fade(100, 1f);
							settingsBtnFade.Fade(100, 1f);
						}

						View focus = CurrentFocus;
						if (focus != null)
						{
							InputMethodManager imm = (InputMethodManager)ApplicationContext.GetSystemService("input_method");
							imm.HideSoftInputFromWindow(focus.WindowToken, 0);
						}

						currentModuleNavButton = null;
					}
				};
			}

			return rl;
		}

		public override void OnBackPressed()
		{
			if (buttonRow.Alpha < .5f)
			{
				Console.WriteLine("fading out");
				settingsBtn.SetImageResource(Resource.Drawable.settings_button);
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
			else if (currentModuleNavButton != null)
			{
				currentModuleNavButton.PerformClick();
			}
			else {
				base.OnBackPressed();
			}
		}

		protected override void OnResume()
		{
			base.OnResume();
			Console.WriteLine("resuming alarm screen");
			instance = this;
			LinearLayout moduleLayout = FindViewById<LinearLayout>(Resource.Id.module_layout);
			while(moduleLayout.ChildCount > 0) moduleLayout.RemoveViewAt(0);
			RefreshModules(Intent.GetIntExtra("alarm_number", -1));

			scrollView.setOnScrollChangedListener(new HorizontalScrollListener());
			if ((int)Build.VERSION.SdkInt >= 23)
			{
				if (CheckSelfPermission(Android.Manifest.Permission.ReadExternalStorage)
			   == Permission.Granted && ManageAlarms.songsRadioGroup == null)
				{
					new ManageAlarms.GetSongs().Execute();
				}
			}
			else if (ManageAlarms.songsRadioGroup == null)
			{
				new ManageAlarms.GetSongs().Execute();
			}
				
		}

		protected override void OnPause()
		{
			Console.WriteLine("pausing alarm screen");
			base.OnPause();
		}

		protected override void OnStop()
		{
			Console.WriteLine("stopping alarm screen");
			base.OnStop();
		}

		protected override void OnDestroy()
		{
			Console.WriteLine("destroying alarm screen");
			instance = null;
			base.OnDestroy();
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

		public class ScrollViewOnListener : Java.Lang.Object, View.IOnTouchListener
		{
			public bool OnTouch(View v, MotionEvent e)
			{
				return false;
			}
		}

		public class ScrollViewOffListener : Java.Lang.Object, View.IOnTouchListener
		{
			public bool OnTouch(View v, MotionEvent e)
			{
				return true;
			}
		}

		public class HorizontalScrollListener : CustomHorizontalScrollView.IOnScrollChangedListener
		{
			public void onScrollEnd()
			{
				Console.WriteLine("scroll ended");
				LinearLayout scroll_layout = (LinearLayout)((ViewGroup)scrollView).GetChildAt(0);
				int moduleWidth = DisplayMetrics.WidthPixels;
				int numModules = scroll_layout.Width / moduleWidth;
				Console.WriteLine("num modules: " + numModules);

				currentModule = (scrollView.ScrollX + (moduleWidth/2))/ moduleWidth;

				int target = currentModule * moduleWidth;
				scrollView.AnimateScrollTo(target);

				if (selectedTab != -1) moduleTabs[selectedTab].Animate().Alpha(.15f).SetDuration(100).Start();
				moduleTabs[currentModule].Animate().Alpha(.4f).SetDuration(100).Start();
				selectedTab = currentModule;
			}

			public void onScrollStart()
			{
				Console.WriteLine("scroll started");
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

		public void ToggleScrolling(bool enabled)
		{
			if(enabled)
				scrollView.SetOnTouchListener(new MainActivity.ScrollViewOnListener());
			else
				scrollView.SetOnTouchListener(new MainActivity.ScrollViewOffListener());
		}                        
	}
}

