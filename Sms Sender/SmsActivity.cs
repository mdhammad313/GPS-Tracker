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
using Android.Telephony;
using Android.Provider;

namespace Sms_Sender
{
    [Activity(Label = "SmsActivity", MainLauncher = true)]
    public class SmsActivity : Activity
    {
        string address = "";
        string displayName = "";
        string sms = "";
        string selectedItem = "";
        Button inboxButton;
        Button sendSMSButton;
        EditText inputNumber;
        TextView showInbox;
        Spinner smsSpinner;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bundle"></param>
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.SendSms);
            sendSMSButton = FindViewById<Button>(Resource.Id.sendSMS);
            inputNumber = FindViewById<EditText>(Resource.Id.phoneTextBox);
            inboxButton = FindViewById<Button>(Resource.Id.inboxButton);
            inboxButton.Click += InboxButton_Click;
            sendSMSButton.Click += SendSMSButton_Click;
            showInbox = FindViewById<TextView>(Resource.Id.showInbox);
            inputNumber.EditorAction += InputNumber_EditorAction;
            smsSpinner = FindViewById<Spinner>(Resource.Id.smsSpinner);


            //Binding string.XMl to spinner
            smsSpinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(SmsSpinner_ItemSelected);
            var adapter = ArrayAdapter.CreateFromResource(this, Resource.Array.smsArray, Android.Resource.Layout.SimpleSpinnerItem);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            smsSpinner.Adapter = adapter;




        }

        //When spinner Item Changes
        private void SmsSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            try
            {
                Spinner spinner = (Spinner)sender;
                selectedItem = spinner.GetItemAtPosition(e.Position).ToString();

                sms = "";
                List<DataModelSms> accessSMS = new List<DataModelSms>();
                //int counter = 1;
                accessSMS = getAllSms();

                foreach (DataModelSms dms in accessSMS)
                {
                    //Look sms number in phonelookup table
                    Android.Net.Uri uri = Android.Net.Uri.WithAppendedPath(ContactsContract.PhoneLookup.ContentFilterUri, Android.Net.Uri.Encode(dms.getReceiver));

                    //Phone lookup table 
                    //Returns data with specified table , column and where clause
                    Android.Database.ICursor cursor = ContentResolver.Query(uri, new String[] { ContactsContract.PhoneLookup.InterfaceConsts.DisplayName }, null, null, null);

                    //Move cursor to first row
                    if (cursor.MoveToFirst())
                    {
                        do
                        {
                            //Get column index to print that column value of first row and then print string by column index of first row
                            displayName = cursor.GetString(cursor.GetColumnIndex(ContactsContract.Contacts.InterfaceConsts.DisplayName));
                        }
                        while (cursor.MoveToNext());
                    }

                    else
                        displayName = "";



                    if (dms.folderName == "inbox")
                    {
                        address = "Sender";
                    }

                    else if (dms.folderName == "sent")
                    {
                        address = "Receiver";
                    }

                    else
                        address = "Saved";

                    if (dms.folderName == selectedItem)
                    {
                        sms += string.Format("{4}: {0} \nDisplay Name: {5} \nDate: {1} \nBody:{2} \nType:{3}\n----------------------\n", dms.getReceiver, dms.getDate, dms.getBody, dms.folderName, address, displayName);
                    }
                }
                showInbox.Text = sms;
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        //Keyboard Input button event
        private void InputNumber_EditorAction(object sender, TextView.EditorActionEventArgs e)
        {
            if (e.ActionId == Android.Views.InputMethods.ImeAction.Send)
            {
                sendSMS();
            }

        }

        private void sendSMS()
        {
            if (inputNumber.Text == "")
            {
                return;
            }

            SmsManager.Default.SendTextMessage(inputNumber.Text, null, "Hello from Hammad Shabbir", null, null);
        }

        private void SendSMSButton_Click(object sender, EventArgs e)
        {
            sendSMS();
        }


        private void InboxButton_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    List<DataModelSms> accessSMS = new List<DataModelSms>();
            //    int counter = 1;
            //    accessSMS = getAllSms();

            //    foreach (DataModelSms dms in accessSMS)
            //    {
            //        if (dms.folderName == "Draft")
            //        {
            //            sms += string.Format("Counter: {4} \nFrom: {0} \nDate: {1} \nBody:{2} \nType:{3}\n----------------------\n", dms.getReceiver, dms.getDate, dms.getBody, dms.folderName, counter++);
            //        }
            //    }
            //    showInbox.Text = sms;
            //}
            //catch (Exception ex)
            //{
            //    throw;
            //}
        }

        private List<DataModelSms> getAllSms()
        {

            try
            {
                List<DataModelSms> allSms = new List<DataModelSms>();
                DataModelSms singleSMS;
                var smsuri = Android.Net.Uri.Parse("content://sms/"+ selectedItem );
                Android.Database.ICursor cur = ContentResolver.Query(smsuri, null, null, null, null);
                int col = cur.ColumnCount;
                int row = cur.Count;
                string[] columnnames = cur.GetColumnNames();
                //sms = "No of rows" + row + "\n";

                while (cur.MoveToNext())
                {
                    //Show All columnsof each smns
                    //for (int i = 0; i < col; i++)
                    //{
                    //    sms += cur.GetColumnName(i) + ": " + cur.GetString(i) + "\n";

                    //}
                    singleSMS = new DataModelSms();
                    singleSMS.getReceiver = cur.GetString(cur.GetColumnIndex("address"));
                    var smsTime = new DateTime(1970, 1, 1, 5, 0, 0, DateTimeKind.Utc);
                    singleSMS.getDate = smsTime.AddMilliseconds(cur.GetDouble(cur.GetColumnIndex("date"))).ToString();
                    singleSMS.getBody = cur.GetString(cur.GetColumnIndex("body"));

                    //Specific row . first get column index of that row and then get string for that specific colimn an row
                    if (cur.GetString(cur.GetColumnIndex("type")).Contains("1"))
                    {
                        singleSMS.folderName = "inbox";
                    }

                    else if (cur.GetString(cur.GetColumnIndex("type")).Contains("2"))
                        singleSMS.folderName = "sent";

                    else if(cur.GetString(cur.GetColumnIndex("type")).Contains("3"))
                        singleSMS.folderName = "draft";

                    allSms.Add(singleSMS);

                }
                return allSms;

            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}