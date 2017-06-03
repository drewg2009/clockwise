using System;
using System.Collections.Generic;
using System.Text;
using Android.App;
using Android.Content;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Location;
using Android.Graphics;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Clockwise.Helpers;
using Geolocator.Plugin;


namespace Clockwise.Droid
{
    public class NewTraffic : Module
    {
        private EditText startUrl;
        private EditText destUrl;
        private EditText tripName;
        private ImageView currentLocationToggle;
        private Spinner travelModeSpinner;
        private RelativeLayout fromLocationContainer;
        string travelModeString = "driving";
        bool useCurrentLocation;
        GoogleApiClient apiClient;
        LocationRequest locRequest;
        Location location;
        bool _isGooglePlayServicesInstalled;

		public NewTraffic(Context c, int index, View v, TextView tv = null) : base(c, index, v, tv)
        {
            
            Typeface font = Typeface.CreateFromAsset(c.Resources.Assets, "HelveticaNeueLight.ttf");
            view.FindViewById<TextView>(Resource.Id.currentLocLabel).Typeface = font;
            view.FindViewById<TextView>(Resource.Id.fromLocLabel).Typeface = font;
            view.FindViewById<TextView>(Resource.Id.destLocLabel).Typeface = font;
            view.FindViewById<EditText>(Resource.Id.fromLocationInput).Typeface = font;
            view.FindViewById<TextView>(Resource.Id.tripNameLabel).Typeface = font;
			view.FindViewById<EditText>(Resource.Id.tripNameInput).Typeface = font;
			view.FindViewById<EditText>(Resource.Id.destinationLocationInput).Typeface = font;
            view.FindViewById<TextView>(Resource.Id.travelModeLabel).Typeface = font;

			startUrl = v.FindViewById<EditText>(Resource.Id.fromLocationInput);
            destUrl = v.FindViewById<EditText>(Resource.Id.destinationLocationInput);
            tripName = v.FindViewById<EditText>(Resource.Id.tripNameInput);

			currentLocationToggle = v.FindViewById<ImageView>(Resource.Id.locationToggleBtn);
            travelModeSpinner = v.FindViewById<Spinner>(Resource.Id.travelModeSpinner);
            var adapter = ArrayAdapter.CreateFromResource(
                c, Resource.Array.transportation_methods, Android.Resource.Layout.SimpleSpinnerItem);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            travelModeSpinner.Adapter = adapter;
            travelModeSpinner.ItemSelected += (object sender, AdapterView.ItemSelectedEventArgs e) =>
            {
                Spinner spinner = (Spinner)sender;
                travelModeString = spinner.GetItemAtPosition(e.Position).ToString().ToLower();
            };
            saveBtn.FindViewById<TextView>(Resource.Id.save_text).Typeface = font;
			GetLocation();


		}


        private async void GetLocation(){
            var locator = CrossGeolocator.Current;
			locator.DesiredAccuracy = 50;
			var position = await locator.GetPositionAsync(timeoutMilliseconds: 10000);

			if (position == null)
			{
                Toast.MakeText(context, "GPS not currently working. Please try again later", ToastLength.Long).Show();
			}

			try
			{
                Geocoder geocoder = new Geocoder(context);
                var addresses = await geocoder.GetFromLocationAsync(position.Latitude, position.Longitude, 1);
				if (addresses == null)
					Console.WriteLine("No address found for position.");
                else{
                    startUrl.Text = GetAddressString(addresses[0]);
				}

			}
			catch (Exception ex)
			{
                string error = ex.ToString();
				Toast.MakeText(context, "Could Not Retrieve Address. Please try again later", ToastLength.Long).Show();
			}
		}

		private string GetAddressString(Address addr)
		{
			StringBuilder sb = new StringBuilder();

			for (int i = 0; i < addr.MaxAddressLineIndex; i++)
			{
				sb.Append(addr.GetAddressLine(i));
				sb.Append(" ");
			}
			return sb.ToString();
		}

