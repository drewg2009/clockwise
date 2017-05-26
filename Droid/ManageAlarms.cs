
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Animation;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Clockwise.Helpers;
using Android.Content.PM;
using Java.Util;
using Android.Gms.Ads;

namespace Clockwise.Droid
{
    [Activity(Label = "Clockwise", MainLauncher = true, Icon = "@mipmap/icon")]
    public class ManageAlarms : Activity
    {
        public static ManageAlarms instance = null;
        public static SongManager sm = null;
        public static List<Song> songList = null;
        public static List<Song> defaultList = null;
        public static RadioGroup defaultsRadioGroup = null;
        public static RadioGroup songsRadioGroup = null;
        public static Typeface fontLight = null;
        public static Typeface fontBold = null;
		private InterstitialAd mInterstitialAd;
		private int selectedMonth;
        private int selectedDayOfMonth;
        private int selectedYear;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
			// Create your application here
			SetContentView(Resource.Layout.manage_alarms);
            instance = this;

            View view = FindViewById<LinearLayout>(Resource.Id.root);

            fontLight = Typeface.CreateFromAsset(Resources.Assets, "HelveticaNeueLight.ttf");
            fontBold = Typeface.CreateFromAsset(Resources.Assets, "HelveticaNeueBold.ttf");


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
            minutepicker.SetFormatter(new MainActivity.TwoDigitFormatter());

            ampmpicker.MaxValue = 1;
            ampmpicker.MinValue = 0;
            ampmpicker.SetDisplayedValues(new string[] { "am", "pm" });

            //Set default time
            hourpicker.Value = (6);
            minutepicker.Value = (0);
            ampmpicker.Value = (0);

            //Repeat days
            TextView[] repeatDays = new TextView[7];
            repeatDays[0] = FindViewById<TextView>(Resource.Id.sundayInput);
            repeatDays[1] = FindViewById<TextView>(Resource.Id.mondayInput);
            repeatDays[2] = FindViewById<TextView>(Resource.Id.tuesdayInput);
            repeatDays[3] = FindViewById<TextView>(Resource.Id.wednesdayInput);
            repeatDays[4] = FindViewById<TextView>(Resource.Id.thursdayInput);
            repeatDays[5] = FindViewById<TextView>(Resource.Id.fridayInput);
            repeatDays[6] = FindViewById<TextView>(Resource.Id.saturdayInput);

            int repeatDaysResult = 0;
            for (int i = 0; i < 7; i++)
            {
                TextView tv = repeatDays[i];
                tv.SetTextColor(Color.ParseColor("#CCCCCC"));
                int temp = 1 << i;
                tv.Click += delegate
                {
                    //Turn on
                    if (tv.CurrentTextColor == Color.ParseColor("#CCCCCC"))
                    {
                        //tv.setTextColor(Color.parseColor("#7BD678"));
                        ObjectAnimator colorAnim = ObjectAnimator.OfInt(tv, "textColor",
                                Color.ParseColor("#CCCCCC"), Color.ParseColor("#7BD678"));
                        colorAnim.SetEvaluator(new ArgbEvaluator());
                        colorAnim.Start();
                        tv.Typeface = fontBold;
                    }
                    //Turn off
                    else
                    {
                        ObjectAnimator colorAnim = ObjectAnimator.OfInt(tv, "textColor",
                                    Color.ParseColor("#7BD678"), Color.ParseColor("#CCCCCC"));
                        colorAnim.SetEvaluator(new ArgbEvaluator());
                        colorAnim.Start();
                        tv.Typeface = fontLight;
                    }

                    repeatDaysResult = repeatDaysResult ^ temp;
                };

            }

            FindViewById<TextView>(Resource.Id.date_text).Typeface = fontLight;

            //Options
            TextView options = FindViewById<TextView>(Resource.Id.options_button);
            RelativeLayout optionsRow = FindViewById<RelativeLayout>(Resource.Id.options_row);

            options.Typeface = fontLight;
            ScrollView scrollView = FindViewById<ScrollView>(Resource.Id.settings_scroller);
            LinearLayout settings = (LinearLayout)((ViewGroup)scrollView).GetChildAt(0);
            SeekBar volume = settings.FindViewById<SeekBar>(Resource.Id.volume_seek_bar);
            RelativeLayout alarmToneRow = settings.FindViewById<RelativeLayout>(Resource.Id.alarm_tone_row);
            alarmToneRow.Click += delegate
            {
                StartActivity(typeof(SongSelect));
            };

