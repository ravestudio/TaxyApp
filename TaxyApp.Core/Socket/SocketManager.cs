using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxyApp.Core.Socket
{
    public class SocketManager
    {
        private SocketClient client = null;

        public SocketManager(SocketClient client)
        {
            this.client = client;
        }

        public Task Auth()
        {
            //SocketRequest request = new SocketRequest();
            //request.client_type = "passenger";
            //request.clientid = TaxyApp.Core.Session.Instance.GetUser().Id;
            //request.token = TaxyApp.Core.Session.Instance.GetUser().token;
            //request.request="auth";

            Windows.Data.Json.JsonObject json = new Windows.Data.Json.JsonObject();
            json.Add("client_type", Windows.Data.Json.JsonValue.CreateStringValue("passenger"));
            json.Add("clientid", Windows.Data.Json.JsonValue.CreateStringValue(TaxyApp.Core.Session.Instance.GetUser().Id.ToString()));
            json.Add("token", Windows.Data.Json.JsonValue.CreateStringValue(TaxyApp.Core.Session.Instance.GetUser().token));
            json.Add("request", Windows.Data.Json.JsonValue.CreateStringValue("auth"));
            string msg = json.Stringify();

            Task send = client.SendAsync(msg);

            return send;
        }

        public void Start()
        {
            client.StrartListen();
        }

        public Task Read()
        {
            Task read = client.ReceiveDataAsync();

            return read;
        }

    }
}
