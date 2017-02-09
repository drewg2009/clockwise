using Android.App;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using System.Linq;
using Android.Views;
using Clockwise.Helpers;
namespace Clockwise.Droid
{
	[Activity(Label = "Clockwise", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : Activity
	{
		bool alarmOn = true;
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

			//Setup Toggle(s)
			//ImageView on_toggle = FindViewById<ImageView>(Resource.Id.on_toggle);
			//ImageView off_toggle = FindViewById<ImageView>(Resource.Id.off_toggle);
			//off_toggle.Visibility = Android.Views.ViewStates.Gone;

			//on_toggle.Click += delegate {
			//	System.Console.WriteLine("on clicked");

			//	if (alarmOn)
			//	{
			//		System.Console.WriteLine("turning alarm off");
			//		alarmOn = false;
			//		on_toggle.Visibility = Android.Views.ViewStates.Gone;
			//		off_toggle.Visibility = Android.Views.ViewStates.Visible;
			//	}
			//};

			//off_toggle.Click += delegate
			//{
			//	System.Console.WriteLine("off clicked");

			//	if (!alarmOn)
			//	{
			//		alarmOn = true;
			//		System.Console.WriteLine("turning alarm on");
			//		off_toggle.Visibility = Android.Views.ViewStates.Gone;
			//		on_toggle.Visibility = Android.Views.ViewStates.Visible;
			//	}
			//};

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
					//Turn alarm on
					int hourSet = hourpicker.Value + ampmpicker.Value * 12;
					if (hourSet == 12 && ampmpicker.Value == 0) hourSet = 0;
					if (hourSet == 24) hourSet = 12;
					Settings.AlarmTime = hourSet + ":" + minutepicker.Value;

					alarm_toggle.SetImageResource(Resource.Drawable.on_toggle);
				}
				else {
					//Turn alarm off
					Settings.AlarmTime = string.Empty;
					alarm_toggle.SetImageResource(Resource.Drawable.off_toggle);
				}
			};



		}

		public static void setTime()
		{

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
			                        
	}
}

