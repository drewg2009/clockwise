
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Ads;
using Android.Graphics;
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
		private InterstitialAd mInterstitialAd;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.create_module);

			Typeface boldFont = Typeface.CreateFromAsset(Resources.Assets, "HelveticaNeueBold.ttf");
			Typeface lightFont = Typeface.CreateFromAsset(Resources.Assets, "HelveticaNeueLight.ttf");

			//Set fonts
			FindViewById<TextView>(Resource.Id.create_module_title).Typeface = lightFont;
			FindViewById<TextView>(Resource.Id.single).Typeface = boldFont;
			FindViewById<TextView>(Resource.Id.weather_desc).Typeface = lightFont;
			FindViewById<TextView>(Resource.Id.custom).Typeface = boldFont;
			FindViewById<TextView>(Resource.Id.news_desc).Typeface = lightFont;
			FindViewById<TextView>(Resource.Id.reddit_desc).Typeface = lightFont;
			FindViewById<TextView>(Resource.Id.twitter_desc).Typeface = lightFont;
			FindViewById<TextView>(Resource.Id.countdown_desc).Typeface = lightFont;
			FindViewById<TextView>(Resource.Id.traffic_desc).Typeface = lightFont;
			FindViewById<TextView>(Resource.Id.reminders_desc).Typeface = lightFont;
			FindViewById<TextView>(Resource.Id.toggle_text).Typeface = boldFont;
			FindViewById<TextView>(Resource.Id.fact_desc).Typeface = lightFont;
			FindViewById<TextView>(Resource.Id.quote_desc).Typeface = lightFont;
			FindViewById<TextView>(Resource.Id.history_desc).Typeface = lightFont;

			//ImageView cancel_button
			FindViewById<ImageView>(Resource.Id.cancel_button).Click += delegate {
				Finish();
			};

			int alarmIndex = Intent.GetIntExtra("alarm_index", 0);

			//Dropdown setup
			Weather weather = new Weather(Application.Context, alarmIndex, FindViewById<LinearLayout>(Resource.Id.weather_settings));
			weather.CreateSetup(FindViewById<ImageView>(Resource.Id.addWeather));

			News news = new News(ApplicationContext, alarmIndex, FindViewById<LinearLayout>(Resource.Id.news_settings));
			news.CreateSetup(this, FindViewById<ImageView>(Resource.Id.addNews));

			Reddit reddit = new Reddit(ApplicationContext, alarmIndex, FindViewById<LinearLayout>(Resource.Id.reddit_settings));
			reddit.CreateSetup(this, FindViewById<ImageView>(Resource.Id.addReddit));

			Twitter twitter = new Twitter(ApplicationContext, alarmIndex, FindViewById<LinearLayout>(Resource.Id.twitter_settings));
			twitter.CreateSetup(this, FindViewById<ImageView>(Resource.Id.addTwitter));

			Countdown countdown = new Countdown(ApplicationContext, alarmIndex, 
			                                    FindViewById<LinearLayout>(Resource.Id.countdown_settings));
			countdown.CreateSetup(this, FindViewById<ImageView>(Resource.Id.addCountdown));

			Reminders reminders = new Reminders(ApplicationContext, alarmIndex,
			                                    FindViewById<LinearLayout>(Resource.Id.reminders_settings));
			reminders.CreateSetup(this, FindViewById<ImageView>(Resource.Id.addReminders));

			FindViewById(Resource.Id.addTraffic).Click += delegate {
				Intent i = new Intent(ApplicationContext, typeof(TrafficActivity));
                i.PutExtra("alarm_index", alarmIndex);
                StartActivity(i);
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


			if (Settings.GetFact(alarmIndex))
				factToggle.ScaleX = -1;

			if (Settings.GetQuote(alarmIndex))
				quoteToggle.ScaleX = -1;

			if (Settings.GetTDIH(alarmIndex))
				tdihToggle.ScaleX = -1;

			factToggle.Click += delegate
			{
				factToggle.ScaleX *= -1;
				Settings.EditFact(alarmIndex, factToggle.ScaleX < 0);
			};

			quoteToggle.Click += delegate
			{
				quoteToggle.ScaleX *= -1;
				Settings.EditQuote(alarmIndex, quoteToggle.ScaleX < 0);
			};

			tdihToggle.Click += delegate
			{
				tdihToggle.ScaleX *= -1;
				Settings.EditTDIH(alarmIndex, tdihToggle.ScaleX < 0);
			};
		}

        protected override void OnResume()
        {
            base.OnResume();
            AdCheck();
        }

		private void AdCheck()
		{
			mInterstitialAd = new InterstitialAd(this);
			mInterstitialAd.AdUnitId = GetString(Resource.String.interstitial_ad_unit_id);

			if (!mInterstitialAd.IsLoaded)
			{
				RequestNewInterstitial();
			}

			var adListener = new CustomAdlistener();
			mInterstitialAd.AdListener = adListener;
			adListener.AdLoaded += () =>
			{
                if (Helpers.Settings.AdsCreateModule > 2)
				{
					if (mInterstitialAd.IsLoaded)
					{
						mInterstitialAd.Show();
					}

                    Helpers.Settings.AdsCreateModule = 0;
				}
				else
				{
                    Helpers.Settings.AdsCreateModule += 1;
				}
			};


		}

		private void RequestNewInterstitial()
		{
			AdRequest adRequest = new AdRequest.Builder().Build();
			mInterstitialAd.LoadAd(adRequest);
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
