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
	public class News
	{
		static Spinner spinner;
		static EditText amountInput;
		public static void Setup(int index, int subindex, View v, ImageView addButton, Activity activity)
		{
			Typeface font = Typeface.CreateFromAsset(activity.ApplicationContext.Resources.Assets, "HelveticaNeueLight.ttf");
			v.FindViewById<TextView>(Resource.Id.categoryText).Typeface = font;
			v.FindViewById<TextView>(Resource.Id.amountText).Typeface = font;

			spinner = v.FindViewById<Spinner>(Resource.Id.newsCategorySpinner);
			spinner.OnItemSelectedListener = null;
			amountInput = v.FindViewById<EditText>(Resource.Id.amountInput);

			if (subindex >= 0)
			{
				string savedModule = Settings.GetNews(index, subindex);
				string numPosts = savedModule.Substring(0, savedModule.IndexOf(':'));
				string category = savedModule.Substring(savedModule.IndexOf(':') + 1);
				string[] categories = activity.ApplicationContext.Resources.GetStringArray(Resource.Array.news_categories_array);
				for (int i = 0; !categories[i].Equals(category); i++)
					spinner.SetSelection(i + 1);

				amountInput.Text = numPosts;
			}

			AnimationManager am = new AnimationManager(false);
			AnimationHelper settingsHelper = new AnimationHelper(v, am);
			v.Measure(RelativeLayout.LayoutParams.MatchParent, RelativeLayout.LayoutParams.MatchParent);
			int expandedHeight = v.MeasuredHeight;

			if (addButton != null)
			{
				addButton.Click += delegate {
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

							//Clear
							spinner.SetSelection(0);
							amountInput.Text = string.Empty;
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
						Settings.AddNews(index, spinner.SelectedItem.ToString(), int.Parse(amountInput.Text));
					else
						Settings.EditNews(index, subindex, spinner.SelectedItem.ToString(), int.Parse(amountInput.Text));

					if (addButton != null)
					{
						//Collapse
						int targetHeight = 0;
						int duration = (int)(200);
						settingsHelper.collapse(duration, targetHeight);
						addButton.SetImageResource(Resource.Drawable.plus);

						//Clear
						spinner.SetSelection(0);
						amountInput.Text = string.Empty;
					}

					View view = activity.CurrentFocus;
					if (view != null)
					{
						InputMethodManager imm = (InputMethodManager)activity.ApplicationContext.GetSystemService("input_method");
						imm.HideSoftInputFromWindow(view.WindowToken, 0);
					}

					Toast.MakeText(activity.ApplicationContext, "News module saved.", ToastLength.Short).Show();
				}
				else {
					Toast.MakeText(activity.ApplicationContext, "Amount must be between 1 and 10 posts.", ToastLength.Long).Show();
				}
			};
		}

		public static void clearSettings(View v)
		{
			amountInput.Text = string.Empty;
			spinner.SetSelection(0);
		}

		public class SpinnerLister : Java.Lang.Object , AdapterView.IOnItemSelectedListener
		{
			public void OnItemSelected(AdapterView av, View v, int i, long l)
			{
				spinner.SetSelection(i);
			}

			public void OnNothingSelected(AdapterView adapterView)
			{

			}
		}
	}
}
