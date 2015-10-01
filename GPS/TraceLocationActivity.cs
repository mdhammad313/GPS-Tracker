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

namespace GPS
{
    /// <summary>
    /// Trace Other user location
    /// </summary>
    [Activity(Label = "TraceLocationActivity")]
    public class TraceLocationActivity : Activity
    {

        private TextView _showData;
        private EditText _sendId;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bundle"></param>
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.TraceLocation);

            _showData = FindViewById<TextView>(Resource.Id.lastLocationTextView);
            _sendId = FindViewById<EditText>(Resource.Id.traceLastLocation);
            FindViewById<Button>(Resource.Id.LastLocationButton).Click += TraceLocationActivity_Click;
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
                Coordinates longlat = new Coordinates();

                if (_sendId.Text == "")
                {
                    Toast.MakeText(this, "First Enter id to Trace Location", ToastLength.Long);
                }

                //Send ID to webRequest class to request server 
                else
                {
                    longlat.uniqueId = int.Parse(_sendId.Text);
                    longlat = await WebRequestServer.TraceLastKnownLocation(longlat);

                    if (longlat == null)
                    {
                        _showData.Text = string.Format("Latitude: " + "N\\A" + "\nLongitude: " + "N\\A" + "\nAccuracy: " + "N\\A" + "\nDatetime: " + "N\\A" + "\nSpeed: " + "N\\A");
                    }

                    else
                        _showData.Text = string.Format("Latitude: " + longlat.Latitude + "\nLongitude: " + longlat.Longitude + "\nAccuracy: " + longlat.Accuracy + "\nDateTime: " + longlat.timeStamp + "\nSpeed: " + longlat.speed);
                }

            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}