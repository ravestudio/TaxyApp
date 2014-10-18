using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxyApp.Core.Entities
{
    public class Order: Entity<int>
    {
        public DateTime StartDate { get; set; }

        public IList<OrderRouteItem> route = new List<OrderRouteItem>();

        public IList<OrderRouteItem> Route
        {
            get
            {
                return this.route;
            }
        }

        public string Title
        {
            get
            {
                return string.Format("Order {0}", this.StartDate);
            }
        }

        public override void ReadData(Windows.Data.Json.JsonObject jsonObj)
        {
            var type = jsonObj["idorder"].ValueType;

            this.Id = (int)jsonObj["idorder"].GetNumber();

            this.StartDate = DateTime.Parse(jsonObj["startdate"].GetString(), System.Globalization.CultureInfo.InvariantCulture);

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

                this.route.Add(routeItem);
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
