using System;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Clockwise.Helpers;

namespace Clockwise.Droid
{
	public class Twitter : Module
	{
		private EditText amountInput;
		private EditText usernameInput;
		private Activity activity;
		public Twitter(Context c, int index, View v, Activity activity) : base(c, index, v)
		{
			this.activity = activity;
			Typeface font = Typeface.CreateFromAsset(activity.ApplicationContext.Resources.Assets, "HelveticaNeueLight.ttf");
			v.FindViewById<TextView>(Resource.Id.usernameText).Typeface = font;
			v.FindViewById<TextView>(Resource.Id.amountText).Typeface = font;

			amountInput = v.FindViewById<EditText>(Resource.Id.amountInput);
			usernameInput = v.FindViewById<EditText>(Resource.Id.usernameInput);

			saveBtn.FindViewById<TextView>(Resource.Id.save_text).Typeface = font;
		}

		public void CreateSetup(ImageView addButton)
		{
			AnimationManager am = new AnimationManager(false);
			AnimationHelper settingsHelper = new AnimationHelper(view, am);
			view.Measure(RelativeLayout.LayoutParams.MatchParent, RelativeLayout.LayoutParams.MatchParent);
			int expandedHeight = view.MeasuredHeight;

			addButton.Click += delegate
			{
				if (!am.Animating)
				{
					if (view.LayoutParameters.Height == 0)
					{
						//Expand
						int targetHeight = expandedHeight;
						int duration = (int)(200);
						settingsHelper.expand(duration, targetHeight);
						addButton.SetImageResource(Resource.Drawable.up_icon);
					}
					else
					{
						//Collapse
						int targetHeight = 0;
						int duration = (int)(200);
						settingsHelper.collapse(duration, targetHeight);
						addButton.SetImageResource(Resource.Drawable.plus);

						//Clear
						usernameInput.Text = string.Empty;
						amountInput.Text = string.Empty;
					}
				}
			};

			saveBtn.Click += delegate
			{
				int result = 0;
				if (int.TryParse(amountInput.Text, out result) &&
					int.Parse(amountInput.Text) > 0 && int.Parse(amountInput.Text) <= 10)
				{
					Settings.AddTwitter(index, usernameInput.Text, int.Parse(amountInput.Text));

					//Collapse
					int targetHeight = 0;
					int duration = (int)(200);
					settingsHelper.collapse(duration, targetHeight);
					addButton.SetImageResource(Resource.Drawable.plus);

					//Clear
					usernameInput.Text = string.Empty;
					amountInput.Text = string.Empty;

					View focus = activity.CurrentFocus;
					if (focus != null)
					{
						InputMethodManager imm = (InputMethodManager)activity.ApplicationContext.GetSystemService("input_method");
						imm.HideSoftInputFromWindow(focus.WindowToken, 0);
					}

					Toast.MakeText(activity.ApplicationContext, "Twitter module saved.", ToastLength.Short).Show();
				}
				else
				{
					Toast.MakeText(activity.ApplicationContext, "Select between 1 and 10 posts.", ToastLength.Long).Show();
				}
			};
		}

		public void EditSetup(int subindex, ImageView settingImage, ImageView navButton)
		{
			string savedModule = Settings.GetReddit(index, subindex);
			usernameInput.Text = savedModule.Substring(0, savedModule.IndexOf(':'));
			amountInput.Text = savedModule.Substring(savedModule.IndexOf(':') + 1);

			saveBtn.Click += delegate
			{
				int result = 0;
				if (int.TryParse(amountInput.Text, out result) &&
					int.Parse(amountInput.Text) > 0 && int.Parse(amountInput.Text) <= 10)
				{
					//Fade editor
					AnimationHelper editorFade = new AnimationHelper(view, null);
					AnimationHelper imageFade = new AnimationHelper(settingImage, null);
					editorFade.Fade(200, 0f);
					imageFade.Fade(200, 1f);
					navButton.SetImageResource(Resource.Drawable.edit_button);

					//Expand clock
					AnimationHelper clockHeight = new AnimationHelper(MainActivity.clock_settings_layout, new AnimationManager(MainActivity.clock_settings_layout.Height > 0));
					clockHeight.expand(200, (int)(MainActivity.DisplayMetrics.HeightPixels * .4));

					Settings.EditTwitter(index, subindex, usernameInput.Text, int.Parse(amountInput.Text));

					View focus = activity.CurrentFocus;
					if (focus != null)
					{
						InputMethodManager imm = (InputMethodManager)activity.ApplicationContext.GetSystemService("input_method");
						imm.HideSoftInputFromWindow(focus.WindowToken, 0);
					}

					Toast.MakeText(activity.ApplicationContext, "Twitter module saved.", ToastLength.Short).Show();
				}
				else
				{
					Toast.MakeText(activity.ApplicationContext, "Select between 1 and 10 posts.", ToastLength.Long).Show();
				}
			};

		}
	}
}
