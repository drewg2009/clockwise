using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
        LinearLayout searchFromContainer;
        Button trafficSaveBtn;
        Button trafficCancelButton;
        IList<Address> fromAddresses;
        IList<Address> toAddresses;
        Location currentLocation;
        string provider = "";
        string locationName = "";
        string transportationMethod = "driving";
        string fromLocation = Helpers.Settings.EMPTY_MODULE;
        string toLocation = "";
        int maxResults = 1;
        Marker fromMarker;
        Marker toMarker;
        AlertDialog locationDialog;
        int subIndex;
        Spinner transType;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            fromAddresses = new List<Address>();
            toAddresses = new List<Address>();
            RequestedOrientation = Android.Content.PM.ScreenOrientation.Portrait;
            SetContentView(Resource.Layout.traffic);
            initLocationCode();
            initUI();
            initMap();
            subIndex = Intent.GetIntExtra("subindex", -1);
            if (subIndex != -1)
            {
                LoadExistingTraffic();
            }
        }

        private void LoadExistingTraffic()
        {
            int alarmIndex = Intent.GetIntExtra("alarm_index", -1);
            string trafficString = Helpers.Settings.GetTraffic(alarmIndex, subIndex);
            string[] tripAndRestSplit;

            tripAndRestSplit = trafficString.Split(':');
            locationName = tripAndRestSplit[0];
            fromLocation = tripAndRestSplit[1];
            toLocation = tripAndRestSplit[2];
            transportationMethod = tripAndRestSplit[3];
            //from location exists
            if (!fromLocation.Equals(Helpers.Settings.EMPTY_MODULE))
            {
                searchFromContainer.Visibility = ViewStates.Visible;
                searchFrom.Text = fromLocation;
                toggleDefaultLocationImage.SetImageResource(Resource.Drawable.off_toggle);
            }

            searchTo.Text = toLocation;
            string[] transMethods = Resources.GetStringArray(Resource.Array.transportation_methods);
            int index = 0;
            for (int i = 0; i < transMethods.Length; i++)
            {
                if (transMethods[i].ToLower().Equals(transportationMethod))
                {
                    index = i;
                    break;
                }
            }
            transType.SetSelection(index);
            trafficSaveBtn.Enabled = true;
        }

        private void initUI()
        {
            transType = FindViewById<Spinner>(Resource.Id.transType);
            transType.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);
            var adapter = ArrayAdapter.CreateFromResource(
                    this, Resource.Array.transportation_methods, Android.Resource.Layout.SimpleSpinnerItem);

            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            transType.Adapter = adapter;

            searchFrom = FindViewById<EditText>(Resource.Id.search_from);
            searchFrom.TextChanged += (object sender, Android.Text.TextChangedEventArgs e) =>
            {
                validateLocationInput();
                fromLocation = searchFrom.Text;
            };
            searchTo = FindViewById<EditText>(Resource.Id.search_to);
            searchTo.TextChanged += (object sender, Android.Text.TextChangedEventArgs e) =>
            {
                validateLocationInput();
                toLocation = searchTo.Text;

            };
            trafficSaveBtn = FindViewById<Button>(Resource.Id.trafficSaveButton);
            trafficCancelButton = FindViewById<Button>(Resource.Id.trafficCancelButton);
            toggleDefaultLocationImage = FindViewById<ImageView>(Resource.Id.toggleDefaultLocation);
            searchFromContainer = FindViewById<LinearLayout>(Resource.Id.search_from_container);

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

            toggleDefaultLocationImage.Click += delegate
            {
                toggleDefaultLocationSetting();
            };

            transportationMethod = transType.GetItemAtPosition(0).ToString();

        }

        private void saveSettings()
        {
            int index = Intent.GetIntExtra("alarm_index", -1);
            //if there is no saved module, add new traffic module
            if (subIndex == -1)
            {
                Helpers.Settings.AddTraffic(index, locationName, fromLocation, toLocation, transportationMethod);
            }
            else
            {
                Helpers.Settings.EditTraffic(index, subIndex, locationName, fromLocation, toLocation, transportationMethod);

            }
        }

        private void toggleDefaultLocationSetting()
        {
            if (toggleDefaultLocationImage.Drawable.GetConstantState() == GetDrawable(Resource.Drawable.on_toggle).GetConstantState())
            {
                toggleDefaultLocationImage.SetImageDrawable(GetDrawable(Resource.Drawable.off_toggle));
                searchFromContainer.Visibility = ViewStates.Visible;
            }
            else if (toggleDefaultLocationImage.Drawable.GetConstantState() == GetDrawable(Resource.Drawable.off_toggle).GetConstantState())
            {
                toggleDefaultLocationImage.SetImageDrawable(GetDrawable(Resource.Drawable.on_toggle));
                fromLocation = "";
                searchFrom.Text = "";
                if (fromMarker != null)
                {
                    fromMarker.Remove();
                    fromMarker = null;
                }
                searchFromContainer.Visibility = ViewStates.Gone;
            }
            validateLocationInput();

        }

        private void updateMapToLocation(IList<Address> addresses, bool isFrom, EditText view = null, Vector v = null, Marker marker = null)
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

        protected override void OnResume()
        {
            base.OnResume();
            string Provider = LocationManager.GpsProvider;

            if (locMgr.IsProviderEnabled(Provider))
            {
                locMgr.RequestLocationUpdates(Provider, 2000, 1, this);
            }
            else
            {
                //Log.Info(tag, Provider + " is not available. Does the device have location services enabled?");
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
            locMgr.RemoveUpdates(this);
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
                saveSettings();
                Finish();
            };
            alertBuilder.SetTitle("Set Trip Name");
            locationDialog = alertBuilder.Create();
            locationDialog.Show();
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

        private void validateLocationInput()
        {
            if (toggleDefaultLocationImage.Drawable.GetConstantState() == GetDrawable(Resource.Drawable.on_toggle).GetConstantState())
            {
                trafficSaveBtn.Enabled = toMarker != null && toLocation.Length > 0 && transportationMethod.Length > 0
                    && searchTo.Text.Length > 0;
            }
            else if (toggleDefaultLocationImage.Drawable.GetConstantState() == GetDrawable(Resource.Drawable.off_toggle).GetConstantState())
            {
                trafficSaveBtn.Enabled = toMarker != null && fromMarker != null && toLocation.Length > 0 && transportationMethod.Length > 0
                    && searchTo.Text.Length > 0 && searchFrom.Length() > 0 && fromLocation.Length > 0;

            }
        }

        private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            transportationMethod = spinner.GetItemAtPosition(e.Position).ToString().ToLower();
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            map = googleMap;
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
