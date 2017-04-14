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
		SearchView searchFrom;
		SearchView searchTo;
		IList<Address> fromAddresses;
		IList<Address> toAddresses;

		int maxResults = 1;


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
			updateLocation();
		}

		private void initUI()
		{
			Spinner transType = FindViewById<Spinner>(Resource.Id.transType);
			transType.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);
			var adapter = ArrayAdapter.CreateFromResource(
					this, Resource.Array.transportation_methods, Android.Resource.Layout.SimpleSpinnerItem);

			adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
			transType.Adapter = adapter;

			searchFrom = (SearchView)FindViewById(Resource.Id.search_from);
			searchFrom.QueryTextFocusChange += delegate
			{
				updateMapToLocation(fromAddresses, searchFrom);
			};

			searchTo = (SearchView)FindViewById(Resource.Id.search_to);
			searchTo.QueryTextFocusChange += delegate
			{
				updateMapToLocation(toAddresses, searchTo);
				addPath();
			};

		}

		private void updateMapToLocation(IList<Address> addresses, SearchView view)
		{
			var thread = new Thread(new ThreadStart(() =>
			{
				Geocoder geocoder;
				geocoder = new Geocoder(this);
				addresses = geocoder.GetFromLocationName(view.Query, maxResults);
			}));
			thread.Start();

			while (thread.IsAlive)
			{
				//do nothing to wait and add address after addresses have been pulled
			}
			if (addresses.Count > 0)
			{
				addMarker(addresses.ElementAt(0));
			}
		}

		private void initLocationCode()
		{
			locMgr = GetSystemService(Context.LocationService) as LocationManager;

		}



		private void updateLocation()
		{
			string Provider = LocationManager.GpsProvider;

			if (locMgr.IsProviderEnabled(Provider))
			{
				locMgr.RequestLocationUpdates(Provider, 0, 300000, this);
			}
			else
			{
				//Log.Info(tag, Provider + " is not available. Does the device have location services enabled?");
			}

		}

		private void addMarker(Address addr)
		{

			MarkerOptions markerOpt1 = new MarkerOptions();
			markerOpt1.SetPosition(new LatLng(addr.Latitude, addr.Longitude));
			StringBuilder sb = new StringBuilder();

			for (int i = 0; i < addr.MaxAddressLineIndex; i++)
			{
				sb.Append(addr.GetAddressLine(i));
				sb.Append(" ");
			}
			markerOpt1.SetTitle(sb.ToString());
			map.AddMarker(markerOpt1);
			map.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(new LatLng(addr.Latitude, addr.Longitude), 13));

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
		}

		public void OnLocationChanged(Location location)
		{
			throw new NotImplementedException();
		}

		public void OnProviderDisabled(string provider)
		{
			throw new NotImplementedException();
		}

		public void OnProviderEnabled(string provider)
		{
			throw new NotImplementedException();
		}

		public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
		{
			throw new NotImplementedException();
		}
	}
}
