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
        /// <summary>
        /// Server Ip where Server is hosted
        /// </summary>
        /// 
        //private static Uri ServerRootURL = new Uri("http://192.168.0.71");
        private static Uri ServerRootURL = new Uri("http://pggpstracker.azurewebsites.net/");

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

                //HttpResponseMessage response = await client.PostAsync("/webservice/api/location/getlocation", requestContent);
                HttpResponseMessage response = await client.PostAsync("/api/location/getlocation", requestContent);

                if (response.IsSuccessStatusCode)
                {
                    return "Location successfully saved on server";
                }

                return "Unable to save location on server";

            }

            catch (System.Net.WebException ex)
            {
                Toast.MakeText(Application.Context, "Network is unreachable", ToastLength.Short).Show();
                return null;
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
                Coordinates latlon = new Coordinates();
                HttpClient client = new HttpClient();
                client.BaseAddress = ServerRootURL;
                client.DefaultRequestHeaders.Add("Accept", "application/json");


                string requestJson = JsonConvert.SerializeObject(longlat);
                StringContent requestContent = new StringContent(requestJson);
                requestContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                //HttpResponseMessage response = await client.PostAsync("/webservice/api/location/lastlocation", requestContent);
                HttpResponseMessage response = await client.PostAsync("/api/location/lastlocation", requestContent);


                if (response.IsSuccessStatusCode)
                {
                    string lastLocation = await response.Content.ReadAsStringAsync();
                    latlon = JsonConvert.DeserializeObject<Coordinates>(lastLocation);


                    //string LastKnownLocation = JsonConvert.DeserializeObject<string>(lastLocation);
                    //LastKnownLocation.Trim();
                    //int index = LastKnownLocation.IndexOf("L", 0);
                    //int secondIndex;
                    //FormatingString = LastKnownLocation.Substring(1,index-1);
                    //FormatingString += "\n";
                    //secondIndex =  LastKnownLocation.IndexOf("L", index+1);
                    //secondIndex = secondIndex - index;
                    //FormatingString += LastKnownLocation.Substring(index, secondIndex - 1);
                    //FormatingString += "\n";
                    //int newPosition = index + secondIndex; 
                    //index = LastKnownLocation.IndexOf("A",newPosition);
                    //secondIndex = index;
                    //index = index - newPosition;
                    //FormatingString += LastKnownLocation.Substring(newPosition, index);

                    //FormatingString += "\n";
                    //FormatingString += LastKnownLocation.Substring(secondIndex);

                    return latlon;
                }
                else
                {
                    return null;
                }



            }

            catch (System.Net.WebException ex)
            {
                Toast.MakeText(Application.Context, "Network is unreachable", ToastLength.Short).Show();
                return null;
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
                Coordinates latlon = new Coordinates();
                HttpClient client = new HttpClient();
                client.BaseAddress = ServerRootURL;
                client.DefaultRequestHeaders.Add("Accept", "application/json");

                string requestJson = JsonConvert.SerializeObject(longlat);
                StringContent requestContent = new StringContent(requestJson);
                requestContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                //HttpResponseMessage response = await client.PostAsync("/webservice/api/location/tracelastlocation", requestContent);
                HttpResponseMessage response = await client.PostAsync("/api/location/tracelastlocation", requestContent);


                if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    return null;
                }

                if (response.IsSuccessStatusCode)
                {
                    string lastLocation = await response.Content.ReadAsStringAsync();
                    latlon = JsonConvert.DeserializeObject<Coordinates>(lastLocation);

                    return latlon;
                }
                else
                {
                    return null;
                }
            }

            catch (System.Net.WebException ex)
            {
                Toast.MakeText(Application.Context, "Network is unreachable", ToastLength.Short).Show();
                return null;
            }

            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Check Wether shared preference id exisat in our data
        /// </summary>
        /// <param name="longlat"></param>
        /// <returns></returns>
        //async public static Task<Coordinates> SharedPreferenceIdAuthen(Coordinates longlat)
        //{
        //    try
        //    {
        //        Coordinates latlon = new Coordinates();
        //        HttpClient client = new HttpClient();
        //        client.BaseAddress = ServerRootURL;
        //        client.DefaultRequestHeaders.Add("Accept", "application/json");

        //        string requestJson = JsonConvert.SerializeObject(longlat);
        //        StringContent requestContent = new StringContent(requestJson);
        //        requestContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

        //        HttpResponseMessage response = await client.PostAsync("/webservice/api/location/tracelastlocation", requestContent);

        //        if (response.IsSuccessStatusCode)
        //        {
        //            string lastLocation = await response.Content.ReadAsStringAsync();
        //            latlon = JsonConvert.DeserializeObject<Coordinates>(lastLocation);

        //            return latlon;
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}

    }
}