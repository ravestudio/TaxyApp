using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;

namespace TaxyApp.Core
{
    public class WebApiClient
    {
        public async Task<string> GetData(string url, List<KeyValuePair<string, string>> data)
        {

            string res = string.Empty;

            //byte[] dataStream = Encoding.UTF8.GetBytes(data);

            var uri = new Uri(url);
            System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient();
            System.Net.Http.HttpContent content = new System.Net.Http.FormUrlEncodedContent(data);

            //httpClient.PostAsync(uri, content);

            string text = string.Empty;

            try
            {
                System.Net.Http.HttpResponseMessage response = await httpClient.PostAsync(uri, content);

                text = await response.Content.ReadAsStringAsync();
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
            }

            return text;
        }

        public T GetObject<T>(string jsonStr)
            where T : class, Entities.IEntity, new()
        {
            T obj = null;

            var jsonObj = Windows.Data.Json.JsonValue.Parse(jsonStr).GetObject();

            obj = new T();
            obj.ReadData(jsonObj);

            return obj;
        }

    }
}
