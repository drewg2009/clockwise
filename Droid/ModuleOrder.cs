
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using Clockwise.Helpers;
using Java.Lang;

namespace Clockwise.Droid
{
	[Activity(Label = "ModuleOrder")]
	public class ModuleOrder : Activity//, View.IOnTouchListener//, View.IOnLongClickListener
	{
		bool isTouching = false;
		long touchTime = 0;
		float originalY = 0;
		List<RelativeLayout> rows = new List<RelativeLayout>();
		int selectionIndex = -1;
		DisplayMetrics metrics;
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.module_order);
			metrics = Resources.DisplayMetrics;
			LinearLayout root = FindViewById<LinearLayout>(Resource.Id.module_order_root);
			root.Measure(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
			Console.WriteLine("root height: {0}", root.MeasuredHeight);
			RelativeLayout moduleList = FindViewById<RelativeLayout>(Resource.Id.module_list);

			Console.WriteLine("screen height: {0}", metrics.HeightPixels);
			RelativeLayout saveButton = FindViewById<RelativeLayout>(Resource.Id.save_button);
			saveButton.Measure(ViewGroup.LayoutParams.MatchParent , ViewGroup.LayoutParams.WrapContent);
			int selectionRegion = root.MeasuredHeight - saveButton.MeasuredHeight;
			Console.WriteLine("selectionRegion: {0}", selectionRegion);

			Console.WriteLine("save height: {0}", saveButton.MeasuredHeight);
			int index = Intent.GetIntExtra("alarm_index", -1);
			string[] moduleOrder = Settings.ModuleOrder.Split('|')[index].Split(':');
			int moduleRowHeight = selectionRegion / moduleOrder.Length;
			Console.WriteLine("moduleRow: " + moduleRowHeight);
			int statusBarHeight = metrics.HeightPixels - root.MeasuredHeight;
			for (int i = 0; i < moduleList.ChildCount; i++)
			{
				RelativeLayout rl = (RelativeLayout)moduleList.GetChildAt(i);
				rl.FindViewById<TextView>(Resource.Id.module_title).Text = moduleOrder[i];
				rl.LayoutParameters.Height = (int)(moduleRowHeight * .95);
				RelativeLayout.LayoutParams lp = (RelativeLayout.LayoutParams)rl.LayoutParameters;
				/*
					0 -> moduleHeight*.05
					1 -> moduleHeight + moduleHeight*.05
					2 -> 2*moduleHeight + moduleHeight*.05
				*/
				lp.SetMargins(0, (i * moduleRowHeight) + (int)(moduleRowHeight * .05), 0, 0);
				ImageView iv = rl.FindViewById<ImageView>(Resource.Id.module_icon);
				iv.LayoutParameters.Width = (int)(moduleRowHeight * .5);
				iv.LayoutParameters.Height = (int)(moduleRowHeight * .5);
				switch (moduleOrder[i])
				{
					case "weather":
						iv.SetImageResource(Resource.Drawable.weather_icon);
						break;
					case "news":
						iv.SetImageResource(Resource.Drawable.news_icon);
						break;
					case "reddit":
						iv.SetImageResource(Resource.Drawable.reddit_icon);
						break;
					case "twitter":
						iv.SetImageResource(Resource.Drawable.twitter_icon);
						break;
					case "traffic":
						iv.SetImageResource(Resource.Drawable.traffic_icon);
						break;
					case "reminders":
						iv.SetImageResource(Resource.Drawable.todo_icon);
						break;
					case "countdown":
						iv.SetImageResource(Resource.Drawable.countdown_icon);
						break;
					case "fact":
						iv.SetImageResource(Resource.Drawable.fact_icon);
						break;
					case "quote":
						iv.SetImageResource(Resource.Drawable.quote_icon);
						break;
					case "tdih":
						iv.SetImageResource(Resource.Drawable.tdih_icon);
						break;
				}
				rows.Add(rl);
			}


			root.Touch += (object sender, View.TouchEventArgs e) =>
			{
				float y = e.Event.GetY();
				switch (e.Event.Action)
				{
					case MotionEventActions.Down:

						if (e.Event.GetPointerId(e.Event.ActionIndex) == 0)
						{
							touchTime = JavaSystem.CurrentTimeMillis();
							originalY = e.Event.GetY();
							isTouching = true;
							Console.WriteLine("down: " + touchTime);
							Handler handler = new Handler();
							handler.PostDelayed(delegate
							{
								if (isTouching)
								{

									if (y < selectionRegion)
									{
										//Get selection
										y -= statusBarHeight;
										y += (int)(moduleRowHeight * .5);
										selectionIndex = (int)(y / (float)selectionRegion * moduleOrder.Length);
										Console.WriteLine("long press at {0}", selectionIndex);
									}
								}
							}, 500);
						}
						break;
					case MotionEventActions.Move:
						if (e.Event.GetPointerId(e.Event.ActionIndex) == 0)
						{
							Console.WriteLine("moving: " + y);
						}
						break;
					case MotionEventActions.Up:
						if (e.Event.GetPointerId(e.Event.ActionIndex) == 0)
						{
							Console.WriteLine("up");
							originalY = touchTime = 0;
							isTouching = false;
						}
						break;
				}
			};
		}
	}
}
