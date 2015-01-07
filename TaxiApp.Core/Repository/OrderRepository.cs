﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxiApp.Core.Repository
{
    public class OrderRepository : Repository<Entities.Order, int>
    {
        public OrderRepository(TaxiApp.Core.WebApiClient apiClient): base(apiClient)
        {

        }

        public override void Create(Entities.Order order)
        {
            base.Create(order);
        }

        public async Task<IList<Entities.Order>> GetUserOrders(Entities.User user)
        {
            IList<Entities.Order> orderList = new List<Entities.Order>();

            var postData = new List<KeyValuePair<string, string>>();

            postData.Add(new KeyValuePair<string, string>("idpassenger", user.Id.ToString()));
            postData.Add(new KeyValuePair<string, string>("token", user.token));
            postData.Add(new KeyValuePair<string, string>("idcompany", "1"));

            string url = "http://serv.giddix.ru/api/passenger_getmyorders/";

            string data = await this._apiClient.GetData(url, postData);

            var jsonObj = Windows.Data.Json.JsonValue.Parse(data).GetObject();

            var resp = jsonObj["response"].GetObject();
            var orders = resp["orders"];


            if (orders.ValueType == Windows.Data.Json.JsonValueType.Array)
            {
                var orderArray = orders.GetArray();

                for (int i = 0; i < orderArray.Count; i++)
                {
                    var ordderValue = orderArray[i];
                    Entities.Order order = this.GetObject(ordderValue);
                    orderList.Add(order);
                }
            }


            return orderList;
        }
    }
}
