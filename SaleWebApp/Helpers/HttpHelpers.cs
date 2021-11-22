using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using Newtonsoft.Json.Linq;

namespace SaleWebApp.Helpers
{
    public static class HttpHelpers
    {
        public static async Task<(T content, string message)> HttpGetHelper<T>(this HttpClient httpClient, string url, string key)
        {
            var responseJson = (content: default(T), message:"");
            var response = await httpClient.GetAsync(url);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                string responseMessage = await response.Content.ReadAsStringAsync();
                responseJson = JsonObjectToObject<T>(responseMessage, key);
            }
            return responseJson;
        }

        public static async Task<(T content, string message)> HttpPostHelper<T>(this HttpClient httpClient, string url, object value, string key)
        {
            var responseJson = (content: default(T), message: "");
            string jsonValue = JsonConvert.SerializeObject(value);
            using (var content = new StringContent(jsonValue, Encoding.UTF8, "application/json"))
            {
                var response = await httpClient.PostAsync(url, content);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string responseMessage = await response.Content.ReadAsStringAsync();
                    responseJson = JsonObjectToObject<T>(responseMessage, key);
                }
            }
            return responseJson;
        }

        public static async Task<(T content, string message)> HttpPutHelper<T>(this HttpClient httpClient, string url, object value, string key)
        {
            var responseJson = (content: default(T), message: "");
            string jsonValue = JsonConvert.SerializeObject(value);
            using (var content = new StringContent(jsonValue, Encoding.UTF8, "application/json"))
            {
                var response = await httpClient.PutAsync(url, content);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string responseMessage = await response.Content.ReadAsStringAsync();
                    responseJson = JsonObjectToObject<T>(responseMessage, key);
                }
            }
            return responseJson;
        }
        public static async Task<(T content, string message)> HttpDeleteHelper<T>(this HttpClient httpClient, string url, string key)
        {
            var responseJson = (content: default(T), message: "");
            var response = await httpClient.DeleteAsync(url);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                string responseMessage = await response.Content.ReadAsStringAsync();
                responseJson = JsonObjectToObject<T>(responseMessage, key);
            }
            return responseJson;
        }
        public static (T content, string message)  JsonObjectToObject<T>(string jsonString, string key)
        {
            var result = (content: default(T), message: "");
            var jsonObject = JObject.Parse(jsonString);
            if (jsonObject != null)
            {
                result.content = jsonObject[key].ToObject<T>();
                result.message = jsonObject["message"].ToString();
            }
            return result;
        }
}
}