        public void CreateSetup(Activity activity, ImageView addButton)
        {
            AnimationManager am = new AnimationManager(false);
            AnimationHelper settingsHelper = new AnimationHelper(view, am);
            view.Measure(RelativeLayout.LayoutParams.MatchParent, RelativeLayout.LayoutParams.MatchParent);
            int expandedHeight = view.MeasuredHeight;

            addButton.Click += delegate
            {
                if (!am.Animating)
                {
                    if (view.LayoutParameters.Height == 0)
                    {
                        //Expand
                        int targetHeight = expandedHeight;
                        int duration = (int)(200);
                        settingsHelper.expand(duration, targetHeight);
                        addButton.SetImageResource(Resource.Drawable.up_icon);
                    }
                    else
                    {
                        //Collapse
                        int targetHeight = 0;
                        int duration = (int)(200);
                        settingsHelper.collapse(duration, targetHeight);
                        addButton.SetImageResource(Resource.Drawable.plus);
                        //Clear
                        startUrl.Text = string.Empty;
                        destUrl.Text = string.Empty;
                        currentLocationToggle.SetImageResource(Resource.Drawable.off_toggle);
                    }
                }
            };

            saveBtn.Click += delegate
            {
                if (destUrl.Length() > 0)
                {
                    //startUrl can be blank
                    if (useCurrentLocation)
                    {
                        SaveModule(addButton, activity, tripName.Text, startUrl.Text, destUrl.Text, travelModeString);
                    }
                    else
                    {
                        if (startUrl.Text.Length == 0)
                        {
							Toast.MakeText(context, "Please enter a from location.", ToastLength.Long).Show();
                                             
                        }
                        else if(tripName.Text.Length == 0){
							Toast.MakeText(context, "Please enter a trip name.", ToastLength.Long).Show();
						}
                        else
                        {
							SaveModule(addButton, activity, tripName.Text, startUrl.Text, destUrl.Text, travelModeString);
						}
                    }

                }
                else
                {
                    Toast.MakeText(context, "Please enter a destination address", ToastLength.Long).Show();
                }
            };
        }

        private void SaveModule(ImageView addButton, Activity activity, string trip, string fromDestination, string toDestination, string mode)
        {
            Settings.AddTraffic(index, trip, fromDestination, toDestination, mode);
            addButton.PerformClick();

            View v = activity.CurrentFocus;
            if (v != null)
            {
                InputMethodManager imm = (InputMethodManager)activity.ApplicationContext.GetSystemService("input_method");
                imm.HideSoftInputFromWindow(view.WindowToken, 0);
            }

            Toast.MakeText(context, "Traffic module saved.", ToastLength.Short).Show();
        }

        private void EditModule(ImageView navButton, int subindex, string trip, string fromDestination, string toDestination, string mode)
        {
            navButton.PerformClick();
            Settings.EditTraffic(index, subindex, trip, fromDestination, toDestination, mode);
            Toast.MakeText(context, "Traffic module saved.", ToastLength.Short).Show();
        }



        public void EditSetup(int subindex, ImageView navButton)
        {
            string savedModule = Settings.GetTraffic(index, subindex);
            string[] tripAndRestSplit;
            tripAndRestSplit = savedModule.Split(':');
            string locationName = tripAndRestSplit[0];
            string fromLocation = tripAndRestSplit[1];
            string toLocation = tripAndRestSplit[2];
            string transportationMethod = tripAndRestSplit[3];

            //from location exists
            if (!fromLocation.Equals(Helpers.Settings.EMPTY_MODULE))
            {
                fromLocationContainer.Visibility = ViewStates.Visible;
                startUrl.Text = fromLocation;
                currentLocationToggle.SetImageResource(Resource.Drawable.off_toggle);
            }

            string[] transMethods = context.Resources.GetStringArray(Resource.Array.transportation_methods);
            int transIndex = 0;
            for (int i = 0; i < transMethods.Length; i++)
            {
                if (transMethods[i].ToLower().Equals(transportationMethod))
                {
                    transIndex = i;
                    break;
                }
            }
            travelModeSpinner.SetSelection(transIndex);

            title.Text = locationName;
            saveBtn.Click += delegate
            {

                if (destUrl.Length() > 0)
                {
                    //startUrl can be blank
                    if (useCurrentLocation)
                    {
                        EditModule(navButton, subindex, locationName, fromLocation, toLocation, transportationMethod);
                    }
                    else
                    {
                        if (startUrl.Text.Length > 0)
                        {
                            EditModule(navButton, subindex, locationName, fromLocation, toLocation, transportationMethod);
                        }
                        else
                        {
                            Toast.MakeText(context, "Please enter a from location.", ToastLength.Long).Show();
                        }
                    }

                }
                else
                {
                    Toast.MakeText(context, "Please enter a destination address", ToastLength.Long).Show();
                }
            };


        }

    }
}
