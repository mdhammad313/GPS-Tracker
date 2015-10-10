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
        ISharedPreferences pref = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
        // string uniqueId;
        Coordinates latlon = new Coordinates();
        Location _currentLocation;
        LocationManager _locationManager;
        System.String _locationProvider;
        DateTime[] broadCastDate = new DateTime[5];
        DateTime[] storeTimeElapse= new DateTime[5];

        /// <summary>
        /// Service broadcasting to activities along with data 
        /// </summary>
        /// <param name="location"></param>
        /// <param name="InComingData"></param>
        private void BroadcastStarted(Coordinates location, string InComingData)
        {
            try
            {
                Intent BroadcastIntent = new Intent(this, typeof(GelLocation.MyLocationReceiver));
                BroadcastIntent.SetAction(GelLocation.MyLocationReceiver.GRID_STARTED);
                BroadcastIntent.AddCategory(Intent.CategoryDefault);                
                var cordinates = JsonConvert.SerializeObject(location);
                BroadcastIntent.PutExtra(InComingData, cordinates);
                SendBroadcast(BroadcastIntent);
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, "Broadcast", ToastLength.Long).Show();
            }
        }

        private void LastTwoBroadcastDate(DateTime[] timeStamp)
        {
            try
            {
                Intent BroadcastIntent = new Intent(this, typeof(GelLocation.MyLocationReceiver));
                BroadcastIntent.SetAction(GelLocation.MyLocationReceiver.GRID_STARTED);
                BroadcastIntent.AddCategory(Intent.CategoryDefault);

                var cordinates = JsonConvert.SerializeObject(timeStamp);
                BroadcastIntent.PutExtra("timeStamp", cordinates);
                SendBroadcast(BroadcastIntent);

            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, "Broadcast two", ToastLength.Long).Show();
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
                Toast.MakeText(Application.Context, "DeviceId", ToastLength.Long).Show();
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

                BroadcastStarted(latlon, "provider");

                if (acceptableLocationProviders.Any())
                {
                    _locationProvider = acceptableLocationProviders.First();
                }
                else
                {
                    //Set these properties if no provideer is enabled
                    _locationProvider = String.Empty;
                    latlon.Latitude = 0;
                    latlon.Longitude = 0;
                    latlon.Accuracy = 0;
                    latlon.speed = 0;
                    //BroadcastStarted(latlon);
                }

                try
                {
                    if (_locationProvider != null && _locationProvider != string.Empty)
                    {
                        //Request for gps with 0 time and 0 distance travelled 
                        _locationManager.RequestLocationUpdates(_locationProvider, 30000, 0, this);
                        //Once aplication has requested updates now ILocationListener interface will be called and OnLocationUpdate method called
                        //due to 0 distance covered
                    }
                }
                catch (System.Exception ex)
                {
                    Toast.MakeText(Application.Context, "ILM1", ToastLength.Long).Show();
                }
            }
            catch (Exception message)
            {
                Toast.MakeText(Application.Context, "ILM2", ToastLength.Long).Show();
            }
        }

        #region Interface IlocationListener
        async public void OnLocationChanged(Location location)
        {
            try
            {
                //Directly get it from preferences 
                string uniqueId = pref.GetString("UniqueId", String.Empty);
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
                    latlon.Bearing = _currentLocation.Bearing;
                    latlon.altittude = _currentLocation.Altitude;

                    //Get time from location class
                    var gpsTime = new DateTime(1970, 1, 1, 5, 0, 0, DateTimeKind.Utc);
                    latlon.timeStamp = gpsTime.AddMilliseconds(_currentLocation.Time);

                    //Last five time elapsed
                    for(int i =0; i < broadCastDate.Length; i++)
                    {
                        storeTimeElapse[i] = broadCastDate[i];  
                        if(i == 0)
                        {
                            broadCastDate[i] = latlon.timeStamp;
                        }

                        else
                        {
                            broadCastDate[i] = storeTimeElapse[i - 1];
                        }
                    }
                    
                    //If preferences is empty then take data from global variable which is coming from activity
                    if (uniqueId == "")
                    {
                        Toast.MakeText(Application.Context, "No id to save data  ", ToastLength.Short).Show();
                        //latlon.uniqueId = int.Parse(this.uniqueId);
                    }

                    else
                        latlon.uniqueId = int.Parse(uniqueId);

                    BroadcastStarted(latlon, "cordinates");
                    LastTwoBroadcastDate(broadCastDate);
                    //Send request to server on every update

                    //If accuracy > 0 then discard value
                    if (latlon.Accuracy >= 100)
                    {
                        return;
                    }

                    var responseString = await WebRequestServer.SendingCordinates(latlon);
                    Toast.MakeText(Application.Context, responseString, ToastLength.Short).Show();
                }

            }

            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Long).Show();
            }
        }

        public void OnProviderDisabled(string provider)
        {
            try
            {
                InitializeLocationManager();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
            }
        }

        public void OnProviderEnabled(string provider)
        {
            try
            {
                InitializeLocationManager();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
            }
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            Toast.MakeText(Application.Context, "status Changed", ToastLength.Long).Show();
        }
        #endregion

        #region LifeCycleEvents
        public override IBinder OnBind(Intent intent)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, "OnBind", ToastLength.Long).Show();
            }
            return null;
        }

        [Obsolete]
        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            try
            {
                //Retreive data from intent 
                //uniqueId = intent.GetStringExtra("userId");
                getDeviceId();
                InitializeLocationManager();
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, "OnStartCommmand", ToastLength.Long).Show();
            }
            return StartCommandResult.Sticky;
        }
        #endregion
    }
}