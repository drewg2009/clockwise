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
	public class Reminders : Module
	{
		private EditText listTitleInput;
		private List<View> reminderViews;
		private LinearLayout parent;
		private Button addReminderBtn;
		public Reminders(Context c, int index, View v, TextView tv = null) : base(c, index, v, tv)
		{
			//ClearSettings();
			Typeface font = Typeface.CreateFromAsset(c.Resources.Assets, "HelveticaNeueLight.ttf");
			listTitleInput = view.FindViewById<EditText>(Resource.Id.listTitleInput);
			listTitleInput.Typeface = font;

			reminderViews = new List<View>();
			saveBtn.FindViewById<TextView>(Resource.Id.save_text).Typeface = font;
			parent = view.FindViewById<LinearLayout>(Resource.Id.reminderContainer);
			addReminderBtn = view.FindViewById<Button>(Resource.Id.addReminderButton);
			addReminderBtn.Typeface = font;

			//Add button
			addReminderBtn.Click += delegate
			{
				if (listTitleInput.Text != string.Empty)
				{
					if (reminderViews.Count == 0) AddReminderView(null);
					else
					{
						if (reminderViews[reminderViews.Count - 1]
							  .FindViewById<EditText>(Resource.Id.reminderInput).Text != string.Empty)
						{
							if (!reminderViews[reminderViews.Count - 1]
								.FindViewById<EditText>(Resource.Id.reminderInput).Text.Contains(Settings.SEPARATERS))
							{
								View newReminder = AddReminderView(null);
								EditText et = newReminder.FindViewById<EditText>(Resource.Id.reminderInput);
								et.RequestFocus();
							}
							else
							{
								Toast.MakeText(context, "Reminder cannot have special characters.", ToastLength.Short).Show();
							}
						}
						else Toast.MakeText(context, "Please enter a reminder", ToastLength.Short).Show();
					}
				}

				else
					Toast.MakeText(context, "Please give a list title", ToastLength.Long).Show();
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
						ClearSettings();
					}
				}
			};

			saveBtn.Click += delegate
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
					Settings.AddReminders(index, listTitleInput.Text, remindersSB.ToString().TrimEnd(';'));
					Toast.MakeText(context, "Reminders Module Saved", ToastLength.Short).Show();
					View focus = activity.CurrentFocus;
					if (focus != null)
					{
						InputMethodManager imm = (InputMethodManager)activity.ApplicationContext.GetSystemService("input_method");
						imm.HideSoftInputFromWindow(focus.WindowToken, 0);
					}

					addButton.PerformClick();
				}
				else
				{
					Toast.MakeText(context, "Please give a list title and at least 1 reminder.", ToastLength.Long).Show();
				}
			};
		}

		public void Reset(int subindex)
		{
			string savedModule = Settings.GetReminders(index, subindex);
			string[] splitSettings = savedModule.Split(':');
			string listTitle = splitSettings[0];

			listTitleInput.Text = listTitle;
			string[] list = splitSettings[1].Split(';');
			//If building the layout
			for (int i = 0; i<list.Length; i++)
			{
				View v = AddReminderView(list[i]);
			}

			view.LayoutParameters.Height = ViewGroup.LayoutParams.MatchParent;
		}

		public void EditSetup(int subindex, ImageView navButton)
		{
			Reset(subindex);
			saveBtn.Click += delegate
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
					navButton.PerformClick();
					Settings.EditReminders(index, subindex, listTitleInput.Text, remindersSB.ToString());
					Toast.MakeText(context, "Reminders Module Saved", ToastLength.Short).Show();
				}
				else
				{
					Toast.MakeText(context, "Please give a list title and at least 1 reminder.", ToastLength.Long).Show();
				}
			};

		}


		private View AddReminderView(string reminderString)
		{
			View child = View.Inflate(context, Resource.Layout.new_reminder_template, null);
			child.FindViewById<TextView>(Resource.Id.reminderCount).Text = "" + (reminderViews.Count + 1) + ") ";
			//child.FindViewById<EditText>(Resource.Id.reminderInput).RequestFocus();

			reminderViews.Add(child);

			if (reminderString != null)
			{
				child.FindViewById<EditText>(Resource.Id.reminderInput).Text = reminderString;
			}

			int index = reminderViews.Count - 1;

			child.FindViewById<ImageView>(Resource.Id.reminderDeleteButton).Click += delegate {
				RemoveReminderView(index);
			};

			//child.FindViewById<EditText>(Resource.Id.reminderInput).Focusable = false;

			parent.AddView(child);

			RefreshHeight();
			return child;
		}

		private void RemoveReminderView(int id)
		{
			parent.RemoveView(reminderViews[id]);
			reminderViews.RemoveAt(id); //CHECK
			if (reminderViews.Count > 0)
				reminderViews[reminderViews.Count - 1].FindViewById(Resource.Id.reminderInput).RequestFocus();
			parent.Invalidate();
			RefreshHeight();
		}

		private void RefreshHeight()
		{
			//Adjust height
			AnimationManager am = new AnimationManager(false);
			AnimationHelper temp = new AnimationHelper(view, am);
			view.Measure(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
			int x = view.MeasuredHeight;
			temp.expand(10, x);
		}

		public void ClearSettings()
		{
			listTitleInput.Text = string.Empty;

			//Remove extra rows
			foreach(View view1 in reminderViews)
			{
				parent.RemoveView(view1);
			}
			reminderViews.Clear();
		}
	}
}
