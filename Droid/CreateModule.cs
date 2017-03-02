
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
using Clockwise.Helpers;

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

			//Toggle setup
			ImageSwitcher factToggle = FindViewById<ImageSwitcher>(Resource.Id.factToggle);
			ImageSwitcher quoteToggle = FindViewById<ImageSwitcher>(Resource.Id.quoteToggle);
			ImageSwitcher tdihToggle = FindViewById<ImageSwitcher>(Resource.Id.tdihToggle);

			factToggle.SetFactory(new Toggle());
			quoteToggle.SetFactory(new Toggle());
			tdihToggle.SetFactory(new Toggle());

			factToggle.SetImageResource(Resource.Drawable.setting_toggle);
			quoteToggle.SetImageResource(Resource.Drawable.setting_toggle);
			tdihToggle.SetImageResource(Resource.Drawable.setting_toggle);


			if (Settings.Fact != string.Empty)
				factToggle.ScaleX = -1;

			if (Settings.Quote != string.Empty)
				quoteToggle.ScaleX = -1;

			if (Settings.TDIH != string.Empty)
				tdihToggle.ScaleX = -1;

			factToggle.Click += delegate
			{
				Settings.Fact = Settings.Fact == string.Empty ? "fact:" : string.Empty;
				factToggle.ScaleX = Settings.Fact == string.Empty ? 1 : -1;
			};

			quoteToggle.Click += delegate
			{
				Settings.Quote = Settings.Quote == string.Empty ? "quote:" : string.Empty;
				quoteToggle.ScaleX = Settings.Quote == string.Empty ? 1 : -1;
			};

			tdihToggle.Click += delegate
			{
				Settings.TDIH = Settings.TDIH == string.Empty ? "tdih:" : string.Empty;
				tdihToggle.ScaleX = Settings.TDIH == string.Empty ? 1 : -1;
			};
		}
	}

	public class Toggle : Java.Lang.Object, ViewSwitcher.IViewFactory
	{
		public View MakeView()
		{
			ImageView myView = new ImageView(Android.App.Application.Context);
			return myView;
		}

	}
}
