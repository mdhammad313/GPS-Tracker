using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Locations;
using Android;


using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Android.Telephony;

namespace GPS
{
    [Activity(Label = "GPS", MainLauncher = true, Icon = "@drawable/icon")]
    public class GelLocation : Activity , ILocationListener
    {

        Coordinates latlon = new Coordinates();
        Location _currentLocation;
        LocationManager _locationManager;
        TextView _locationText;
        TextView _addressText;
        string _locationProvider;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.GetLocation);
            _locationText = FindViewById<TextView>(Resource.Id.textView1);
            _addressText = FindViewById<TextView>(Resource.Id.textView2);
            FindViewById<TextView>(Resource.Id.button1).Click += GelLocation_Click;

            InitializeLocationManager();
            getDeviceId();
        }

         protected override void OnStart()
        {
            base.OnStart();

            StartService(new Intent(this, typeof(BackgroundService)));
        }

        private void getDeviceId()
        {
            try
            {
                TelephonyManager manager = (TelephonyManager)GetSystemService(Context.TelephonyService);
                latlon.DeviceId = manager.DeviceId;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        /*The LocationManager class will listen for GPS updates from the device and notify the application by way of events.
          In this example we ask Android for the best location provider that matches a given set of Criteria and provide that 
          provider to LocationManager.*/
        private void InitializeLocationManager()
        {
            _locationManager = (LocationManager)GetSystemService(LocationService);
            //Application criteria for selecting provider
            Criteria criteriaForLocationService = new Criteria{ Accuracy = Accuracy.NoRequirement };
            IList<string> acceptableLocationProviders = _locationManager.GetProviders(criteriaForLocationService, true);

            if (acceptableLocationProviders.Any())
            {
                _locationProvider = acceptableLocationProviders.First();
            }
            else
            {
                _locationProvider = String.Empty;
            }
        }

        /*Button Event
        This method uses the Geocoder API to retrieve a street address for the current location.
        The Geocoder class retrieves a list of address from Google over the internet.Network calls are expensive, 
        so the call is made asynchronously using the async/ await keywords*/
         async private void GelLocation_Click(object sender, EventArgs e)
        {
            if (_currentLocation == null)
            {
                _addressText.Text = "Can't determine the current address.";
                return;
            }

            Geocoder geocoder = new Geocoder(this);
            IList<Address> addressList = await geocoder.GetFromLocationAsync(_currentLocation.Latitude, _currentLocation.Longitude, 10);
            Address address;
            List<string> allLocations = new List<string>();


            for (int j = 0; j < addressList.Count; j++)
            {
                address = addressList.ElementAt(j);

                if (address != null)
                {
                    StringBuilder deviceAddress = new StringBuilder();
                    for (int i = 0; i <= address.MaxAddressLineIndex; i++)
                    {
                        deviceAddress.Append(address.GetAddressLine(i)).AppendLine(",");
                    }
                    _addressText.Text = deviceAddress.ToString();
                }
                else
                {
                    _addressText.Text = "Unable to determine the address.";
                }

                allLocations.Add(_addressText.Text);
            }
            _addressText.Text = string.Join("\n", allLocations);
        }

        #region InterfaceMethods

        //Call according to the critweria we set
         public void OnLocationChanged(Location location)
        {
            _currentLocation = location;
            
            if (_currentLocation == null)
            {
                _locationText.Text = "Unable to determine your location.";
            }
            else
            {
                _locationText.Text = String.Format("Latitude: {0} \nLongitude: {1} \nAccuracy: {2}", _currentLocation.Latitude, _currentLocation.Longitude,_currentLocation.Accuracy);
                //Android.Util.Log.Info("GetLocation", "Latitude: " + _currentLocation.Latitude.ToString() + "        Longitude: " + _currentLocation.Longitude.ToString() );
                //latlon.Latitude = _currentLocation.Latitude;
                //latlon.Longitude = _currentLocation.Longitude;
                //latlon.Accuracy = _currentLocation.Accuracy;
                //var responseString = await WebRequestServer.SendingCordinates(latlon);
                //Toast.MakeText(this, responseString, ToastLength.Short).Show();
            }
        }

        public void OnProviderDisabled(string provider)
        {
            InitializeLocationManager();
        }

        public void OnProviderEnabled(string provider)
        {
            InitializeLocationManager();
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            
        }
        #endregion

        //Override OnResume so that Activity1 will begin listening to the LocationManager when the activity comes into the foreground:
        protected override void OnResume()
        {
            base.OnResume();
            _locationManager.RequestLocationUpdates(_locationProvider, 10000, 0, this);

        }

        //Override OnPause and unsubscribe Activity1 from the LocationManager when the activity goes into the background
        protected override void OnPause()
        {
            base.OnPause();
            //_locationManager.RemoveUpdates(this);
        }

        protected override void OnDestroy()
        {
       
            base.OnDestroy();
            //Android.Util.Log.Info("GetLocation: ", IsFinishing.ToString());
        }

    }
}

