using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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


		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
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


		}

		private void initLocationCode()
		{
			locMgr = GetSystemService(Context.LocationService) as LocationManager;

		}

		private List<Address> getAddressesFromLocation(double latitude,double longitude)
		{
			Geocoder geocoder;
			List<Address> addresses;
			geocoder = new Geocoder(this);

			addresses = geocoder.GetFromLocationAsync(latitude,longitude,1);
			return addresses;
		}

		private void updateLocation()
		{
				string Provider = LocationManager.GpsProvider;

			    if(locMgr.IsProviderEnabled(Provider))
			    {
			      locMgr.RequestLocationUpdates (Provider, 0,300000, this);
				}
		   		 else
		   		 {
		      		//Log.Info(tag, Provider + " is not available. Does the device have location services enabled?");
		    	}

		}

		private void addMarkers()
		{
				MarkerOptions markerOpt1 = new MarkerOptions();
				markerOpt1.SetPosition(new LatLng(50.379444, 2.773611));
				markerOpt1.SetTitle("Vimy Ridge");
				map.AddMarker(markerOpt1);
		}
		private void initMap()
		{
			mapFragment = FragmentManager.FindFragmentByTag("map") as MapFragment;
			if (mapFragment == null)
			{
			    GoogleMapOptions mapOptions = new GoogleMapOptions()
					.InvokeMapType(GoogleMap.MapTypeHybrid)
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
			addMarkers();
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
