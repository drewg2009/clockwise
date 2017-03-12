using System;
using Android.Views;
using Android.Content;
using Android.Graphics;
using Android.Widget;
using Clockwise.Helpers;

namespace Clockwise.Droid
{
	public class Weather
	{
		public static void Setup(int index, View v, ImageView addButton, Context c)
		{
			Typeface font = Typeface.CreateFromAsset(c.Resources.Assets, "HelveticaNeueLight.ttf");

			AnimationManager am = new AnimationManager(false);
			AnimationHelper settingsHelper = new AnimationHelper(v, am);
			v.Measure(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.MatchParent);
			v.Measure(RelativeLayout.LayoutParams.MatchParent, RelativeLayout.LayoutParams.MatchParent);
			int expandedHeight = v.MeasuredHeight;

			if (addButton != null)
			{
				addButton.Click += delegate
				{
					if (!am.Animating)
					{
						if (v.LayoutParameters.Height == 0)
						{
							//Expand
							int targetHeight = expandedHeight;
							int duration = (int)(200);
							settingsHelper.expand(duration, targetHeight);
							addButton.SetImageResource(Resource.Drawable.up_icon);
						}
						else {
							//Collapse
							int targetHeight = 0;
							int duration = (int)(200);
							settingsHelper.collapse(duration, targetHeight);
							addButton.SetImageResource(Resource.Drawable.plus);

						}
					}
				};
			}

			TextView t1 = v.FindViewById<TextView>(Resource.Id.fahrenheitText);
			TextView t2 = v.FindViewById<TextView>(Resource.Id.currentTempText);
			TextView t3 = v.FindViewById<TextView>(Resource.Id.maxTempText);
			TextView t4 = v.FindViewById<TextView>(Resource.Id.descriptionText);

			t1.Typeface = font;
			t2.Typeface = font;
			t3.Typeface = font;
			t4.Typeface = font;

			ImageSwitcher s1 = v.FindViewById<ImageSwitcher>(Resource.Id.fahrenheitToggle);
			ImageSwitcher s2 = v.FindViewById<ImageSwitcher>(Resource.Id.currentTempToggle);
			ImageSwitcher s3 = v.FindViewById<ImageSwitcher>(Resource.Id.maxTempToggle);
			ImageSwitcher s4 = v.FindViewById<ImageSwitcher>(Resource.Id.descriptionToggle);

			s1.SetFactory(new Toggle());
			s2.SetFactory(new Toggle());
			s3.SetFactory(new Toggle());
			s4.SetFactory(new Toggle());

			s1.SetImageResource(Resource.Drawable.setting_toggle);
			s2.SetImageResource(Resource.Drawable.setting_toggle);
			s3.SetImageResource(Resource.Drawable.setting_toggle);
			s4.SetImageResource(Resource.Drawable.setting_toggle);


			string weatherSettings = Settings.Weather.Split('|')[index];
			if (weatherSettings != Settings.EMPTY_MODULE)
			{
				string[] settings = weatherSettings.Split(':');
				s1.ScaleX = settings[1] == "1" ? -1 : 1;
				s2.ScaleX = settings[2] == "1" ? -1 : 1;
				s3.ScaleX = settings[3] == "1" ? -1 : 1;
				s4.ScaleX = settings[4] == "1" ? -1 : 1;
			}

			s1.Click += delegate {
				s1.ScaleX *= -1;
			};

			s2.Click += delegate
			{
				s2.ScaleX *= -1;
			};

			s3.Click += delegate
			{
				s3.ScaleX *= -1;
			};

			s4.Click += delegate
			{
				s4.ScaleX *= -1;
			};

			LinearLayout saveButton = v.FindViewById<LinearLayout>(Resource.Id.save_button);
			saveButton.FindViewById<TextView>(Resource.Id.save_text).Typeface = font;
			saveButton.Click += delegate {
				if (s1.ScaleX < 0 || s2.ScaleX < 0 || s3.ScaleX < 0)
				{
					Settings.EditWeather(index,
						(s1.ScaleX < 0),
						(s2.ScaleX < 0),
						(s3.ScaleX < 0),
						(s4.ScaleX < 0));

					//Collapse
					int targetHeight = 0;
					int duration = (int)(200);
					settingsHelper.collapse(duration, targetHeight);
					addButton.SetImageResource(Resource.Drawable.plus);

					Toast.MakeText(c, "Weather module saved.", ToastLength.Short).Show();
				}
				else {
					Toast.MakeText(c, "Please turn on at least one setting", ToastLength.Long).Show();
				}
			};
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
