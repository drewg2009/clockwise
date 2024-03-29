﻿using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Text;
using Android.Views;
using Android.Views.InputMethods;

using Android.Widget;
using Clockwise.Helpers;
using Java.Util;

namespace Clockwise.Droid
{
	public class Countdown : Module
	{
		private EditText eventNameInput;
		private EditText eventDateInput;
		public Countdown(Context c, int index, View v, TextView tv = null) : base(c, index, v, tv)
		{
			Typeface font = Typeface.CreateFromAsset(c.Resources.Assets, "HelveticaNeueLight.ttf");
			view.FindViewById<TextView>(Resource.Id.eventDateText).Typeface = font;
			view.FindViewById<TextView>(Resource.Id.eventNameText).Typeface = font;
			eventNameInput = view.FindViewById<EditText>(Resource.Id.eventNameInput);
			eventDateInput = view.FindViewById<EditText>(Resource.Id.eventDateInput);
			saveBtn.FindViewById<TextView>(Resource.Id.save_text).Typeface = font;

			eventDateInput.InputType = InputTypes.Null;
			eventDateInput.Click += delegate
			{
				Calendar calendar = Calendar.Instance;
				calendar.TimeInMillis = Java.Lang.JavaSystem.CurrentTimeMillis();
				int year = calendar.Get(CalendarField.Year);
				int month = calendar.Get(CalendarField.Month);
				int day = calendar.Get(CalendarField.DayOfMonth);

				if (eventDateInput.Text != string.Empty)
				{
					string[] date = eventDateInput.Text.Split('/');
					month = int.Parse(date[0]) - 1;
					day = int.Parse(date[1]);
					year = int.Parse(date[2]);
				}

				//First param may need to be activity
				DatePickerDialog dateWindow = new DatePickerDialog(c, (object sender2, DatePickerDialog.DateSetEventArgs e2) =>
				{
					calendar.Set(e2.Year, e2.Month, e2.DayOfMonth);
					Date selectedDate = new Date(calendar.Time.Time);
					calendar.TimeInMillis = Java.Lang.JavaSystem.CurrentTimeMillis();
					calendar.Set(CalendarField.HourOfDay, 0);
					calendar.Set(CalendarField.Minute, 0);
					calendar.Set(CalendarField.Second, 0);
					Date currentDate = new Date(calendar.Time.Time);
					currentDate.Time = currentDate.Time + (1000 * 60 * 60 * 24) - 1;

					if (selectedDate.After(currentDate))
						eventDateInput.Text = "" + (e2.Month + 1) + "/" + e2.DayOfMonth + "/" + e2.Year;
					else
						Toast.MakeText(c, "Please selected a date after today", ToastLength.Long).Show();

				}, year, month, day);
				dateWindow.Window.SetType(WindowManagerTypes.ApplicationPanel);
				//set min date to current date/time
				dateWindow.DatePicker.MinDate = calendar.TimeInMillis - 1000;
				dateWindow.Show();
			};
		}

		public void CreateSetup(Activity activity, ImageView addButton)
		{
			AnimationManager am = new AnimationManager(false);
			AnimationHelper settingsHelper = new AnimationHelper(view, am);
			view.Measure(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
			int expandedHeight = view.MeasuredHeight;

			addButton.Click += delegate
			{
				if (!am.Animating)
				{
					if (view.LayoutParameters.Height == 0)
					{
						//Expand
						int targetHeight = expandedHeight;
						int duration = 200;
						settingsHelper.expand(duration, targetHeight);
						addButton.SetImageResource(Resource.Drawable.up_icon);
					}
					else
					{
						//Collapse
						int targetHeight = 0;
						int duration = 200;
						settingsHelper.collapse(duration, targetHeight);
						addButton.SetImageResource(Resource.Drawable.plus);
						//Clear
						eventNameInput.Text = string.Empty;
						eventDateInput.Text = string.Empty;
					}
				}
			};

			saveBtn.Click += delegate
			{
				if (eventNameInput.Text != string.Empty && eventDateInput.Text != string.Empty)
				{
					bool success = Settings.AddCountdown(index, eventNameInput.Text, eventDateInput.Text);

					//Collapse
					addButton.PerformClick();

					View focus = activity.CurrentFocus;
					if (focus != null)
					{
						InputMethodManager imm = (InputMethodManager)activity.ApplicationContext.GetSystemService("input_method");
						imm.HideSoftInputFromWindow(focus.WindowToken, 0);
					}
					if(success)
						Toast.MakeText(activity.ApplicationContext, "Countdown module saved.", ToastLength.Short).Show();
					else
						Toast.MakeText(activity.ApplicationContext, "Countdown module already exists.", ToastLength.Short).Show();
				}
				else
				{
					Toast.MakeText(activity.ApplicationContext, "Please fill all fields", ToastLength.Long).Show();
				}
			};
		}

		public void Reset(int subindex)
		{
			string[] savedModuleSettings = Settings.GetCountdown(index, subindex).Split(':');
			eventNameInput.Text = savedModuleSettings[0];
			eventDateInput.Text = savedModuleSettings[1];	
		}

		public void EditSetup(int subindex, ImageView navButton)
		{
			Reset(subindex);

			saveBtn.Click += delegate
			{
				if (eventNameInput.Text != string.Empty && eventDateInput.Text != string.Empty)
				{
					navButton.PerformClick();
					bool success = Settings.EditCountdown(index, subindex, eventNameInput.Text, eventDateInput.Text);
					if(success)
						Toast.MakeText(context, "Countdown module saved.", ToastLength.Short).Show();
					else
						Toast.MakeText(context, "Countdown module already exists.", ToastLength.Short).Show();
				}
				else
				{
					Toast.MakeText(context, "Please fill all fields", ToastLength.Long).Show();
				}
			};
		}
	}
}
