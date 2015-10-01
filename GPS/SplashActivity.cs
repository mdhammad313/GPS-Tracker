using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace GPS
{
    /// <summary>
    /// Check already stored preference
    /// </summary>
    [Activity(Label = "SplashActivity", MainLauncher = true)]
    public class SplashActivity : Activity
    {

        /// <summary>
        /// Firsst Activity to run
        /// </summary>
        /// <param name="bundle"></param>
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            //Check for already stored preferences
            ISharedPreferences pref = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
            string uniqueId = pref.GetString("UniqueId", String.Empty);

            //If not then take it to login Activity
            if (uniqueId == string.Empty)
            {
                Intent intent = new Intent(this, typeof(LoginActivity));
                this.StartActivity(intent);
                this.Finish();
            }

            //If any then save it to coordinates class and send it to Main Activity
            else
            {
                Coordinates getUniqueId = new Coordinates
                {
                    uniqueId = int.Parse(uniqueId)
                };

                Intent intent = new Intent(this, typeof(GelLocation));
                //Send preferences to main activity
                intent.PutExtra("user", JsonConvert.SerializeObject(getUniqueId));
                this.StartActivity(intent);
                this.Finish();
            }

        }
    }
}