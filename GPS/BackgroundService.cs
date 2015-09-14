using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Locations;
using Android.Views;
using Android.Widget;
using Android.Telephony;

namespace GPS
{
    [Service]
    class BackgroundService : Service, ILocationListener
    {
        Coordinates latlon = new Coordinates();
        Location _currentLocation;
        LocationManager _locationManager;
        System.String _locationProvider;


        private void InitializeLocationManager()
        {
            _locationManager = (LocationManager)GetSystemService(LocationService);
            //Application criteria for selecting provider
            Criteria criteriaForLocationService = new Criteria { Accuracy = Accuracy.NoRequirement };
            IList<string> acceptableLocationProviders = _locationManager.GetProviders(criteriaForLocationService, true);

            if (acceptableLocationProviders.Any())
            {
                _locationProvider = acceptableLocationProviders.First();
            }
            else
            {
                _locationProvider = String.Empty;
            }

            try
            {
                if (_locationProvider != null)
                {
                    //Request for gps with 0 time and 0 distance travelled 
                    _locationManager.RequestLocationUpdates(_locationProvider, 0, 0, this);
                    //Once aplication has requested updates now ILocationListener interface will be called and OnLocationUpdate method called
                    //due to 0 distance covered

                }
            }
            catch (System.Exception ex)
            {
                throw;
            }


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


        #region Interface IlocationListener

        async public void OnLocationChanged(Location location)
        {
            _currentLocation = location;

            if (_currentLocation == null)
            {
               // _locationText.Text = "Unable to determine your location.";
            }
            else
            {
                //_locationText.Text = String.Format("{0},{1}", _currentLocation.Latitude, _currentLocation.Longitude);
                Android.Util.Log.Info("GetLocation", "Latitude: " + _currentLocation.Latitude.ToString() + "        Longitude: " + _currentLocation.Longitude.ToString());
                latlon.Latitude = _currentLocation.Latitude;
                latlon.Longitude = _currentLocation.Longitude;
                latlon.Accuracy = _currentLocation.Accuracy;
                var responseString = await WebRequestServer.SendingCordinates(latlon);
                Toast.MakeText(this, responseString, ToastLength.Short).Show();
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
            throw new NotImplementedException();
        }
        #endregion

        #region LifeCycleEvents
        public override IBinder OnBind(Intent intent)
        {
            throw new NotImplementedException();
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {

            getDeviceId();
            InitializeLocationManager();

            return base.OnStartCommand(intent, flags, startId);

        }

        public override void OnStart(Intent intent, int startId)
        {
            base.OnStart(intent, startId);
        }
        #endregion
    }
}