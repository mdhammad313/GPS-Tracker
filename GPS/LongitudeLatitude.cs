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
    /// 
    /// </summary>
    [Serializable]
     class Coordinates
    {
        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public float Accuracy { get; set; }

        public string DeviceId { get; set; }

        public List<string> provider { get; set; }

        public int uniqueId { get; set; }

        public float speed { get; set; }

        public DateTime timeStamp { get; set; }

        public float Bearing { get; set; }

        public double altittude { get; set; }

    }




}