            RelativeLayout alarmNameRow = settings.FindViewById<RelativeLayout>(Resource.Id.alarm_name_row);
            TextView nameText = settings.FindViewById<TextView>(Resource.Id.name_text);
            alarmNameRow.Click += delegate
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(this);
                builder.SetTitle("Alarm Name");
                EditText input = new EditText(this);
                //input.InputType = Android.Text.InputTypes.ClassText | Android.Text.InputTypes.TextVariationPassword;
                builder.SetView(input);
                builder.SetPositiveButton("OK", (sender, e) =>
                {
                    if (!input.Text.Contains(Settings.SEPARATERS))
                        nameText.Text = input.Text;
                    else
                        Toast.MakeText(ApplicationContext, "Alarm name cannot have special characters", ToastLength.Long).Show();
                });

                builder.SetNegativeButton("CANCEL", (object sender, DialogClickEventArgs e) =>
                {
                    ((Dialog)sender).Dismiss();
                });

                builder.Show();
            };

            SeekBar snoozeBar = settings.FindViewById<SeekBar>(Resource.Id.snooze_bar);
            TextView snoozeOutput = FindViewById<TextView>(Resource.Id.snooze_output);
            snoozeOutput.Typeface = fontLight;
            string[] snoozeValues = Resources.GetStringArray(Resource.Array.snooze_values);
            snoozeBar.Max = snoozeValues.Length - 1;
            snoozeBar.ProgressChanged += delegate
            {
                snoozeOutput.Text = snoozeValues[snoozeBar.Progress];
            };


            RelativeLayout clockSettings = FindViewById<RelativeLayout>(Resource.Id.clock_settings);
            options.Click += delegate
            {
                AnimationHelper scrollViewHeight = new AnimationHelper(scrollView,
                                                                       new AnimationManager(scrollView.Height > 0));
                if (scrollView.Height == 0)
                {
                    //expand
                    int targetHeight = view.Height - clockSettings.Height - optionsRow.Height;
                    scrollViewHeight.expand(200, targetHeight);
                    options.Text = "Hide Options";
                }
                else
                {
                    //collapse
                    scrollViewHeight.collapse(200, 0);
                    options.Text = "Options";
                    volume.Progress = 10;
                    snoozeBar.Progress = 2;
                    nameText.Text = "None";

                }
            };

