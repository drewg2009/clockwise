using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Clockwise.Helpers;
using Java.Lang;

namespace Clockwise.Droid
{
	public class Reminders
	{
		static EditText listTitleInput;
		static List<View> reminderViews;
		static View view;
		static Context context;
		static LinearLayout parent;
		public static void Setup(int index, int subindex, View v, ImageView addButton, Activity activity)
		{
			Typeface font = Typeface.CreateFromAsset(activity.ApplicationContext.Resources.Assets, "HelveticaNeueLight.ttf");
			listTitleInput = v.FindViewById<EditText>(Resource.Id.listTitle);
			listTitleInput.Typeface = font;

			ClearSettings();
			parent = v.FindViewById<LinearLayout>(Resource.Id.reminderContainer);
			view = v;
			context = activity;
			Button addReminderButton = v.FindViewById<Button>(Resource.Id.addReminderButton);
			addReminderButton.Typeface = font;
			listTitleInput = v.FindViewById<EditText>(Resource.Id.listTitle);
			reminderViews = new List<View>();

			if (subindex >= 0)
			{
				string savedModule = Settings.GetReminders(index, subindex);
				string[] splitSettings = savedModule.Split(':');
				string listTitle = splitSettings[0];

				listTitleInput.Text = listTitle;
				//If building the layout
				for (int i = 1; i < splitSettings.Length; i++)
				{
					AddReminderView(splitSettings[i]);
				}
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
							ClearSettings();
						}
					}
				};
			}

			//Add button
			v.FindViewById<Button>(Resource.Id.addReminderButton).Click += delegate {
				if (listTitleInput.Text != string.Empty)
				{
					if (reminderViews.Count == 0) AddReminderView(null);
					else {
						if (reminderViews[reminderViews.Count - 1]
						      .FindViewById<EditText>(Resource.Id.reminderInput).Text != string.Empty)
						{
							if (!reminderViews[reminderViews.Count - 1]
								.FindViewById<EditText>(Resource.Id.reminderInput).Text.Contains(Settings.SEPARATERS))
							{
								View newReminder = AddReminderView(null);
							}
							else {
								Toast.MakeText(context, "Reminder cannot have special characters.", ToastLength.Short).Show();
							}
						}
						else Toast.MakeText(context, "Please enter a reminder", ToastLength.Short).Show();
					}
				}

				else
					Toast.MakeText(context, "Please give a list title", ToastLength.Long).Show();
			};

			LinearLayout saveButton = v.FindViewById<LinearLayout>(Resource.Id.save_button);
			saveButton.FindViewById<TextView>(Resource.Id.save_text).Typeface = font;
			saveButton.Click += delegate
			{
				//build string of reminders to add to shared preferences
				StringBuilder remindersSB = new StringBuilder();
				foreach (View view1 in reminderViews)
				{
					string reminder = view1.FindViewById<EditText>(Resource.Id.reminderInput).Text;
					if (reminder != string.Empty) remindersSB.Append(reminder + ";");
				}

				if (listTitleInput.Text.Length > 0 && reminderViews.Count > 0
				    && (reminderViews[0].FindViewById<EditText>(Resource.Id.reminderInput)).Text.Trim() != string.Empty)
				{
					if (subindex < 0)
						Settings.AddReminders(index, listTitleInput.Text, remindersSB.ToString());
					else
						Settings.EditReminders(index, subindex, listTitleInput.Text, remindersSB.ToString());

					Toast.MakeText(context, "Reminders Module Saved", ToastLength.Short).Show();

					View view2 = activity.CurrentFocus;
					if (view2 != null)
					{
						InputMethodManager imm = (InputMethodManager)activity.ApplicationContext.GetSystemService("input_method");
						imm.HideSoftInputFromWindow(view.WindowToken, 0);
					}

					//Clear


					if (addButton != null)
					{
						//Collapse
						int targetHeight = 0;
						int duration = (int)(200);
						settingsHelper.collapse(duration, targetHeight);
						addButton.SetImageResource(Resource.Drawable.plus);
						ClearSettings();
					}


				}
				else {
					Toast.MakeText(context, "Please give a list title and at least 1 reminder.", ToastLength.Long).Show();
				}
			};
		}

		private static View AddReminderView(string reminderString)
		{
			View child = View.Inflate(context, Resource.Layout.new_reminder_template, null);
			//child.FindViewById<EditText>(Resource.Id.reminderInput).SetHintTextColor(Color.Gray);
			child.FindViewById<TextView>(Resource.Id.reminderCount).Text = "" + (reminderViews.Count + 1) + ") ";

			child.FindViewById<EditText>(Resource.Id.reminderInput).RequestFocus();


			reminderViews.Add(child);

			if (reminderString != null)
			{
				child.FindViewById<EditText>(Resource.Id.reminderInput).Text = reminderString;
			}

			int index = reminderViews.Count - 1;

			child.FindViewById<ImageView>(Resource.Id.reminderDeleteButton).Click += delegate {
				RemoveReminderView(index);
			};

			parent.AddView(child);

			return child;

		}

		private static void RemoveReminderView(int id)
		{
			parent.RemoveView(reminderViews[id]);
			reminderViews.RemoveAt(id); //CHECK
			if (reminderViews.Count > 0)
				reminderViews[reminderViews.Count - 1].FindViewById(Resource.Id.reminderInput).RequestFocus();
			parent.Invalidate();
		}

		public static void ClearSettings()
		{
			if (listTitleInput != null)
			{
				listTitleInput.Text = string.Empty;
				listTitleInput = null;
			}

			if (reminderViews != null)
			{
				if (parent != null)
				{
					//Remove extra rows
					foreach(View view1 in reminderViews)
					{
						parent.RemoveView(view1);
					}
				}
				reminderViews.Clear();
				reminderViews = null;
			}
		}
	}
}
