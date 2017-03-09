
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
	[Activity(Label = "ManageAlarms")]
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
				AlarmUtils.SetTime(Android.App.Application.Context, hourSet, minutepicker.Value, currentAlarms.Length);

				//Add view
				LinearLayout alarmViwer = (LinearLayout)FindViewById(Resource.Id.alarm_viewer);

				RelativeLayout alarmRow = (RelativeLayout)LayoutInflater.Inflate(Resource.Layout.alarm_display, null);

				View gap = new View(Application.Context);
				LinearLayout.LayoutParams gap_params = new LinearLayout.LayoutParams(
					LinearLayout.LayoutParams.MatchParent, 10);
				gap.LayoutParameters = gap_params;
				alarmViwer.AddView(gap);
				alarmViwer.AddView(alarmRow);

			};
		}
	}
}
