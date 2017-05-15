using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Locations;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Util;

namespace Clockwise.Droid
{


	[Activity(Label = "TrafficActivity")]
	public class TrafficActivity : Activity, IOnMapReadyCallback, ILocationListener
	{
		MapFragment mapFragment;
		GoogleMap map;
		LocationManager locMgr;
		EditText searchFrom;
		EditText searchTo;
		ImageView searchFromBtn;
		ImageView searchToBtn;
        ImageView toggleDefaultLocationImage;
		Button trafficSaveBtn;
		Button trafficCancelButton;
		IList<Address> fromAddresses;
		IList<Address> toAddresses;
		Location currentLocation;
        string provider = "";
        string locationName = "";
        string transportationMethod = "";
		bool loaded = false;
		int maxResults = 1;
		Marker fromMarker;
		Marker toMarker;
		AlertDialog locationDialog;


		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			fromAddresses = new List<Address>();
			toAddresses = new List<Address>();
			this.RequestedOrientation = Android.Content.PM.ScreenOrientation.Portrait;
			SetContentView(Resource.Layout.traffic);
			initUI();
			initLocationCode();
			initMap();
		}

		private void populateLocationOnLoad()
		{
			Geocoder geoCoder = new Geocoder(this);
			Location lastKnown = locMgr.GetLastKnownLocation(provider);

			fromAddresses = geoCoder.GetFromLocation(lastKnown.Latitude, lastKnown.Longitude, maxResults);
			Vector v = new Vector();
			v.Add(lastKnown.Latitude);
			v.Add(lastKnown.Longitude);
			updateMapToLocation(fromAddresses, true, null, v, fromMarker);
			searchFrom.Text = getAddressString(fromAddresses.ElementAt(0));
		}

		private void initUI()
		{
			Spinner transType = FindViewById<Spinner>(Resource.Id.transType);
			transType.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);
			var adapter = ArrayAdapter.CreateFromResource(
					this, Resource.Array.transportation_methods, Android.Resource.Layout.SimpleSpinnerItem);

			adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
			transType.Adapter = adapter;

			searchFrom = FindViewById<EditText>(Resource.Id.search_from);
			searchFrom.TextChanged += (object sender, Android.Text.TextChangedEventArgs e) =>
			{
				validateLocationInput();
			};
			searchTo = FindViewById<EditText>(Resource.Id.search_to);
			searchTo.TextChanged += (object sender, Android.Text.TextChangedEventArgs e) =>
			{
				validateLocationInput();
			};
			trafficSaveBtn = FindViewById<Button>(Resource.Id.trafficSaveButton);
			trafficCancelButton = FindViewById<Button>(Resource.Id.trafficCancelButton);
            toggleDefaultLocationImage = FindViewById<ImageView>(Resource.Id.toggleDefaultLocation);

			searchFromBtn = FindViewById<ImageView>(Resource.Id.search_from_btn);
			searchFromBtn.Click += delegate
			{
				updateMapToLocation(fromAddresses, true, searchFrom, null, fromMarker);
			};

			searchToBtn = FindViewById<ImageView>(Resource.Id.search_to_btn);
			searchToBtn.Click += delegate
			{
				updateMapToLocation(toAddresses, false, searchTo, null, toMarker);
			};

			trafficSaveBtn.Click += delegate
			{
					createLocationNameDialog();
			};

