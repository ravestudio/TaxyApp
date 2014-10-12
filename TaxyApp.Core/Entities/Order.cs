using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxyApp.Core.Entities
{
    public class Order: Entity<int>
    {
        IList<OrderRouteItem> Route = new List<OrderRouteItem>();

        public override void ReadData(Windows.Data.Json.JsonObject jsonObj)
        {
            var type = jsonObj["idorder"].ValueType;

            this.Id = (int)jsonObj["idorder"].GetNumber();

            var routeArray = jsonObj["routes"].GetArray();

            for (int i = 0; i < routeArray.Count; i++)
            {
                string addr = routeArray[i].GetObject()["address"].GetString();
                string coords = routeArray[i].GetObject()["coords"].GetString();

                coords = coords.Trim();
                string[] coordsArray = coords.Split(',');

                OrderRouteItem routeItem = new OrderRouteItem()
                {
                    Address = addr,

                    Latitude = double.Parse(coordsArray[0], System.Globalization.CultureInfo.InvariantCulture),
                    Longitude = double.Parse(coordsArray[1], System.Globalization.CultureInfo.InvariantCulture)
                };

                this.Route.Add(routeItem);
            }

            base.ReadData(jsonObj);
        }
    }

    

    public class OrderRouteItem
    {
        public string Address { get; set; }

        public double Latitude {get; set; }
        public double Longitude { get; set; }
    }
}
