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
    /// 
    /// </summary>
    [Activity(Label = "LoginActivity")]
    public class LoginActivity : Activity
    {
        EditText _uniqueId;
        CheckBox _rememberMe;
        Button _loginButton;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bundle"></param>
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.LoginLayout);
            _uniqueId = FindViewById<EditText>(Resource.Id.createUniqueId);
            _rememberMe = FindViewById<CheckBox>(Resource.Id.rememberMe);
            _loginButton = FindViewById<Button>(Resource.Id.loginButton);
            _loginButton.Click += _loginButton_Click;
        }

        private void _loginButton_Click(object sender, EventArgs e)
        {
            try
            {
                Coordinates getUniqueId = new Coordinates
                {
                    //Save textbox id in object
                    uniqueId =  int.Parse(_uniqueId.Text)
                };

                //If checkbox is enabled or disable then save data in shared preference
                if (_rememberMe.Checked || _rememberMe.Checked == false)
                {
                   
                    ISharedPreferences pref = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
                    //Enable us to edit file
                    ISharedPreferencesEditor edit = pref.Edit();
                    edit.PutString("UniqueId", _uniqueId.Text.Trim());
                    edit.Apply();

                    Intent intent = new Intent(this, typeof(GelLocation));
                    //Send preferences to main activity
                    intent.PutExtra("user", JsonConvert.SerializeObject(getUniqueId));
                    this.StartActivity(intent);
                    //User cannot navigate to this activity
                    this.Finish();
                }

                //Otherwise dont save in preferences only send data to main activity
                //else
                //{
                //    Intent intent = new Intent(this, typeof(GelLocation));
                //    //Send preferences to main activity
                //    intent.PutExtra("user", JsonConvert.SerializeObject(getUniqueId));
                //    this.StartActivity(intent);
                //    //User cannot navigate to this activity
                //    this.Finish();
                //}

            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}