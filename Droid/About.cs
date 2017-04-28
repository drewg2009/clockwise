
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Clockwise.Droid
{
	[Activity(Label = "About")]
	public class About : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.about);

			Typeface fontLight = Typeface.CreateFromAsset(Resources.Assets, "HelveticaNeueLight.ttf");
			Typeface fontBold = Typeface.CreateFromAsset(Resources.Assets, "HelveticaNeueBold.ttf");

			LinearLayout scrollLayout = FindViewById<LinearLayout>(Resource.Id.about_scroll_layout);
			for (int i = 0; i < scrollLayout.ChildCount; i++)
			{
				TextView tv = (TextView)scrollLayout.GetChildAt(i);
				tv.Typeface = fontLight;
			}

			FindViewById(Resource.Id.cancel_button).Click += delegate {
				Finish();
			};
		}
	}
}
