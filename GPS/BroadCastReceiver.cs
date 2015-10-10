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
    [BroadcastReceiver]
    [IntentFilter(new[] { Intent.ActionBootCompleted })]
    class BroadCastReceiver : BroadcastReceiver
    {
     //   private BackgroundService bs;
      //  private Context cont;
        /// <summary>
        /// Invoked by OS when device is rebooted Permisssion is being given
        /// in manifest . When device rebooted OS broadcast it to all 
        /// applications and receiver with this permisssion willbe invoked 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="intent"></param>
        public override void OnReceive(Context context, Intent intent)
        {

            try
            {
                Toast.MakeText(context, "Broadcast Receive: ", ToastLength.Long).Show();
                context.StartService(new Intent(context, typeof(BackgroundService)));
            }
            catch (Exception ex)
            {

            }
        }
    }
}