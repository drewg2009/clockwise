﻿
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

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			// Create your application here
			SetContentView(Resource.Layout.manage_alarms);
			instance = this;

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
					else {
						ObjectAnimator colorAnim = ObjectAnimator.OfInt(tv, "textColor",
									Color.ParseColor("#7BD678"), Color.ParseColor("#CCCCCC"));
						colorAnim.SetEvaluator(new ArgbEvaluator());
						colorAnim.Start();
						tv.Typeface = fontLight;
					}

					repeatDaysResult = repeatDaysResult ^ temp;
				};

			}
			FindViewById<TextView>(Resource.Id.save_button).Typeface = fontBold;
			FindViewById<TextView>(Resource.Id.save_button).Click += delegate {
				String[] currentAlarms = Settings.Alarms.Split('|');
				//Turn alarm on
				int hourSet = hourpicker.Value + ampmpicker.Value * 12;
				if (hourSet == 12 && ampmpicker.Value == 0) hourSet = 0;
				if (hourSet == 24) hourSet = 12;
				Console.WriteLine("passing time: " + hourSet + ":" + minutepicker.Value);
				AlarmUtils.SetTime(Android.App.Application.Context, hourSet, minutepicker.Value, 
				                   Settings.Alarms == string.Empty ? 0 : currentAlarms.Length, repeatDaysResult, true);

				//Save repeat days
				Settings.RepeatDays += Settings.RepeatDays == string.Empty ? "" + repeatDaysResult : "|" + repeatDaysResult;

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
				LinearLayout deleteLayout = alarmRow.FindViewById<LinearLayout>(Resource.Id.delete_alarm_layout);

				editLayout.Tag = Settings.Alarms.Split('|').Length - 1;
				editLayout.Click += delegate {
					Intent editAlarm = new Intent(Application.Context, typeof(MainActivity));
					editAlarm.PutExtra("alarm_number", (int) editLayout.Tag);
					StartActivity(editAlarm);
				};

				var metrics = Resources.DisplayMetrics;
				alarmRow.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent,
				                                                          (int)(metrics.HeightPixels * .1));
				TextView alarmTime = alarmRow.FindViewById<TextView>(Resource.Id.alarm_time);
				ImageView alarmStatus = alarmRow.FindViewById<ImageView>(Resource.Id.alarm_status);
				alarmStatus.SetImageResource(Resource.Drawable.alarm_on);

				deleteLayout.Click += delegate {
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
					LinearLayout.LayoutParams.MatchParent, (int)(10*metrics.Density));
				gap.LayoutParameters = gap_params;
				alarmViwer.AddView(gap);
				alarmViwer.AddView(alarmRow);

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

		protected override void OnResume()
		{
			base.OnResume();
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
					if(!am && hour != 12){
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
			LinearLayout deleteLayout = alarmRow.FindViewById<LinearLayout>(Resource.Id.delete_alarm_layout);
			editLayout.Tag = alarmIndex;

			//Attaches editing action
			editLayout.Click += delegate
			{
				
				Intent editAlarm = new Intent(Application.Context, typeof(MainActivity));
				editAlarm.PutExtra("alarm_number", (int)editLayout.Tag);
				StartActivity(editAlarm);
			};

			//Attaches delete action
			deleteLayout.Click += delegate
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

			if (status == (int)Settings.AlarmStatus.ALARM_ON) 
				alarmStatus.SetImageResource(Resource.Drawable.alarm_on);


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
						else {

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

	}
}
