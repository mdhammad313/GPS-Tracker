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
        //        ISharedPreferences pref = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
        string uniqueId;
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
               
                for (int i = 0; i < acceptableLocationProviders.Count; i++)
                {

                    latlon.provider.Add(acceptableLocationProviders[i]);
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
                    latlon.Latitude = 0;
                    latlon.Longitude = 0;
                    latlon.Accuracy = 0;
                    latlon.speed = 0;
                    BroadcastStarted(latlon);
                }

                try
                {
                    if (_locationProvider != null && _locationProvider != string.Empty)
                    {
                        //Request for gps with 0 time and 0 distance travelled 
                        _locationManager.RequestLocationUpdates(_locationProvider, 10000, 0, this);
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
                //Directly get it from preferences 
                // string uniqueId = pref.GetString("UniqueId", String.Empty);

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
                    latlon.speed = _currentLocation.Speed;
                    //If preferences is empty then take data from global variable which is coming from activity
                    //if (uniqueId == "")
                    //{
                    //    latlon.uniqueId = int.Parse(this.uniqueId);
                    //}

                    //else
                        latlon.uniqueId = int.Parse(uniqueId);

                    BroadcastStarted(latlon);
                    //Send request to server on every update
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
            //Retreive data from intent 
            uniqueId = intent.GetStringExtra("userId");
            //Toast.MakeText(this, "On Start Command: ", ToastLength.Long).Show();
            getDeviceId();
            InitializeLocationManager();
            return StartCommandResult.Sticky;
        }
        #endregion
    }
}