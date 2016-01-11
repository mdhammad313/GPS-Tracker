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

namespace Sms_Sender
{
    class DataModelSms
    {
        public string getReceiver{ get; set; }

        public string getDate{ get; set; }

        public string getBody { get; set; } 

        public string folderName { get; set; }

    }
}