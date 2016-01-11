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
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Sms_Sender.Model;

namespace Sms_Sender
{
    class RequestServer
    {

        private static Uri ServerRootURL = new Uri("http://192.168.0.71");
        private static string ServerLocation = "/drug/api/verification/";

        async public Task<List<SavingSmsRequestModel>> SendingCordinates()
        {
            try
            {
                PendingSmsResponseModel responseModel = new Model.PendingSmsResponseModel();
                HttpClient client = new HttpClient();
                client.BaseAddress = ServerRootURL;
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                //string requestJson = JsonConvert.SerializeObject();
                //StringContent requestContent = new StringContent(requestJson);
                //requestContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                HttpResponseMessage response = await client.GetAsync(ServerLocation + "pendingverification");

                string responseData = await response.Content.ReadAsStringAsync();
                responseModel = JsonConvert.DeserializeObject<PendingSmsResponseModel>(responseData);

                if (responseModel.IsValid == false)
                {
                    return null;
                }
                
                return responseModel.CodeAndNumber;
            }
            catch (System.Net.WebException ex)
            {
                return null;
            }

            catch (Exception ex)
            {
                return null;
            }
        }


    }
}