using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Locations;

namespace GPS
{
    /// <summary>
    /// Trace Other user location
    /// </summary>
    [Activity(Label = "TraceLocationActivity")]
    public class TraceLocationActivity : Activity
    {
        Coordinates longlat;
        private TextView _showData;
        private EditText _inputId;
        private TextView _streetAddress;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bundle"></param>
        protected override void OnCreate(Bundle bundle)
        {
            try
            {
                base.OnCreate(bundle);
                SetContentView(Resource.Layout.TraceLocation);
                _showData = FindViewById<TextView>(Resource.Id.lastLocationTextView);
                _inputId = FindViewById<EditText>(Resource.Id.traceLastLocation);
                _streetAddress = FindViewById<TextView>(Resource.Id.streetAddressTextView);
                FindViewById<Button>(Resource.Id.LastLocationButton).Click += TraceLocationActivity_Click;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async private void TraceLocationActivity_Click(object sender, EventArgs e)
        {
            try
            {
                longlat = new Coordinates();

                if (_inputId.Text == "")
                {
                    Toast.MakeText(this, "First Enter id to Trace Location", ToastLength.Long);
                }

                else
                {
                    //Send ID to webRequest class to request server 
                    longlat.uniqueId = int.Parse(_inputId.Text);
                    longlat = await WebRequestServer.TraceLastKnownLocation(longlat);

                    if (longlat == null)
                    {
                        _showData.Text = string.Format("Latitude: " + "N\\A" + "\nLongitude: " + "N\\A" + "\nAccuracy: " + "N\\A" + "\nDatetime: " + "N\\A" + "\nSpeed: " + "N\\A" + "\nBearing: " + "N\\A" + "\nAltitude: " + "N\\A");
                    }

                    else
                        _showData.Text = string.Format("Latitude: " + longlat.Latitude + "\nLongitude: " + longlat.Longitude + "\nAccuracy: " + longlat.Accuracy + "\nDateTime: " + longlat.timeStamp + "\nSpeed: " + longlat.speed + "\nBearing: " + longlat.Bearing + "\nAltitude: " + longlat.altittude);
                }
                streetAddress(longlat);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Street Address
        /// </summary>
        /// <param name="latlon"></param>
        async private void streetAddress(Coordinates latlon)
        {
            try
            {
                if (latlon == null || latlon.Latitude == 0 || latlon.Longitude == 0 )
                {
                    _streetAddress.Text = "Can't determine the current address.";
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
                        _streetAddress.Text = deviceAddress.ToString();
                    }
                    else
                    {
                        _streetAddress.Text = "Unable to determine the address.";
                    }

                    allLocations.Add(_streetAddress.Text);
                }
                _streetAddress.Text = string.Join("\n", allLocations);

            }

            catch (Java.IO.IOException ex)
            {
                Toast.MakeText(this, "Network is unreachable", ToastLength.Short).Show();
            }

            catch (Exception ex)
            {
                throw;
            }

        }



    }
}