            TextView dateOutput = FindViewById<TextView>(Resource.Id.date_output);
            FindViewById<TextView>(Resource.Id.save_button).Typeface = fontLight;
            FindViewById<TextView>(Resource.Id.save_button).Click += delegate
            {
                String alarmName = "";
                if (scrollView.Height > 0)
                {
                    if (nameText.Text != "None") alarmName = nameText.Text;

                    options.PerformClick();
                }
                String[] currentAlarms = Settings.Alarms.Split('|');
                //Turn alarm on
                int hourSet = hourpicker.Value + ampmpicker.Value * 12;
                if (hourSet == 12 && ampmpicker.Value == 0) hourSet = 0;
                if (hourSet == 24) hourSet = 12;
                Console.WriteLine("passing time: " + hourSet + ":" + minutepicker.Value);
                long alarmTimeInMillis;
                if (dateOutput.Text != "")
                {

                    alarmTimeInMillis = AlarmUtils.SetTime(Application.Context, selectedMonth, selectedDayOfMonth,
                                                           selectedYear, hourSet, minutepicker.Value,
                                       Settings.Alarms == string.Empty ? 0 : currentAlarms.Length, repeatDaysResult,
                                       int.Parse(snoozeValues[snoozeBar.Progress]), (volume.Progress + 1), alarmName);
                }
                else
                {
                    alarmTimeInMillis = AlarmUtils.SetTime(Application.Context, hourSet, minutepicker.Value,
                                       Settings.Alarms == string.Empty ? 0 : currentAlarms.Length, repeatDaysResult, true,
                                       int.Parse(snoozeValues[snoozeBar.Progress]), (volume.Progress + 1), alarmName);
                }


                foreach (TextView tv in repeatDays)
                {
                    tv.SetTextColor(Color.ParseColor("#CCCCCC"));
                    tv.Typeface = fontLight;
                }

                repeatDaysResult = 0;

                //Add view
                LinearLayout alarmViwer = (LinearLayout)FindViewById(Resource.Id.alarm_viewer);
                LinearLayout alarmRow = (LinearLayout)LayoutInflater.Inflate(Resource.Layout.alarm_display, null);
                LinearLayout editLayout = alarmRow.FindViewById<LinearLayout>(Resource.Id.edit_alarm_layout);
                View deleteImage = alarmRow.FindViewById(Resource.Id.delete_button);
                if (alarmName != "")
                {
                    if (alarmName.Length > 12)
                    {
                        alarmName = alarmName.Substring(0, 12) + "...";
                    }
                    TextView alarmNameText = alarmRow.FindViewById<TextView>(Resource.Id.name_text);
                    alarmNameText.Text = alarmName;
                }

                TextView dateText = alarmRow.FindViewById<TextView>(Resource.Id.date_text);
                dateText.Typeface = fontLight;
                Calendar calender = Calendar.Instance;
                calender.TimeInMillis = alarmTimeInMillis;
                int dayOfWeek = calender.Get(CalendarField.DayOfWeek) - 1;
                int month = calender.Get(CalendarField.Month);
                int date = calender.Get(CalendarField.DayOfMonth);
                dateText.Text = Resources.GetStringArray(Resource.Array.week_days)[dayOfWeek]
                    + ", " + Resources.GetStringArray(Resource.Array.months)[month]
                    + " " + date;

                editLayout.Tag = Settings.Alarms.Split('|').Length - 1;
                editLayout.Click += delegate
                {
                    Intent editAlarm = new Intent(Application.Context, typeof(MainActivity));
                    editAlarm.PutExtra("alarm_number", (int)editLayout.Tag);
                    StartActivity(editAlarm);
                };

                var metrics = Resources.DisplayMetrics;
                alarmRow.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent,
                                                                          (int)(metrics.HeightPixels * .1));
                TextView alarmTime = alarmRow.FindViewById<TextView>(Resource.Id.alarm_time);
                ImageView alarmStatus = alarmRow.FindViewById<ImageView>(Resource.Id.alarm_status);
                alarmStatus.SetImageResource(Resource.Drawable.alarm_on);

                deleteImage.Click += delegate
                {
                    Console.Write("deleting alarm");
                    int status = int.Parse(Settings.GetAlarmField((int)alarmRow.Tag, Settings.AlarmField.Status));
                    if (status == (int)Settings.AlarmStatus.ALARM_ON)
                        AlarmUtils.Cancel(Application.Context, (int)editLayout.Tag, false);
                    Settings.DeleteAlarm((int)editLayout.Tag);
                    Console.Write("New alarms: " + Settings.Alarms);
                    RefreshAlarms();
                };

                alarmTime.Typeface = fontLight;
                alarmTime.Text = hourpicker.Value + ":" + minutepicker.Value.ToString("00") + " " + (ampmpicker.Value == 0 ? "am" : "pm");

                View gap = new View(Application.Context);
                LinearLayout.LayoutParams gap_params = new LinearLayout.LayoutParams(
                    LinearLayout.LayoutParams.MatchParent, (int)(10 * metrics.Density));
                gap.LayoutParameters = gap_params;
                alarmViwer.AddView(gap);
                alarmViwer.AddView(alarmRow);

