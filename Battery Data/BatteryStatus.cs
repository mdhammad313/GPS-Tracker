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

namespace Battery_Data
{
    [Activity(Label = "BatteryStatusActivity" , MainLauncher = true)]
    public class BatteryStatus : Activity
    {
        string[] batteryStatusArray = new string[]{"", "UnKnown", "Charging", "Dis Charging", "Not Charging", "Full" };
        private TextView batteryLevelTextView;
        private TextView batteryStatusTextView;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.BatteryStatus);
            Button batteryButton = FindViewById<Button>(Resource.Id.batteryButton);
            batteryLevelTextView = FindViewById<TextView>(Resource.Id.batteryLevelTextView);
            batteryStatusTextView = FindViewById<TextView>(Resource.Id.batteryStatusTextView);
            batteryStatus();
        }

        public void batteryStatus()
        {
            var filter = new IntentFilter(Intent.ActionBatteryChanged);
            var battery = RegisterReceiver(null, filter);
            int level = battery.GetIntExtra(BatteryManager.ExtraLevel, -1);
            int scale = battery.GetIntExtra(BatteryManager.ExtraScale, -1);
            int status = battery.GetIntExtra(BatteryManager.ExtraStatus, -1);

            int BPercetage = (int)System.Math.Floor(level * 100D / scale);
            batteryLevelTextView.Text += BPercetage + "%";
            batteryStatusTextView.Text += batteryStatusArray[status];            

            
        }
    }
}