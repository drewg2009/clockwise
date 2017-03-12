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
	public class Reddit
	{
		static EditText amountInput;
		static EditText subredditInput;

		public static void Setup(int index, int subindex, View v, ImageView addButton, Activity activity)
		{
			Typeface font = Typeface.CreateFromAsset(activity.ApplicationContext.Resources.Assets, "HelveticaNeueLight.ttf");
			v.FindViewById<TextView>(Resource.Id.subredditText).Typeface = font;
			v.FindViewById<TextView>(Resource.Id.amountText).Typeface = font;

			amountInput = v.FindViewById<EditText>(Resource.Id.amountInput);
			subredditInput = v.FindViewById<EditText>(Resource.Id.subredditInput);

			if (subindex >= 0)
			{
				string savedModule = Settings.GetReddit(index, subindex);
				subredditInput.Text = savedModule.Substring(0, savedModule.IndexOf(':'));
				amountInput.Text = savedModule.Substring(savedModule.IndexOf(':') + 1);
			}

			AnimationManager am = new AnimationManager(false);
			AnimationHelper settingsHelper = new AnimationHelper(v, am);
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

			LinearLayout saveButton = v.FindViewById<LinearLayout>(Resource.Id.save_button);
			saveButton.FindViewById<TextView>(Resource.Id.save_text).Typeface = font;
			saveButton.Click += delegate
			{
				if (int.Parse(amountInput.Text) > 0 && int.Parse(amountInput.Text) <= 10)
				{
					if (subindex < 0)
						Settings.AddReddit(index, subredditInput.Text, int.Parse(amountInput.Text));
					else
						Settings.EditReddit(index, subindex, subredditInput.Text, int.Parse(amountInput.Text));
					//Clear
					subredditInput.Text = string.Empty;
					amountInput.Text = string.Empty;

					if (addButton != null)
					{
						//Collapse
						int targetHeight = 0;
						int duration = (int)(200);
						settingsHelper.collapse(duration, targetHeight);
						addButton.SetImageResource(Resource.Drawable.plus);
					}

					View view = activity.CurrentFocus;
					if (view != null)
					{
						InputMethodManager imm = (InputMethodManager)activity.ApplicationContext.GetSystemService("input_method");
						imm.HideSoftInputFromWindow(view.WindowToken, 0);
					}

					Toast.MakeText(activity.ApplicationContext, "Reddit module saved.", ToastLength.Short).Show();
				}
				else {
					Toast.MakeText(activity.ApplicationContext, "Select between 1 and 10 posts.", ToastLength.Long).Show();
				}
			};
		}

	}
}
