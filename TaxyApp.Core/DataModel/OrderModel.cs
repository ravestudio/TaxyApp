using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace TaxyApp.Core.DataModel
{
    public class OrderModel
    {
        private ObservableCollection<OrderPoint> _orderPointList = null;

        public OrderModel()
        {
            this._orderPointList = new ObservableCollection<OrderPoint>();

            OrderPoint pointfrom = new OrderPoint();
            pointfrom.Priority = 0;
            pointfrom.Location = new LocationItem() {  Address = string.Empty};

            //OrderPoint point1 = new OrderPoint();
            //point1.Priority = 1;
            //point1.Location = new LocationItem() { Address = "Омск, Маркса 89" };

            //OrderPoint point2 = new OrderPoint();
            //point2.Priority = 2;
            //point2.Location = new LocationItem() { Address = "Омск, Учебная 83" };

            //this._orderPointList.Add(pointfrom);
            //this._orderPointList.Add(point1);
            //this._orderPointList.Add(point2);

            this._orderPointList.Add(pointfrom);
        }

        public ObservableCollection<OrderPoint> OrderPointList
        {
            get
            {
                return this._orderPointList;
            }
        }

        public void UpdatePoints()
        {
            if (this._orderPointList.Count == this._orderPointList.Where(p => p.IsDataReady()).Count())
            {
                OrderPoint newPoint = new OrderPoint();
                newPoint.Priority = this._orderPointList.Count;

                newPoint.Location = new LocationItem() {  Address = string.Empty};

                this._orderPointList.Add(newPoint);
            }
        }

        public List<KeyValuePair<string, string>> ConverToKeyValue()
        {
            List<KeyValuePair<string, string>> keyValueData = new List<KeyValuePair<string, string>>();

            keyValueData.Add(new KeyValuePair<string, string>("enddate", System.DateTime.Now.AddHours(1).ToString("yyyy-mm-dd hh:mm")));

            int i = 0;
            foreach(OrderPoint orderPoint in this._orderPointList)
            {
                keyValueData.Add(new KeyValuePair<string, string>
                    (string.Format("address[{0}]",i),
                        string.Format("{0}, {1}, {2} {3}",
                        orderPoint.Location.MapLocation.Address.Country,
                        orderPoint.Location.MapLocation.Address.Town,
                        orderPoint.Location.MapLocation.Address.Street,
                        orderPoint.Location.MapLocation.Address.StreetNumber
                        )));

                keyValueData.Add(new KeyValuePair<string, string>
                    (string.Format("coords[{0}]",i), string.Format("{0} {1}", orderPoint.Location.Point.Position.Latitude, orderPoint.Location.Point.Position.Longitude)));

                keyValueData.Add(new KeyValuePair<string, string>
                    (string.Format("priority[{0}]",i), orderPoint.Priority.ToString()));

                i++;
            }

            return keyValueData;
        }
    }

    public class OrderPoint
    {

        public int Priority { get; set; }
        public LocationItem Location { get; set; }

        public OrderPoint()
        {

        }

        public string PointTitle
        {
            get
            {
                return this.GetPointTitle();
            }
        }

        public string GetPointTitle()
        {
            string res = string.Empty;

            if (this.Priority == 0)
            {
                res = "From";
            }
            else
            {
                res = string.Format("Point {0}", this.Priority);
            }

            return res;
        }

        public bool IsDataReady()
        {
            return (this.Location != null && !string.IsNullOrEmpty(this.Location.Address));
        }
    }

}
