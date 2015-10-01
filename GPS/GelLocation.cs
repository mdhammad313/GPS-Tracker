using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Locations;


using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Newtonsoft.Json;
using Android.Telephony;

namespace GPS
{
    /// <summary>
    /// 
    /// </summary>
    [Activity(Label = "GPS", MainLauncher = false, Icon = "@drawable/icon")]
    public class GelLocation : Activity
    {
        Coordinates latlon = new Coordinates();
        TextView _locationText;
        TextView _addressText;
        TextView _lastKnownLocation;
        TextView _listOfProvider;
        Button _traceLocationButton;
        Coordinates sharedPreferenceId;

        /// <summary>
        /// Initialize instance of class 
        /// </summary>
        public static GelLocation instance;

        private MyLocationReceiver _receiver;

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bundle"></param>
        async protected override void OnCreate(Bundle bundle)
        {
            try
            {
                base.OnCreate(bundle);
                //Get object of this class to show data in broadc
                GelLocation.instance = this;

                SetContentView(Resource.Layout.GetLocation);
                _locationText = FindViewById<TextView>(Resource.Id.currentLocationTextView);
                _addressText = FindViewById<TextView>(Resource.Id.addressTextView);
                _lastKnownLocation = FindViewById<TextView>(Resource.Id.lastLocationTextView);
                _listOfProvider = FindViewById<TextView>(Resource.Id.providerList);
//                FindViewById<TextView>(Resource.Id.getAddressButton).Click += GelLocation_Click;
                _traceLocationButton = FindViewById<Button>(Resource.Id.traceLocationButton);
                _traceLocationButton.Click += _traceLocationButton_Click;

                //Get shared preferences data 
                sharedPreferenceId = JsonConvert.DeserializeObject<Coordinates>(Intent.GetStringExtra("user"));

                //Get device id for currentr user
                getDeviceId();

                //Get last known location of current user
                latlon = await WebRequestServer.GetLastKnownLocation(latlon);

                //if respond is null
                if (latlon == null)
                {
                    _lastKnownLocation.Text = string.Format("Latitude: " + "N\\A" + "\nLongitude: " + "N\\A" + "\nAccuracy: " + "N\\A");
                }

                //Otherwise perform
                else
                    _lastKnownLocation.Text = string.Format("Latitude: " + latlon.Latitude + "\nLongitude: " + latlon.Longitude + "\nAccuracy: " + latlon.Accuracy);

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void _traceLocationButton_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(TraceLocationActivity));
            this.StartActivity(intent);
        }

        /// <summary>
        /// Start service on start up
        /// </summary>
        protected override void OnStart()
        {
            base.OnStart();
            Intent intent = new Intent(this, typeof(BackgroundService));
            //Send preferences to the service to send id to server
            intent.PutExtra("userId", sharedPreferenceId.uniqueId.ToString());
            StartService(intent);
            GelLocation.instance = this;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnPause()
        {
            base.OnPause();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();
            // UnregisterReceiver(_receiver);
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnResume()
        {
            //Register custom receiver receiver 
            base.OnResume();
            IntentFilter filter = new IntentFilter(MyLocationReceiver.GRID_STARTED);
            filter.AddCategory(Intent.CategoryDefault);
            _receiver = new MyLocationReceiver(this);
            RegisterReceiver(_receiver, filter);
        }

 


        async private void streetAddress()
        {
            try
            {
                if (latlon.Latitude == 0 || latlon.Longitude == 0 || latlon == null)
                {
                    _addressText.Text = "Can't determine the current address.";
                    return;
                }

                Geocoder geocoder = new Geocoder(this);
                IList<Address> addressList = await geocoder.GetFromLocationAsync(latlon.Latitude, latlon.Longitude, 10);
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

            catch (Java.IO.IOException ex)
            {
                Toast.MakeText(Application.Context, "Network is unreachable", ToastLength.Short).Show();
            }

            catch (Exception ex)
            {
                throw;
            }

        }


        /// <summary>
        /// Custom receiver class
        /// </summary>
        [BroadcastReceiver]
        public class MyLocationReceiver : BroadcastReceiver
        {
            GelLocation _myactivity;
            //Coordinates GetLocation;
            /// <summary>
            /// 
            /// </summary>
            public MyLocationReceiver()
            {

            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="myactivity"></param>
            public MyLocationReceiver(GelLocation myactivity)
            {
                _myactivity = myactivity;
            }

            /// <summary>
            /// 
            /// </summary>
            public static readonly string GRID_STARTED = "GRID_STARTED";
            /// <summary>
            /// 
            /// </summary>
            /// <param name="context"></param>
            /// <param name="intent"></param>
            public override void OnReceive(Context context, Intent intent)
            {
                try
                {
                    //Geting instance of base class
                    _myactivity = GelLocation.instance;

                    
                    if (intent.Action == GRID_STARTED)
                    {
                        _myactivity.latlon = JsonConvert.DeserializeObject<Coordinates>(intent.GetStringExtra("cor"));

                        //_myactivity.RunOnUiThread(() =>
                        //{

                        _myactivity._locationText.Text = String.Format("Latitude: {0} \nLongitude: {1} \nAccuracy: {2} \nSpeed: {3}", _myactivity.latlon.Latitude, _myactivity.latlon.Longitude, _myactivity.latlon.Accuracy , _myactivity.latlon.speed);
                        _myactivity.streetAddress();

                        //Print provider list
                        _myactivity._listOfProvider.Text = "";

                        if(_myactivity.latlon.provider.Count == 0)
                        {
                            _myactivity._listOfProvider.Text = "N\\A";
                        }

                        for (int i = 0; i < _myactivity.latlon.provider.Count; i++)
                        {
                            _myactivity._listOfProvider.Text += String.Format(_myactivity.latlon.provider[i]);

                            if (i < _myactivity.latlon.provider.Count - 1)
                            {
                                _myactivity._listOfProvider.Text += "\n";
                            }
                        }
                        //});

                        //  Toast.MakeText(context, "Custom Receiver", ToastLength.Long).Show();
                    }

                }
                catch (Exception ex)
                {

                    throw;
                }
            }
        }
    }

}

