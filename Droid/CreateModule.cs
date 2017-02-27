
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Clockwise.Droid
{
	[Activity(Label = "CreateModule")]
	public class CreateModule : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.create_module);

			//ImageView cancel_button
			FindViewById<ImageView>(Resource.Id.cancel_button).Click += delegate {
				Finish();
			};
		}
	}
}
