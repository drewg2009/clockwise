using System;
using Android.Content;
using Android.Content.PM;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Java.Lang;

namespace Clockwise.Droid
{
	public class SingleShotLocationProvider
	{
		public delegate void LocationCallback(GPSCoordinates location);
		public interface LocationCallback2
		{
			void onNewLocationAvailable(GPSCoordinates location);
		}

		public static void requestSingleUpdate(Context context, LocationCallback callback)
		{
			LocationManager locationManager = (LocationManager)context.GetSystemService(Context.LocationService);
			bool canGetLocation = false;
			if ((int)Build.VERSION.SdkInt >= 23 && context.CheckSelfPermission(Android.Manifest.Permission.AccessFineLocation) == Permission.Granted
						&& context.CheckSelfPermission(Android.Manifest.Permission.AccessCoarseLocation) == Permission.Granted
			    && (locationManager.IsProviderEnabled(LocationManager.NetworkProvider)
				|| locationManager.IsProviderEnabled(LocationManager.GpsProvider)))
			{
				canGetLocation = true;
			}
			else if (locationManager.IsProviderEnabled(LocationManager.NetworkProvider)
				|| locationManager.IsProviderEnabled(LocationManager.GpsProvider))
			{
				canGetLocation = true;
			}

			if (canGetLocation)
			{
				Console.WriteLine("\nSingleShot using network/gps");
				Criteria criteria = new Criteria();
				string provider;
				if (locationManager.IsProviderEnabled(LocationManager.GpsProvider))
				{
					provider = LocationManager.GpsProvider;
					criteria.Accuracy = Accuracy.Fine;
				}
				else
				{
					provider = LocationManager.NetworkProvider;
					criteria.Accuracy = Accuracy.Coarse;
				}

				try
				{
					//locationManager.RequestSingleUpdate(criteria, new LocationListener(callback, locationManager), null);
					locationManager.RequestLocationUpdates(provider, 0, 2000, new LocationListener(callback, locationManager));
				}
				catch (SecurityException e)
				{
					callback(null);
				}
			}
			else
			{
				callback(null);
			}
		}

		public class LocationListener : Java.Lang.Object, ILocationListener
		{
			LocationCallback callback;
			LocationManager manager;
			public LocationListener(LocationCallback callback, LocationManager lm)
			{
				this.callback = callback;
				manager = lm;
			}
			public void OnLocationChanged(Location location)
			{
				Console.WriteLine("\nCalling callback");
				manager.RemoveUpdates(this);
				callback(new GPSCoordinates(location.Longitude, location.Latitude));
			}

			public void OnProviderDisabled(string provider)
			{
				return;
			}

			public void OnProviderEnabled(string provider)
			{
				return;
			}

			public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
			{
				return;
			}
		}

		public class GPSCoordinates
		{
			public float Longitude = -1;
			public float Latitude = -1;

			public GPSCoordinates(double lon, double lat)
			{
				Longitude = (float)lon;
				Latitude = (float)lat;
		 	}
		}
	}
}