            toggleDefaultLocationImage.Click += delegate {
                toggleDefaultLocationImages();
            };


		}

        private void toggleDefaultLocationImages(){
            if(toggleDefaultLocationImage.Drawable.GetConstantState() == GetDrawable(Resource.Drawable.on_toggle).GetConstantState()){
                toggleDefaultLocationImage.SetImageDrawable(GetDrawable(Resource.Drawable.off_toggle));
            }
            else if(toggleDefaultLocationImage.Drawable.GetConstantState() == GetDrawable(Resource.Drawable.off_toggle).GetConstantState()){
				toggleDefaultLocationImage.SetImageDrawable(GetDrawable(Resource.Drawable.on_toggle));
			}
        }

		private void updateMapToLocation(IList<Address> addresses, bool isFrom, EditText view = null, Vector v = null, Marker marker = null)
		{
			var thread = new Thread(new ThreadStart(() =>
			{
				Geocoder geocoder;
				geocoder = new Geocoder(this);
				if (v == null)
				{
					addresses = geocoder.GetFromLocationName(view.Text, maxResults);
				}
				else
				{
					addresses = geocoder.GetFromLocation((double)v.Get(0), (double)v.Get(1), maxResults);
				}
			}));
			thread.Start();

			while (thread.IsAlive)
			{
				//do nothing to wait and add address after addresses have been pulled
			}
			if (addresses.Count > 0)
			{
				addMarker(addresses.ElementAt(0), isFrom);
			}
			validateLocationInput();
		}

		private void initLocationCode()
		{
			locMgr = GetSystemService(Context.LocationService) as LocationManager;

		}

		private void initLocationUpdate()
		{
			provider = LocationManager.GpsProvider;

			if (locMgr.IsProviderEnabled(provider))
			{
				locMgr.RequestLocationUpdates(provider, 0, 300000, this);
			}
			else
			{
				//Log.Info(tag, Provider + " is not available. Does the device have location services enabled?");
			}

		}

		private void addMarker(Address addr, bool isFrom)
		{

			MarkerOptions markerOptions = new MarkerOptions();
			markerOptions.SetPosition(new LatLng(addr.Latitude, addr.Longitude));
			markerOptions.SetTitle(getAddressString(addr));
			if (isFrom)
			{
				if (fromMarker != null) fromMarker.Remove();
				fromMarker = map.AddMarker(markerOptions);
			}
			else
			{
				if (toMarker != null) toMarker.Remove();
				toMarker = map.AddMarker(markerOptions);
			}
			map.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(new LatLng(addr.Latitude, addr.Longitude), 13));

		}

		private string getAddressString(Address addr)
		{
			StringBuilder sb = new StringBuilder();

			for (int i = 0; i < addr.MaxAddressLineIndex; i++)
			{
				sb.Append(addr.GetAddressLine(i));
				sb.Append(" ");
			}
			return sb.ToString();
		}

		private void createLocationNameDialog()
		{
			var alertBuilder = new AlertDialog.Builder(this);
			View view = LayoutInflater.Inflate(Resource.Layout.traffic_location_dialog, null);
			alertBuilder.SetView(view);
			Button saveLocationBtn = view.FindViewById<Button>(Resource.Id.saveLocationNameBtn);
			EditText locationBox = view.FindViewById<EditText>(Resource.Id.locationName);
			saveLocationBtn.Click += delegate
			{
				locationName = locationBox.Text;
				locationDialog.Dismiss();
			};
			alertBuilder.SetTitle("Set Trip Name");
			locationDialog = alertBuilder.Create();
			locationDialog.Show();
		}

		//private void addPath()
		//{
		//	if (fromAddresses.Count > 0 && toAddresses.Count > 0)
		//	{
		//		Polyline polyline1 = map.AddPolyline(new PolylineOptions()
		//	.Clickable(true)
		//	.Add(
		//			new LatLng(fromAddresses.ElementAt(0).Latitude,
		//					   fromAddresses.ElementAt(0).Longitude),
		//			new LatLng(toAddresses.ElementAt(0).Latitude,
		//					   toAddresses.ElementAt(0).Longitude)

		//												));
		//	}

		//}

		private void initMap()
		{
			mapFragment = FragmentManager.FindFragmentByTag("map") as MapFragment;
			if (mapFragment == null)
			{
				GoogleMapOptions mapOptions = new GoogleMapOptions()
					.InvokeMapType(GoogleMap.MapTypeNormal)
					.InvokeZoomControlsEnabled(false)
					.InvokeCompassEnabled(true);

				Android.App.FragmentTransaction fragTx = FragmentManager.BeginTransaction();
				mapFragment = MapFragment.NewInstance(mapOptions);
				fragTx.Add(Resource.Id.map, mapFragment, "map");
				fragTx.Commit();

			}
			mapFragment.GetMapAsync(this);
		}

		private void validateLocationInput()
		{
			trafficSaveBtn.Enabled = toMarker != null && fromMarker != null 
				&& searchTo.Text.Length > 0 && searchFrom.Text.Length > 0;
		}

		private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
		{
			Spinner spinner = (Spinner)sender;
            transportationMethod = spinner.GetItemAtPosition(e.Position).ToString();
		}

		public void OnMapReady(GoogleMap googleMap)
		{
			map = googleMap;
			initLocationUpdate();
			populateLocationOnLoad();

		}

		public void OnLocationChanged(Location location)
		{
			currentLocation = location;

		}

		public void OnProviderDisabled(string provider)
		{
		}

		public void OnProviderEnabled(string provider)
		{
		}

		public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
		{
		}

	}
}
