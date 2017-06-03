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
	public class Reddit : Module
	{
		private EditText amountInput;
		private EditText subredditInput;
		public Reddit(Context c, int index, View v, TextView tv = null) : base(c, index, v, tv)
		{
			Typeface font = Typeface.CreateFromAsset(c.Resources.Assets, "HelveticaNeueLight.ttf");
			view.FindViewById<TextView>(Resource.Id.subredditText).Typeface = font;
			view.FindViewById<TextView>(Resource.Id.amountText).Typeface = font;
			amountInput = v.FindViewById<EditText>(Resource.Id.amountInput);
			subredditInput = v.FindViewById<EditText>(Resource.Id.subredditInput);
			saveBtn.FindViewById<TextView>(Resource.Id.save_text).Typeface = font;
		}

		public void CreateSetup(Activity activity, ImageView addButton)
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
						subredditInput.Text = string.Empty;
						amountInput.Text = "1";
					}
				}
			};

			saveBtn.Click += delegate
			{
				int result = 0;
				if (int.TryParse(amountInput.Text, out result) &&
					int.Parse(amountInput.Text) > 0 && int.Parse(amountInput.Text) <= 10)
				{
					Settings.AddReddit(index, subredditInput.Text, int.Parse(amountInput.Text));

					addButton.PerformClick();

					View v = activity.CurrentFocus;
					if (v != null)
					{
						InputMethodManager imm = (InputMethodManager)activity.ApplicationContext.GetSystemService("input_method");
						imm.HideSoftInputFromWindow(view.WindowToken, 0);
					}

					Toast.MakeText(context, "Reddit module saved.", ToastLength.Short).Show();
				}
				else
				{
					Toast.MakeText(context, "Select between 1 and 10 posts.", ToastLength.Long).Show();
				}
			};
		}

		public void Reset(int subindex)
		{
			string savedModule = Settings.GetReddit(index, subindex);
			subredditInput.Text = savedModule.Substring(0, savedModule.IndexOf(':'));
			amountInput.Text = savedModule.Substring(savedModule.IndexOf(':') + 1);
		}

		public void EditSetup(int subindex, ImageView navButton)
		{
			Reset(subindex);
			title.Text = "r/" + subredditInput.Text;
			saveBtn.Click += delegate
			{
				int result = 0;
				if (int.TryParse(amountInput.Text, out result) &&
					int.Parse(amountInput.Text) > 0 && int.Parse(amountInput.Text) <= 10)
				{
					navButton.PerformClick();
					Settings.EditReddit(index, subindex, subredditInput.Text, int.Parse(amountInput.Text));
					Toast.MakeText(context, "Reddit module saved.", ToastLength.Short).Show();
				}
				else
				{
					Toast.MakeText(context, "Select between 1 and 10 posts.", ToastLength.Long).Show();
				}
			};

		}
	}
}
