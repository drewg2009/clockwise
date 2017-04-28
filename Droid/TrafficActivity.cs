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
		Button searchFromBtn;
		IList<Address> fromAddresses;
		IList<Address> toAddresses;
		Location currentLocation;
		string provider;
		bool loaded = false;
		int maxResults = 1;
		Marker fromMarker;
		Marker toMarker;

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


			//createLocationNameDialog("news");
		}

		private void populateLocationOnLoad()
		{
			Geocoder geoCoder = new Geocoder(this);
			Location lastKnown = locMgr.GetLastKnownLocation(provider);

			fromAddresses = geoCoder.GetFromLocation(lastKnown.Latitude, lastKnown.Longitude, maxResults);
			Vector v = new Vector();
			v.Add(lastKnown.Latitude);
			v.Add(lastKnown.Longitude);
			updateMapToLocation(fromAddresses,true, null, v, fromMarker);
			searchFrom.Text= getAddressString(fromAddresses.ElementAt(0));
		}

		private void initUI()
		{
			Spinner transType = FindViewById<Spinner>(Resource.Id.transType);
			transType.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);
			var adapter = ArrayAdapter.CreateFromResource(
					this, Resource.Array.transportation_methods, Android.Resource.Layout.SimpleSpinnerItem);

			adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
			transType.Adapter = adapter;

			searchFromBtn = FindViewById<Button>(Resource.Id.search_from_btn);
			searchFromBtn.Click += delegate {
              updateMapToLocation(fromAddresses,true, searchFrom, null, fromMarker);
			};


			searchFrom = (EditText)FindViewById(Resource.Id.search_from);
			//searchFrom.AfterTextChanged += delegate
			//{

			//};

			searchTo = (EditText)FindViewById(Resource.Id.search_to);
			//searchTo.Focus += delegate
			//{
			//	updateMapToLocation(toAddresses, searchTo, null, toMarker);
			//	addPath();
			//};

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
				addMarker(addresses.ElementAt(0),isFrom);
			}
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

		private void addMarker(Address addr, bool isFrom){

			MarkerOptions markerOptions = new MarkerOptions();
			markerOptions.SetPosition(new LatLng(addr.Latitude, addr.Longitude));
			markerOptions.SetTitle(getAddressString(addr));
			if (isFrom)
			{
				if(fromMarker != null) fromMarker.Remove();
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

		private void createLocationNameDialog(string title)
		{
			//set alert for executing the task
			AlertDialog.Builder alert = new AlertDialog.Builder(this);


			View view = LayoutInflater.Inflate(Resource.Layout.traffic_location_dialog, (ViewGroup)Window.DecorView.RootView);

			alert.SetTitle(title);

			alert.SetView(view);

			//run the alert in UI thread to display in the screen
			RunOnUiThread(() =>
			{
				alert.Show();
			});
		}

		private void addPath()
		{
			if (fromAddresses.Count > 0 && toAddresses.Count > 0)
			{
				Polyline polyline1 = map.AddPolyline(new PolylineOptions()
			.Clickable(true)
			.Add(
					new LatLng(fromAddresses.ElementAt(0).Latitude,
							   fromAddresses.ElementAt(0).Longitude),
					new LatLng(toAddresses.ElementAt(0).Latitude,
							   toAddresses.ElementAt(0).Longitude)

														));
			}

		}

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

		private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
		{
			Spinner spinner = (Spinner)sender;

			//string toast = string.Format("The planet is {0}", spinner.GetItemAtPosition(e.Position));
			//Toast.MakeText(this, toast, ToastLength.Long).Show();
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
