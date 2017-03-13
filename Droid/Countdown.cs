using System;
using Android.App;
using Android.Graphics;
using Android.Text;
using Android.Views;
using Android.Views.InputMethods;

using Android.Widget;
using Clockwise.Helpers;
using Java.Util;

namespace Clockwise.Droid
{
	public class Countdown
	{
		static EditText eventNameInput;
		static EditText eventDateInput;
		static Activity activity;
		public static void Setup(int index, int subindex, View v, ImageView addButton, Activity a)
		{
			activity = a;
			Typeface font = Typeface.CreateFromAsset(activity.ApplicationContext.Resources.Assets, "HelveticaNeueLight.ttf");
			v.FindViewById<TextView>(Resource.Id.eventNameText).Typeface = font;
			v.FindViewById<TextView>(Resource.Id.eventDateInput).Typeface = font;

			eventNameInput = v.FindViewById<EditText>(Resource.Id.eventNameInput);
			eventDateInput = v.FindViewById<EditText>(Resource.Id.eventDateInput);
			eventDateInput.InputType = InputTypes.Null;
			eventDateInput.Click += delegate {
				Calendar calendar = Calendar.Instance;
				calendar.TimeInMillis = Java.Lang.JavaSystem.CurrentTimeMillis();
				DatePickerDialog dateWindow = new DatePickerDialog(activity, (object sender2, DatePickerDialog.DateSetEventArgs e2) =>
				{
					calendar.Set(e2.Year, e2.MonthOfYear, e2.DayOfMonth);
					Date selectedDate = new Date(calendar.Time.Time);
					Date currentDate = new Date(Java.Lang.JavaSystem.CurrentTimeMillis());
					currentDate.Hours = 0;
					currentDate.Minutes = 0;
					currentDate.Seconds = 0;
					currentDate.Time = currentDate.Time + (1000 * 60 * 60 * 24) - 1;

					if (selectedDate.After(currentDate))
						eventDateInput.Text = "" + (e2.MonthOfYear + 1) + "/" + e2.DayOfMonth + "/" + e2.Year;
					else
						Toast.MakeText(activity.ApplicationContext, "Please selected a date after today", ToastLength.Long).Show();

				}, calendar.Get(CalendarField.Year), calendar.Get(CalendarField.Month), calendar.Get(CalendarField.DayOfMonth));
				dateWindow.Window.SetType(WindowManagerTypes.ApplicationPanel);
				//set min date to current date/time
				dateWindow.DatePicker.MinDate = calendar.TimeInMillis - 1000;
				dateWindow.Show();
			};

			if (subindex >= 0)
			{
				string savedModule = Settings.GetCountdown(index, subindex);
				eventNameInput.Text = savedModule.Substring(0, savedModule.IndexOf(':'));
				eventDateInput.Text = savedModule.Substring(savedModule.IndexOf(':') + 1);
			}

			AnimationManager am = new AnimationManager(false);
			AnimationHelper settingsHelper = new AnimationHelper(v, am);
			v.Measure(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.MatchParent);

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
				if (eventNameInput.Text != string.Empty && eventDateInput.Text != string.Empty)
				{
					if (subindex < 0)
						Settings.AddCountdown(index, eventNameInput.Text, eventDateInput.Text);
					else
						Settings.EditCountdown(index, subindex, eventNameInput.Text, eventDateInput.Text);

					//Clear
					if (subindex < 0)
					{
						eventDateInput.Text = string.Empty;
						eventNameInput.Text = string.Empty;
					}
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

					Toast.MakeText(activity.ApplicationContext, "Countdown module saved.", ToastLength.Short).Show();
				}
				else {
					Toast.MakeText(activity.ApplicationContext, "Please fill all fields", ToastLength.Long).Show();
				}
			};
		}

		public class DateClickListener : Java.Lang.Object, View.IOnClickListener
		{
			public void OnClick(View v)
			{
				
			}
		}
	}
}
