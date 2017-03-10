
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

namespace Clockwise.Droid
{
	[Activity(Label = "Clockwise", MainLauncher = true, Icon = "@mipmap/icon")]
	public class ManageAlarms : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Create your application here
			SetContentView(Resource.Layout.manage_alarms);

			Typeface fontLight = Typeface.CreateFromAsset(Resources.Assets, "HelveticaNeueLight.ttf");
			Typeface fontBold = Typeface.CreateFromAsset(Resources.Assets, "HelveticaNeueBold.ttf");


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
			FindViewById<TextView>(Resource.Id.save_button).Click += delegate {
				String[] currentAlarms = Settings.AlarmTime.Split('|');
				//Turn alarm on
				int hourSet = hourpicker.Value + ampmpicker.Value * 12;
				if (hourSet == 12 && ampmpicker.Value == 0) hourSet = 0;
				if (hourSet == 24) hourSet = 12;
				Console.WriteLine("passing time: " + hourSet + ":" + minutepicker.Value);
				AlarmUtils.SetTime(Android.App.Application.Context, hourSet, minutepicker.Value, 
				                   Settings.AlarmTime == string.Empty ? 0 : currentAlarms.Length, repeatDaysResult, true);

				//Save repeat days
				Settings.RepeatDays += Settings.RepeatDays == string.Empty ? "" + repeatDaysResult : "|" + repeatDaysResult;
				repeatDaysResult = 0;

				//Add view
				LinearLayout alarmViwer = (LinearLayout)FindViewById(Resource.Id.alarm_viewer);
				RelativeLayout alarmRow = (RelativeLayout)LayoutInflater.Inflate(Resource.Layout.alarm_display, null);


				alarmRow.Tag = Settings.AlarmTime.Split('|').Length - 1;
				alarmRow.Click += delegate {
					Intent editAlarm = new Intent(Application.Context, typeof(MainActivity));
					editAlarm.PutExtra("alarm_number", (int) alarmRow.Tag);
					StartActivity(editAlarm);
				};

				var metrics = Resources.DisplayMetrics;

				alarmRow.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent,
				                                                          (int)(metrics.HeightPixels * .1));
				TextView alarmTime = alarmRow.FindViewById<TextView>(Resource.Id.alarm_time);
				alarmTime.Typeface = fontLight;
				alarmTime.Text = hourpicker.Value + ":" + minutepicker.Value.ToString("00") + " " + (ampmpicker.Value == 0 ? "am" : "pm");

				View gap = new View(Application.Context);
				LinearLayout.LayoutParams gap_params = new LinearLayout.LayoutParams(
					LinearLayout.LayoutParams.MatchParent, (int)(10*metrics.Density));
				gap.LayoutParameters = gap_params;
				alarmViwer.AddView(gap);
				alarmViwer.AddView(alarmRow);

			};
		}

		protected override void OnResume()
		{
			base.OnResume();
			LinearLayout alarmViewer = FindViewById<LinearLayout>(Resource.Id.alarm_viewer);
			while (alarmViewer.ChildCount > 0) alarmViewer.RemoveViewAt(0);
			AddAlarms();
		}

		private void AddAlarms()
		{
			LinearLayout alarmViewer = FindViewById<LinearLayout>(Resource.Id.alarm_viewer);
			string alarmSettings = Settings.AlarmTime;
			if (alarmSettings != string.Empty)
			{
				
				string[] alarms = Settings.AlarmTime.Split('|');
				for (int i = 0; i < alarms.Length; i++)
				{
					int hour = System.Int32.Parse(alarms[i].Split(':')[1]);
					int minute = System.Int32.Parse(alarms[i].Split(':')[2]);
					bool am = hour < 12;
					hour = am ? hour : hour - 12;
					AddAlarm(alarmViewer, i, hour, minute, am ? 0 : 1); 
				}
			}
		}

		private void AddAlarm(LinearLayout alarmViewer, int alarmIndex, int hour, int minute, int ampm)
		{
			//Add view
			RelativeLayout alarmRow = (RelativeLayout)LayoutInflater.Inflate(Resource.Layout.alarm_display, null);

			alarmRow.Tag = alarmIndex;
			alarmRow.Click += delegate
			{
				Intent editAlarm = new Intent(Application.Context, typeof(MainActivity));
				editAlarm.PutExtra("alarm_number", (int)alarmRow.Tag);
				StartActivity(editAlarm);
			};

			var metrics = Resources.DisplayMetrics;
			alarmRow.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent,
																	  (int)(metrics.HeightPixels * .1));
			TextView alarmTime = alarmRow.FindViewById<TextView>(Resource.Id.alarm_time);
			alarmTime.Typeface = Typeface.CreateFromAsset(Resources.Assets, "HelveticaNeueLight.ttf");
			alarmTime.Text = hour + ":" + minute.ToString("00") + " " + (ampm == 0 ? "am" : "pm");

			View gap = new View(Application.Context);
			LinearLayout.LayoutParams gap_params = new LinearLayout.LayoutParams(
				LinearLayout.LayoutParams.MatchParent, (int)(10 * metrics.Density));
			gap.LayoutParameters = gap_params;
			alarmViewer.AddView(gap);
			alarmViewer.AddView(alarmRow);
		}
	}
}
