using System;
using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using Clockwise.Helpers;

namespace Clockwise.Droid
{
	public class News
	{
		static Spinner spinner;
		static EditText amountInput;
		public static void Setup(View v, ImageView addButton, Context c, string savedModule)
		{
			Typeface font = Typeface.CreateFromAsset(c.Resources.Assets, "HelveticaNeueLight.ttf");
			v.FindViewById<TextView>(Resource.Id.categoryText).Typeface = font;
			v.FindViewById<TextView>(Resource.Id.amountText).Typeface = font;

			spinner = v.FindViewById<Spinner>(Resource.Id.newsCategorySpinner);
			spinner.OnItemSelectedListener = null;
			amountInput = v.FindViewById<EditText>(Resource.Id.amountInput);

			if (savedModule != string.Empty)
			{
				string numPosts = savedModule.Substring(0, savedModule.IndexOf(':'));
				string category = savedModule.Substring(savedModule.IndexOf(':') + 1);
				string[] categories = c.Resources.GetStringArray(Resource.Array.news_categories_array);
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

						}
					}
				};
			}

			LinearLayout saveButton = v.FindViewById<LinearLayout>(Resource.Id.save_button);
			saveButton.FindViewById<TextView>(Resource.Id.save_text).Typeface = font;
			saveButton.Click += delegate
			{
				string news_modules = Settings.News;
				if (news_modules != string.Empty)
				{
					
				}
				else {
					Settings.News = "news:" + 
				}
				if (addButton != null)
				{
					//Collapse
					int targetHeight = 0;
					int duration = (int)(200);
					settingsHelper.collapse(duration, targetHeight);
					addButton.SetImageResource(Resource.Drawable.plus);
				}

				Toast.MakeText(c, "News module saved.", ToastLength.Short).Show();
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
