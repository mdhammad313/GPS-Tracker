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
using Newtonsoft.Json;

namespace GPS
{
    [Service]
    class BackgroundService : Service, ILocationListener 
    {
        Coordinates latlon = new Coordinates();
        Location _currentLocation;
        LocationManager _locationManager;
        System.String _locationProvider;

        /// <summary>
        /// Service broadcasting to activities along with data 
        /// </summary>
        /// <param name="location"></param>
        
        private void BroadcastStarted(Coordinates location)
        {

            try
            {
                Intent BroadcastIntent = new Intent(this, typeof(GelLocation.MyLocationReceiver));
                BroadcastIntent.SetAction(GelLocation.MyLocationReceiver.GRID_STARTED);
                BroadcastIntent.AddCategory(Intent.CategoryDefault);

                var cordinates = JsonConvert.SerializeObject(location);
                BroadcastIntent.PutExtra("cor", cordinates);
                SendBroadcast(BroadcastIntent);

            }
            catch (Exception ex)
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

        private void InitializeLocationManager()
        {
            try
            {
                latlon.provider = new List<string>();
                _locationManager = (LocationManager)GetSystemService(LocationService);
                //Application criteria for selecting provider
                Criteria criteriaForLocationService = new Criteria { Accuracy = Accuracy.NoRequirement };
                IList<string> acceptableLocationProviders = _locationManager.GetProviders(criteriaForLocationService, true);

                //List<string> providersList = new List<string>();

                for (int i = 0; i < _locationManager.AllProviders.Count; i++)
                {

                    latlon.provider.Add(_locationManager.AllProviders[i]);
                    //providersList.Add();
                }

                BroadcastStarted(latlon);



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
                        _locationManager.RequestLocationUpdates(_locationProvider, 3000, 0, this);
                        //Once aplication has requested updates now ILocationListener interface will be called and OnLocationUpdate method called
                        //due to 0 distance covered

                    }
                }
                catch (System.Exception ex)
                {
                    throw;
                }

            }
            catch (Exception message)
            {
                
                throw;
            }

        }

        #region Interface IlocationListener
        async public void OnLocationChanged(Location location)
        {

            try
            {
                _currentLocation = location;
                if (_currentLocation == null)
                {

                }

                else
                {
                   // Android.Util.Log.Info("GetLocation", "Latitude: " + _currentLocation.Latitude.ToString() + "        Longitude: " + _currentLocation.Longitude.ToString());
                    latlon.Latitude = _currentLocation.Latitude;
                    latlon.Longitude = _currentLocation.Longitude;
                    latlon.Accuracy = _currentLocation.Accuracy;
                    BroadcastStarted(latlon);
                    var responseString = await WebRequestServer.SendingCordinates(latlon);
                    Toast.MakeText(this, responseString, ToastLength.Short).Show();
                }

            }
            catch (Exception ex)
            {

                throw;
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

        #region LifeCycleEvents
        public override IBinder OnBind(Intent intent)
        {
            throw new NotImplementedException();
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            //Toast.MakeText(this, "On Start Command: ", ToastLength.Long).Show();
            getDeviceId();
            InitializeLocationManager();
            return StartCommandResult.Sticky;
        }
        #endregion
    }
}