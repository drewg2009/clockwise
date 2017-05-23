
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
			RelativeLayout moduleList = FindViewById<RelativeLayout>(Resource.Id.module_list);
			int selectionRegion = (int)(metrics.HeightPixels * .95);
			int index = Intent.GetIntExtra("alarm_index", -1);
			string[] moduleOrder = Settings.ModuleOrder.Split('|')[index].Split(':');
			int moduleRowHeight = selectionRegion / 11;
			Console.WriteLine("moduleRow: " + moduleRowHeight);
			for (int i = 0; i < moduleList.ChildCount; i++)
			{
				RelativeLayout rl = (RelativeLayout)moduleList.GetChildAt(i);
				rl.FindViewById<TextView>(Resource.Id.module_title).Text = moduleOrder[i];
				rl.LayoutParameters.Height = moduleRowHeight;
				RelativeLayout.LayoutParams lp = (RelativeLayout.LayoutParams)rl.LayoutParameters;
				lp.SetMargins(0, i* (selectionRegion/10), 0, 0);
				//switch (moduleOrder[i])
				//{

				//}
				rows.Add(rl);
			}
			LinearLayout root = FindViewById<LinearLayout>(Resource.Id.module_order_root);

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
							handler.PostDelayed(delegate {
								if (isTouching)
								{
									Console.WriteLine("long press");
									if (y < selectionRegion)
									{
										//Get selection
										//selectionIndex = 
									}
								}
							}, 500);
						}
			        	break;
					case MotionEventActions.Move:
						if (e.Event.GetPointerId(e.Event.ActionIndex) == 0)
						{
							Console.WriteLine("moving: " + e.Event.GetY());
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
