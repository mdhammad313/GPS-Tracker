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
        //If wifi off Web exception (Server not found)
        //if ip address of server wrong resource not found
        //if ip address correct but webservice not found then resource not found 404
        //if id not available then content not found 204
        //if different networks then task exception handled
        static TaskCanceledException tce = new TaskCanceledException();

        /// <summary>
        /// Server Ip where Server is hosted
        /// </summary>
        /// 
        //private static Uri ServerRootURL = new Uri("http://192.168.0.71");
        //private static string ServerLocation = "/webservice/api/location/";

         private static Uri ServerRootURL = new Uri("http://pggpstracker.azurewebsites.net/");
         private static string ServerLocation = "/api/location/";

        /// <summary>
        /// 
        /// </summary>

        /// <summary>
        /// Sending data in Json format to webservice
        /// </summary>
        /// <param name="longlat"></param>
        /// <returns></returns>
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
                HttpResponseMessage response = await client.PostAsync(ServerLocation + "getlocation", requestContent);

                //If reponse is true
                if (response.IsSuccessStatusCode)
                {
                    return "Saved on server";
                }

                //If resource not found
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return "Resource " + response.ReasonPhrase;
                }

                else
                    return "Unable to save";

            }

            catch (System.Net.WebException ex)
            {
                return "Server Not Found";
            }

            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Retreiveing Data
        /// </summary>
        /// <param name="longlat"></param>
        /// <returns></returns>
        async public static Task<Coordinates> GetLastKnownLocation(Coordinates longlat)
        {
            try
            {
                RequestModel rm = new RequestModel();
                rm.LocationsObject = longlat;
                HttpClient client = new HttpClient();
                client.BaseAddress = ServerRootURL;
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                string requestJson = JsonConvert.SerializeObject(rm);
                StringContent requestContent = new StringContent(requestJson);
                requestContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                HttpResponseMessage response = await client.PostAsync(ServerLocation + "tracelastlocation", requestContent);

                if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    Toast.MakeText(Application.Context, "Id Not Available", ToastLength.Short).Show();
                    return null;
                }

                else if (response.IsSuccessStatusCode)
                {
                    string lastLocation = await response.Content.ReadAsStringAsync();
                    longlat = JsonConvert.DeserializeObject<Coordinates>(lastLocation);
                    return longlat;
                }

                //If resource not found
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }

                //Id content not found
                else
                    return null;
            }

            catch (System.Net.WebException ex)
            {
                return null;
            }

            catch (TaskCanceledException ex)
            {
                throw;
            }

            catch (Exception ex)
            {

                throw;
            }
        }

        /// <summary>
        /// Retreiving others last known location
        /// </summary>
        /// <param name="longlat"></param>
        /// <returns></returns>
        async public static Task<Coordinates> TraceLastKnownLocation(Coordinates longlat)
        {
            try
            {
                RequestModel rm = new RequestModel();
                rm.LocationsObject = longlat;
                HttpClient client = new HttpClient();
                client.BaseAddress = ServerRootURL;
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                string requestJson = JsonConvert.SerializeObject(rm);
                StringContent requestContent = new StringContent(requestJson);
                requestContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                HttpResponseMessage response = await client.PostAsync(ServerLocation + "tracelastlocation", requestContent);

                if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    Toast.MakeText(Application.Context, "Id Not Available", ToastLength.Short).Show();
                    return null;
                }

                else if (response.IsSuccessStatusCode)
                {
                    string lastLocation = await response.Content.ReadAsStringAsync();
                    longlat = JsonConvert.DeserializeObject<Coordinates>(lastLocation);
                    return longlat;
                }

                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }

                else
                {
                    return null;
                }
            }

            catch (System.Net.WebException ex)
            {
                return null;
            }

            catch (Exception ex)
            {
                throw;
            }
        }


    }
}