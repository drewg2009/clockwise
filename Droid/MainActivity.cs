using Android.App;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using System.Linq;
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
			ImageView on_toggle = FindViewById<ImageView>(Resource.Id.on_toggle);
			ImageView off_toggle = FindViewById<ImageView>(Resource.Id.off_toggle);
			off_toggle.Visibility = Android.Views.ViewStates.Gone;

			on_toggle.Click += delegate {
				System.Console.WriteLine("on clicked");

				if (alarmOn)
				{
					System.Console.WriteLine("turning alarm off");
					alarmOn = false;
					on_toggle.Visibility = Android.Views.ViewStates.Gone;
					off_toggle.Visibility = Android.Views.ViewStates.Visible;
				}
			};

			off_toggle.Click += delegate
			{
				System.Console.WriteLine("off clicked");

				if (!alarmOn)
				{
					alarmOn = true;
					System.Console.WriteLine("turning alarm on");
					off_toggle.Visibility = Android.Views.ViewStates.Gone;
					on_toggle.Visibility = Android.Views.ViewStates.Visible;
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
	}
}

