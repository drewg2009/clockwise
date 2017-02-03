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
			hourpicker.SetDisplayedValues(Enumerable.Range(1, 12).Select(n => n.ToString()).ToArray());
			hourpicker.MinValue = 1;
			hourpicker.MaxValue = 12;
			//hourpicker.Value = 3;
			hourpicker.WrapSelectorWheel = true;
			minutepicker.SetDisplayedValues(Enumerable.Range(0, 59).Select(n => n.ToString()).ToArray());
			//ampmpicker.MaxValue = 1;
			//ampmpicker.MinValue = 0;
			ampmpicker.SetDisplayedValues(new string[] { "am", "pm" });

			//Setup Toggle(s)
			ImageView on_toggle = FindViewById<ImageView>(Resource.Id.on_toggle);
			ImageView off_toggle = FindViewById<ImageView>(Resource.Id.off_toggle);


			//button.Click += delegate { button.Text = string.Format("{0} clicks!", count++); };
		}
	}
}

