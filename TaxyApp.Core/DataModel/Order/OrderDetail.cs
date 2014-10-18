using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.ObjectModel;

using Windows.Devices.Geolocation;

namespace TaxyApp.Core.DataModel.Order
{
    public class OrderDetail
    {
        private ObservableCollection<OrderPoint> _orderPointList = null;

        public Windows.UI.Core.CoreDispatcher Dispatcher { get; set; }

        public OrderDetail()
        {
            this._orderPointList = new ObservableCollection<OrderPoint>();

            OrderPoint pointfrom = new OrderPoint();
            pointfrom.Priority = 0;
            pointfrom.Location = new LocationItem() {  Address = "Input address"};

            OrderPoint pointSecond = new OrderPoint();
            pointSecond.Priority = 1;
            pointSecond.Location = new LocationItem() { Address = "Input address" };

            this._orderPointList.Add(pointfrom);
            this._orderPointList.Add(pointSecond);

            this.MapRouteChanged += OrderModel_MapRouteChanged;
        }

        void OrderModel_MapRouteChanged(object sender, EventArgs e)
        {
            int thread = Environment.CurrentManagedThreadId;

            Task showRoutTask = this.ShowRoute().ContinueWith(t =>
                {
                    string msg = "route showed";
                });

        }

        public Windows.UI.Xaml.Controls.Maps.MapControl RouteMapControl { get; set; }

        private Windows.Services.Maps.MapRoute mapRoute = null;
        public Windows.Services.Maps.MapRoute MapRoute
        {
            get
            { return this.mapRoute; }
            set
            {
                this.mapRoute = value;

                NotifyMapRouteChanged();
            }
        }

        public event EventHandler MapRouteChanged;

