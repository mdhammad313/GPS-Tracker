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

namespace Sms_Sender.Model
{
    public class PendingSmsResponseModel : BaseResponseModel
    {
        public List<SavingSmsRequestModel> CodeAndNumber { get; set; }
    }
}