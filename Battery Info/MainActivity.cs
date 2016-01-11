using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;


namespace Battery_Info
{
    [Activity(Label = "Battery_Info", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
            private TextView batteryStatusTextView;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.BatteryStatus);
            Button batteryButton = FindViewById<Button>(Resource.Id.batteryButton);
            batteryStatusTextView = FindViewById<TextView>(Resource.Id.batteryStatusTextView);
            batteryStatus();
        }

        public void batteryStatus()
        {
            var filter = new IntentFilter(Intent.ActionBatteryChanged);
            var battery = RegisterReceiver(null, filter);
            int level = battery.GetIntExtra(BatteryManager.ExtraLevel, -1);
            int scale = battery.GetIntExtra(BatteryManager.ExtraScale, -1);

            int BPercetage = (int)System.Math.Floor(level * 100D / scale);
            batteryStatusTextView.Text += BPercetage + "%";
        }
    }
}

