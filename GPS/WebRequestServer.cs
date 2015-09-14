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

namespace GPS
{
    class WebRequestServer
    {
        
        private static Uri ServerRootURL = new Uri("http://192.168.0.1");
        //Sending data in Json format to webservice
        async public static Task<string> SendingCordinates(Coordinates longlat)
        {

            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = ServerRootURL;
                client.DefaultRequestHeaders.Add("Accept", "application/json");

                string requestJson = JsonConvert.SerializeObject(longlat);

                StringContent requestContent = new StringContent(requestJson);
                requestContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                HttpResponseMessage response = await client.PostAsync("/webservice/api/location/getlocation", requestContent);

                if (response.IsSuccessStatusCode)
                {
                    return "Location successfully saved on server";
                }

                return "Unable to save location on server";

            }
            catch (Exception ex)
            {

                throw;
            }
        }

    }
}