        public void NotifyMapRouteChanged()
        {
            if (MapRouteChanged != null)
            {
                EventArgs args = new EventArgs();
                MapRouteChanged(this, args);
            }
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
            Task<Windows.Services.Maps.MapRoute> FindRouteTask = this.FindRoute();

            FindRouteTask.ContinueWith(t =>
            {
                if (t.Status == TaskStatus.RanToCompletion)
                {
                    
                    this.MapRoute = t.Result;

                    Windows.Foundation.IAsyncAction action =
                    this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        var dlg = new Windows.UI.Popups.MessageDialog("Маршрут найден");
                        dlg.ShowAsync();
                    });
                }
                else
                {

                    Windows.Foundation.IAsyncAction action =
                    this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        var dlg = new Windows.UI.Popups.MessageDialog("Ошибка при поиске маршрута");
                        dlg.ShowAsync();
                    });
                }
            });

            if (this._orderPointList.Count == this._orderPointList.Where(p => p.IsDataReady()).Count())
            {
                OrderPoint newPoint = new OrderPoint();
                newPoint.Priority = this._orderPointList.Count;

                newPoint.Location = new LocationItem() {  Address = string.Empty};

                this._orderPointList.Add(newPoint);
            }
        }

        public async Task<Windows.Services.Maps.MapRoute> FindRoute()
        {
            Windows.Services.Maps.MapRoute route = null;

            int thread = Environment.CurrentManagedThreadId;

            Managers.LocationManager locationMG = Managers.ManagerFactory.Instance.GetLocationManager();

            IEnumerable<Geopoint> geopoints = this._orderPointList.Where(p => p.IsDataReady())
                .OrderBy(p => p.Priority)
                .Select(p => new Geopoint(new BasicGeoposition()
                    {
                        Latitude = p.Location.Latitude,
                        Longitude = p.Location.Longitude
                    }));

            if (geopoints.Count() > 1)
            {
                Windows.Services.Maps.MapRouteFinderResult routeResult = null;
                Task<Windows.Services.Maps.MapRouteFinderResult> routeTask = locationMG.GetRoute(geopoints);

                routeResult = await routeTask;

                if (routeResult.Status == Windows.Services.Maps.MapRouteFinderStatus.Success)
                {
                    route = routeResult.Route;
                }

            }

            return route;
        }

        public async Task ShowRoute()
        {
            if (this.MapRoute != null)
            {
                int thread = Environment.CurrentManagedThreadId;

                Windows.Foundation.IAsyncAction action =
                this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
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
                });

                await action;
            }
        }

        public async Task ShowMyPossitionAsync()
        {
            TaxyApp.Core.Managers.LocationManager locationMG = TaxyApp.Core.Managers.ManagerFactory.Instance.GetLocationManager();

            Geopoint myGeopoint = await locationMG.GetCurrentGeopoint();

            this.RouteMapControl.Center = myGeopoint;

            this.RouteMapControl.ZoomLevel = 12;
            this.RouteMapControl.LandmarksVisible = true;

            AddMapIcon(myGeopoint);
        }

        private void AddMapIcon(Geopoint point)
        {
            Windows.UI.Xaml.Shapes.Ellipse fence = new Windows.UI.Xaml.Shapes.Ellipse();
            fence.Fill = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(255, 50, 120, 90));

            fence.Width = 15;
            fence.Height = 15;

            //MapIcon MapIcon1 = new MapIcon();
            //MapIcon1.Title = "Space Needle";

            var childObj = new Windows.UI.Xaml.Controls.Image
            {
                Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/point.png"))
            };

            Windows.UI.Xaml.Controls.Maps.MapControl.SetLocation(fence, point);
            Windows.UI.Xaml.Controls.Maps.MapControl.SetNormalizedAnchorPoint(fence, new Windows.Foundation.Point(0.5, 0.5));

            RouteMapControl.Children.Add(fence);
        }

        public List<KeyValuePair<string, string>> ConverToKeyValue()
        {
            List<KeyValuePair<string, string>> keyValueData = new List<KeyValuePair<string, string>>();

            //keyValueData.Add(new KeyValuePair<string, string>("enddate", System.DateTime.Now.AddHours(1).ToString("yyyy-mm-dd hh:mm")));

            //keyValueData.Add(new KeyValuePair<string, string>("enddate", System.DateTime.Now.AddHours(1).ToString("yyyy-MM-dd")));

            //keyValueData.Add(new KeyValuePair<string, string>("service", "1023"));
            //keyValueData.Add(new KeyValuePair<string, string>("passengersnum", "3"));

            int i = 0;
            foreach(OrderPoint orderPoint in this._orderPointList.Where(p => p.IsDataReady()))
            {
                string addr = 
                        string.Format("{0}, {1}, {2} {3}",
                        orderPoint.Location.MapLocation.Address.Country,
                        orderPoint.Location.MapLocation.Address.Town,
                        orderPoint.Location.MapLocation.Address.Street,
                        orderPoint.Location.MapLocation.Address.StreetNumber
                        );

                //var utf8 = System.Text.Encoding.UTF8;
                //byte[] utfBytes = utf8.GetBytes(addr);
                //addr = utf8.GetString(utfBytes, 0, utfBytes.Length);

                keyValueData.Add(new KeyValuePair<string, string>
                    (string.Format("address[{0}]",i),addr));

                keyValueData.Add(new KeyValuePair<string, string>
                    (string.Format("coords[{0}]", i), string.Format("{0},{1}",
                    orderPoint.Location.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture),
                    orderPoint.Location.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture))));

                keyValueData.Add(new KeyValuePair<string, string>
                    (string.Format("priority[{0}]",i), orderPoint.Priority.ToString()));

                i++;
            }

            if (this.MapRoute != null)
            {
                keyValueData.Add(new KeyValuePair<string, string>("routemeters", this.MapRoute.LengthInMeters.ToString()));
                keyValueData.Add(new KeyValuePair<string, string>("routetime", this.MapRoute.EstimatedDuration.Minutes.ToString()));
            }

            return keyValueData;
        }
    }
}
