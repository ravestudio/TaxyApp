using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.ObjectModel;
using System.Collections.Generic;

using Windows.Devices.Geolocation;

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

        public Windows.UI.Xaml.Controls.Maps.MapControl RouteMapControl { get; set; }
        public Windows.Services.Maps.MapRoute MapRoute { get; set; }

        public ObservableCollection<OrderPoint> OrderPointList
        {
            get
            {
                return this._orderPointList;
            }
        }

        public void UpdatePoints()
        {
            this.UpdateRoute();

            if (this._orderPointList.Count == this._orderPointList.Where(p => p.IsDataReady()).Count())
            {
                OrderPoint newPoint = new OrderPoint();
                newPoint.Priority = this._orderPointList.Count;

                newPoint.Location = new LocationItem() {  Address = string.Empty};

                this._orderPointList.Add(newPoint);
            }
        }

        public async void UpdateRoute()
        {
            Managers.LocationManager locationMG = Managers.ManagerFactory.Instance.GetLocationManager();

            IEnumerable<Geopoint> geopoints = this._orderPointList.Where(p => p.IsDataReady())
                .OrderBy(p => p.Priority)
                .Select(p => p.Location.Point);

            if (geopoints.Count() > 1)
            {

                Windows.Services.Maps.MapRouteFinderResult routeResult = await locationMG.GetRoute(geopoints);

                if (routeResult.Status == Windows.Services.Maps.MapRouteFinderStatus.Success)
                {
                    this.MapRoute = routeResult.Route;

                    Windows.UI.Xaml.Controls.Maps.MapRouteView viewOfRoute = new Windows.UI.Xaml.Controls.Maps.MapRouteView(this.MapRoute);
                    viewOfRoute.RouteColor = Windows.UI.Colors.Yellow;
                    viewOfRoute.OutlineColor = Windows.UI.Colors.Black;

                    // Add the new MapRouteView to the Routes collection
                    // of the MapControl.
                    this.RouteMapControl.Routes.Add(viewOfRoute);

                    // Fit the MapControl to the route.
                    await this.RouteMapControl.TrySetViewBoundsAsync(
                        this.MapRoute.BoundingBox,
                        null,
                        Windows.UI.Xaml.Controls.Maps.MapAnimationKind.None);
                }
            }
        }

        public async Task<List<KeyValuePair<string, string>>> ConverToKeyValue()
        {
            List<KeyValuePair<string, string>> keyValueData = new List<KeyValuePair<string, string>>();

            keyValueData.Add(new KeyValuePair<string, string>("enddate", System.DateTime.Now.AddHours(1).ToString("yyyy-mm-dd hh:mm")));

            int i = 0;
            foreach(OrderPoint orderPoint in this._orderPointList.Where(p => p.IsDataReady()))
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

            //keyValueData.Add(new KeyValuePair<string, string>("service","1"));
            //keyValueData.Add(new KeyValuePair<string, string>("passengersnum", "1"));

            Managers.LocationManager locationMG = Managers.ManagerFactory.Instance.GetLocationManager();

            IEnumerable<Geopoint> geopoints = this._orderPointList.Where(p => p.IsDataReady())
                .OrderBy(p => p.Priority)
                .Select(p => p.Location.Point);

            Windows.Services.Maps.MapRouteFinderResult routeResult = await locationMG.GetRoute(geopoints);

            if (routeResult.Status == Windows.Services.Maps.MapRouteFinderStatus.Success)
            {
                keyValueData.Add(new KeyValuePair<string, string>("routemeters", routeResult.Route.LengthInMeters.ToString()));
                keyValueData.Add(new KeyValuePair<string, string>("routetime", routeResult.Route.EstimatedDuration.Minutes.ToString()));
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
