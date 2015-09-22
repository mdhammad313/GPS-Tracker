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
    [Activity(Label = "TraceLocationActivity", MainLauncher = true)]
    public class TraceLocationActivity : Activity
    {
        Coordinates longlat = new Coordinates();
        private TextView _showData;
        private EditText _sendId;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.TraceLocation);
            _showData = FindViewById<TextView>(Resource.Id.lastLocationTextView);
            _sendId = FindViewById<EditText>(Resource.Id.traceLastLocation);
            FindViewById<Button>(Resource.Id.LastLocationButton).Click += TraceLocationActivity_Click;
            // Create your application here

        }

        async private void TraceLocationActivity_Click(object sender, EventArgs e)
        {
            try
            {
                if (_sendId.Text == "")
                {
                    Toast.MakeText(this, "First Enter id to Trace Location", ToastLength.Long);
                }

                else
                {
                    longlat.uniqueId = int.Parse(_sendId.Text);
                    longlat = await WebRequestServer.TraceLastKnownLocation(longlat);

                    if (longlat == null)
                    {
                        _showData.Text = string.Format("Latitude: " + "N\\A" + "\nLongitude: " + "N\\A" + "\nAccuracy: " + "N\\A");
                    }

                    else
                        _showData.Text = string.Format("Latitude: " + longlat.Latitude + "\nLongitude: " + longlat.Longitude + "\nAccuracy: " + longlat.Accuracy);
                }

            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}