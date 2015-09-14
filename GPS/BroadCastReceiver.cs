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
    [BroadcastReceiver]
    [IntentFilter(new[] { Intent.ActionBootCompleted } , Categories = new[] { "android.intent.category.DEFAULT" })]
    class BroadCastReceiver : BroadcastReceiver
    {
        
        public override void OnReceive(Context context, Intent intent)
        {

            Toast.MakeText(context, "Intent-action: " , ToastLength.Long).Show();
            context.StartService(new Intent(context, typeof(BackgroundService)));
        }
    }
}