                dateOutput.Text = "";
            };



            View tonePage = LayoutInflater.Inflate(Resource.Layout.fragment_page, null);
            View alarmPage = LayoutInflater.Inflate(Resource.Layout.fragment_page, null);

            defaultsRadioGroup = tonePage.FindViewById<RadioGroup>(Resource.Id.tone_radio_group);
            songsRadioGroup = alarmPage.FindViewById<RadioGroup>(Resource.Id.tone_radio_group);
            LinearLayout defParent = (LinearLayout)defaultsRadioGroup.Parent;
            defParent.RemoveView(defaultsRadioGroup);
            LinearLayout songParent = (LinearLayout)songsRadioGroup.Parent;
            songParent.RemoveView(songsRadioGroup);

            sm = SongManager.getInstance(this);
            //defaultList = sm.getDefaultList();
            //sm = SongManager.getInstance(this);

            if (defaultList == null)
            {
                new GetDefaults().Execute();
            }

            //External Songs
            if ((int)Build.VERSION.SdkInt >= 23)
            {
                if (CheckSelfPermission(
                Android.Manifest.Permission.ReadExternalStorage)
                != Permission.Granted)
                {
                    RequestPermissions(
                            new String[] { Android.Manifest.Permission.ReadExternalStorage },
                            1);
                }
                else
                {
                    //songList = sm.getSongList();
                    new GetSongs().Execute();
                }
            }
            else
            {
                new GetSongs().Execute();
            }



            //Date picker
            View dateLayout = FindViewById(Resource.Id.date_layout);
            TextView date_text = FindViewById<TextView>(Resource.Id.date_text);
            dateOutput.Typeface = fontLight;
            date_text.Typeface = fontLight;
            dateLayout.Click += delegate
            {
                //Launch datepicker
                Calendar calendar = Calendar.Instance;
                calendar.TimeInMillis = Java.Lang.JavaSystem.CurrentTimeMillis();
                int year = calendar.Get(CalendarField.Year);
                int month = calendar.Get(CalendarField.Month);
                int day = calendar.Get(CalendarField.DayOfMonth);

                DatePickerDialog dateWindow = new DatePickerDialog(this, (object sender2, DatePickerDialog.DateSetEventArgs e2) =>
                {
                    calendar.Set(e2.Year, e2.Month, e2.DayOfMonth);
                    int dayOfWeek = calendar.Get(CalendarField.DayOfWeek) - 1;
                    int dayOfMonth = calendar.Get(CalendarField.DayOfMonth);
                    int month2 = calendar.Get(CalendarField.Month);
                    int year2 = calendar.Get(CalendarField.Year);

                    selectedMonth = e2.Month;
                    selectedDayOfMonth = e2.DayOfMonth;
                    selectedYear = e2.Year;

                    dateOutput.Text = Resources.GetStringArray(Resource.Array.week_days)[dayOfWeek]
                        + ", " + Resources.GetStringArray(Resource.Array.months)[month2]
                        + " " + dayOfMonth + ", " + year2;
                }, year, month, day);
                dateWindow.Window.SetType(WindowManagerTypes.ApplicationPanel);
                //set min date to current date/time
                dateWindow.DatePicker.MinDate = calendar.TimeInMillis - 1000;
                dateWindow.Show();
            };

            //About
            FindViewById(Resource.Id.about_button).Click += delegate
            {
                StartActivity(typeof(About));
            };
        }


        protected override void OnResume()
        {
            base.OnResume();
			AdCheck();

			RefreshAlarms();

            TextView[] repeatDays = new TextView[7];
            repeatDays[0] = FindViewById<TextView>(Resource.Id.sundayInput);
            repeatDays[1] = FindViewById<TextView>(Resource.Id.mondayInput);
            repeatDays[2] = FindViewById<TextView>(Resource.Id.tuesdayInput);
            repeatDays[3] = FindViewById<TextView>(Resource.Id.wednesdayInput);
            repeatDays[4] = FindViewById<TextView>(Resource.Id.thursdayInput);
            repeatDays[5] = FindViewById<TextView>(Resource.Id.fridayInput);
            repeatDays[6] = FindViewById<TextView>(Resource.Id.saturdayInput);

            foreach (TextView tv in repeatDays)
            {
                tv.SetTextColor(Color.ParseColor("#CCCCCC"));
                tv.Typeface = Typeface.CreateFromAsset(Resources.Assets, "HelveticaNeueLight.ttf");
            }

            instance = this;
        }

        protected override void OnStop()
        {
            //instance = null;
            base.OnStop();
        }

        public void RefreshAlarms()
        {
            Console.Write("New alarms: " + Settings.Alarms);
            LinearLayout alarmViewer = FindViewById<LinearLayout>(Resource.Id.alarm_viewer);
            while (alarmViewer.ChildCount > 0) alarmViewer.RemoveViewAt(0);
            string alarmSettings = Settings.Alarms;

            if (alarmSettings != string.Empty)
            {

                string[] alarms = Settings.Alarms.Split('|');
                for (int i = 0; i < alarms.Length; i++)
                {
                    int hour = int.Parse(Settings.GetAlarmField(i, Settings.AlarmField.Hour));
                    int minute = int.Parse(Settings.GetAlarmField(i, Settings.AlarmField.Minute));
                    bool am = hour < 12;
                    if (!am && hour != 12)
                    {
                        hour -= 12;
                    }
                    if (am && hour == 0) hour = 12;
                    //hour = am ? hour : hour - 12;
                    AddAlarm(alarmViewer, i, hour, minute, am ? 0 : 1);
                }
            }
        }

        private void AddAlarm(LinearLayout alarmViewer, int alarmIndex, int hour, int minute, int ampm)
        {
            //Add view
            LinearLayout alarmRow = (LinearLayout)LayoutInflater.Inflate(Resource.Layout.alarm_display, null);
            LinearLayout editLayout = alarmRow.FindViewById<LinearLayout>(Resource.Id.edit_alarm_layout);
            View deleteImage = alarmRow.FindViewById(Resource.Id.delete_button);

            string alarmName = Settings.GetAlarmField(alarmIndex, Settings.AlarmField.Name);
            if (alarmName != Settings.EMPTY_MODULE)
            {
                if (alarmName.Length > 12)
                {
                    alarmName = alarmName.Substring(0, 12) + "...";
                }
                TextView alarmNameText = alarmRow.FindViewById<TextView>(Resource.Id.name_text);
                alarmNameText.Text = alarmName;
                alarmNameText.Typeface = fontLight;
            }
            editLayout.Tag = alarmIndex;

            //Attaches editing action
            editLayout.Click += delegate
            {

                Intent editAlarm = new Intent(Application.Context, typeof(MainActivity));
                editAlarm.PutExtra("alarm_number", (int)editLayout.Tag);
                StartActivity(editAlarm);
            };

            //Attaches delete action
            deleteImage.Click += delegate
            {
                Console.Write("deleting alarm");
                int temp = int.Parse(Settings.GetAlarmField((int)editLayout.Tag, Settings.AlarmField.Status));
                if (temp == (int)Settings.AlarmStatus.ALARM_ON)
                    AlarmUtils.Cancel(Application.Context, (int)editLayout.Tag, false);
                Settings.DeleteAlarm((int)editLayout.Tag);
                Console.Write("New alarms: " + Settings.Alarms);
                RefreshAlarms();
            };

            var metrics = Resources.DisplayMetrics;
            alarmRow.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent,
                                                                      (int)(metrics.HeightPixels * .1));
            TextView alarmTime = alarmRow.FindViewById<TextView>(Resource.Id.alarm_time);
            alarmTime.Typeface = Typeface.CreateFromAsset(Resources.Assets, "HelveticaNeueLight.ttf");
            alarmTime.Text = hour + ":" + minute.ToString("00") + (ampm == 0 ? " am" : " pm");
            //alarmTime.TextSize = metrics.HeightPixels

            ImageView alarmStatus = alarmRow.FindViewById<ImageView>(Resource.Id.alarm_status);
            int status = int.Parse(Settings.GetAlarmField(alarmIndex, Settings.AlarmField.Status));

            TextView dateText = alarmRow.FindViewById<TextView>(Resource.Id.date_text);
            if (status == (int)Settings.AlarmStatus.ALARM_ON)
            {
                alarmStatus.SetImageResource(Resource.Drawable.alarm_on);
                dateText.Typeface = fontLight;
                long alarmTimeInMillis = long.Parse(Settings.GetAlarmField(alarmIndex, Settings.AlarmField.Millis));
                Calendar calender = Calendar.Instance;
                calender.TimeInMillis = alarmTimeInMillis;
                int dayOfWeek = calender.Get(CalendarField.DayOfWeek) - 1;
                int month = calender.Get(CalendarField.Month);
                int date = calender.Get(CalendarField.DayOfMonth);
                dateText.Text = Resources.GetStringArray(Resource.Array.week_days)[dayOfWeek]
                    + ", " + Resources.GetStringArray(Resource.Array.months)[month]
                    + " " + date;
            }
            else
                dateText.Text = "";


            View gap = new View(Application.Context);
            LinearLayout.LayoutParams gap_params = new LinearLayout.LayoutParams(
                LinearLayout.LayoutParams.MatchParent, (int)(10 * metrics.Density));
            gap.LayoutParameters = gap_params;
            alarmViewer.AddView(gap);
            alarmViewer.AddView(alarmRow);
        }

        public override async void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            switch (requestCode)
            {
                case 1:
                    {

                        // If request is cancelled, the result arrays are empty.
                        if (grantResults.Length > 0
                            && grantResults[0] == Permission.Granted)
                        {

                            // permission was granted, yay! Do the
                            // contacts-related task you need to do.
                            Settings.AndroidFileAccess = "true";
                            Toast.MakeText(Android.App.Application.Context, "Permission granted to access song files", ToastLength.Short).Show();
                            //sm = SongManager.getInstance(this);
                            //songList = sm.getSongList();
                            new GetSongs().Execute();
                        }
                        else
                        {

                            // permission denied, boo! Disable the
                            // functionality that depends on this permission.
                            Toast.MakeText(Android.App.Application.Context, "Permission denied to access song files", ToastLength.Short).Show();

                        }
                        break;
                    }
            }
        }

        public class GetDefaults : AsyncTask
        {

            protected override void OnPreExecute()
            {
            }

            protected override Java.Lang.Object DoInBackground(params Java.Lang.Object[] @params)
            {
                defaultList = SongManager.getInstance(instance).getDefaultList();
                var metrics = instance.Resources.DisplayMetrics;

                for (int i = 0; i < defaultList.Count; i++)
                {
                    RadioButton rb = new RadioButton(instance.ApplicationContext);
                    rb.Text = defaultList[i].Title;
                    rb.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
                    rb.SetTextColor(Color.Gray);
                    rb.Typeface = fontLight;
                    rb.TextSize = 20;
                    rb.SetPadding(0, (int)(12 * metrics.Density), 0, (int)(12 * metrics.Density));
                    rb.LetterSpacing = .1f;

                    int temp = i;
                    rb.Click += delegate
                    {
                        //play
                        if (sm.isPlaying()) sm.stop();
                        sm.play(defaultList[temp].getUri().ToString());
                        sm.playingIndex = temp;
                    };

                    //Space
                    View space = new View(instance.ApplicationContext);
                    space.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, 1);
                    space.SetBackgroundColor(Color.Gray);
                    space.SetPadding((int)(10 * metrics.Density), 0, 0, 0);

                    defaultsRadioGroup.AddView(rb);
                    defaultsRadioGroup.AddView(space);
                }


                return null;
            }

            protected override void OnPostExecute(Java.Lang.Object result)
            {
            }
        }

        public class GetSongs : AsyncTask
        {
            protected override void OnPreExecute()
            {
            }

            protected override Java.Lang.Object DoInBackground(params Java.Lang.Object[] @params)
            {
                songList = SongManager.getInstance(instance).getSongList();

                if (songList != null && songList.Count > 0)
                {
                    var metrics = instance.Resources.DisplayMetrics;
                    for (int i = 0; i < songList.Count; i++)
                    {
                        RadioButton rb = new RadioButton(instance.ApplicationContext);
                        rb.Text = songList[i].Title;
                        rb.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
                        rb.SetTextColor(Color.Gray);

                        rb.Typeface = fontLight;
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
                        };

                        //Space
                        View space = new View(instance.ApplicationContext);
                        space.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, 1);
                        space.SetBackgroundColor(Color.Gray);
                        space.SetPadding((int)(10 * metrics.Density), 0, 0, 0);

                        songsRadioGroup.AddView(rb);
                        songsRadioGroup.AddView(space);
                    }

                }

                return null;
            }

            protected override void OnPostExecute(Java.Lang.Object result)
            {
            }



        }

		private void RequestNewInterstitial()
		{
			AdRequest adRequest = new AdRequest.Builder().Build();
			mInterstitialAd.LoadAd(adRequest);
		}


		private void AdCheck()
		{
			mInterstitialAd = new InterstitialAd(this);
			mInterstitialAd.AdUnitId = GetString(Resource.String.interstitial_ad_unit_id);

			if (!mInterstitialAd.IsLoaded)
			{
				RequestNewInterstitial();
			}

			var adListener = new CustomAdlistener();
			mInterstitialAd.AdListener = adListener;
			adListener.AdLoaded += () => {
				if (Helpers.Settings.AdsHomeScreen > 4)
				{
					if (mInterstitialAd.IsLoaded)
					{
						mInterstitialAd.Show();
					}

					Helpers.Settings.AdsHomeScreen = 0;
				}
				else
				{
					Helpers.Settings.AdsHomeScreen += 1;
				}
            };


		}

	}




}
