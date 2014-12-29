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
        private ObservableCollection<OrderItem> _orderItemList = null;
        private ObservableCollection<OrderService> _orderServiceList = null;

        public Windows.UI.Core.CoreDispatcher Dispatcher { get; set; }

        public Windows.UI.Xaml.Controls.Primitives.Popup ServicePopup { get; set; }
        public Windows.UI.Xaml.Controls.Primitives.Popup DateTimePopup { get; set; }

        public DateTime EndDate { get; set; }
        public DateTime EndTime { get; set; }

        public OrderDetail()
        {
            InitServiceList();

            this.EndDate = DateTime.Now;
            this.EndTime = DateTime.Now;

            this._orderItemList = new ObservableCollection<OrderItem>();

            OrderPoint pointfrom = new OrderPoint();
            pointfrom.Priority = 0;
            pointfrom.Title = "Address from";
            pointfrom.Location = new LocationItem() {  Address = "Input address"};

            OrderPoint pointSecond = new OrderPoint();
            pointSecond.Priority = 1;
            pointSecond.Title = "Address";
            pointSecond.Location = new LocationItem() { Address = "Input address" };

            this._orderItemList.Add(pointfrom);
            this._orderItemList.Add(pointSecond);

            this._orderItemList.Add(new OrderItem()
                {
                    Priority = 10,
                    Title = "Now",
                    Cmd = "Now"
                });

            this._orderItemList.Add(new OrderItem()
            {
                Priority = 11,
                Title = "Services",
                Cmd = "Services"
            });

            this.MapRouteChanged += OrderModel_MapRouteChanged;
        }

        private void InitServiceList()
        {
            this._orderServiceList = new ObservableCollection<OrderService>();
            this._orderServiceList.Add(new OrderService() { id = 1, Name = "Багаж", Checked = false });
            this._orderServiceList.Add(new OrderService() { id = 2, Name = "Можно курить", Checked = false });
            this._orderServiceList.Add(new OrderService() { id = 4, Name = "Водитель не курит", Checked = false });
            this._orderServiceList.Add(new OrderService() { id = 8, Name = "Детское кресло", Checked = false });
            this._orderServiceList.Add(new OrderService() { id = 16, Name = "Удобства для инвалидов", Checked = false });
            this._orderServiceList.Add(new OrderService() { id = 32, Name = "Перевозка животных ", Checked = false });
        }

        void OrderModel_MapRouteChanged(object sender, EventArgs e)
        {
            int thread = Environment.CurrentManagedThreadId;

            this.ShowRoute();

        }

        public void ShowRoute()
        {
            if (this.MapRoute != null)
            {
                Core.Managers.MapPainter painter = Core.Managers.ManagerFactory.Instance.GetMapPainter();

                Task showRoutTask = painter.ShowRoute(this.RouteMapControl, this.MapRoute).ContinueWith(t =>
                {
                    string msg = "route showed";
                });
            }
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

        public ObservableCollection<OrderItem> OrderItemList
        {
            get
            {
                return this._orderItemList;
            }
        }

        public ObservableCollection<OrderService> OrderServiceList
        {
            get
            {
                return this._orderServiceList;
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

            if (this._orderItemList.OfType<OrderPoint>().Count() == this._orderItemList.OfType<OrderPoint>().Where(p => p.IsDataReady()).Count())
            {
                OrderPoint newPoint = new OrderPoint();
                newPoint.Priority = this._orderItemList.OfType<OrderPoint>().Count();
                newPoint.Title = string.Format("Address {0}", newPoint.Priority);
                newPoint.Location = new LocationItem() {  Address = string.Empty};

                this._orderItemList.Add(newPoint);

                List<OrderItem> sortedList = this.OrderItemList.OrderBy(i => i.Priority).ToList();

                this.OrderItemList.Clear();

                foreach (OrderItem item in sortedList)
                {
                    this.OrderItemList.Add(item);
                }
            }
        }

        public async Task<Windows.Services.Maps.MapRoute> FindRoute()
        {
            Windows.Services.Maps.MapRoute route = null;

            int thread = Environment.CurrentManagedThreadId;

            Managers.LocationManager locationMG = Managers.ManagerFactory.Instance.GetLocationManager();

            IEnumerable<Geopoint> geopoints = this._orderItemList.OfType<OrderPoint>().Where(p => p.IsDataReady())
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

        public async Task ShowMyPossitionAsync()
        {
            Managers.MapPainter painter = Managers.ManagerFactory.Instance.GetMapPainter();
            await painter.ShowMyPossitionAsync(this.RouteMapControl);
        }

        public void ShowServices()
        {
            this.ServicePopup.IsOpen = true;
        }

        public void ShowDateTime()
        {
            this.DateTimePopup.IsOpen = true;
        }



        public List<KeyValuePair<string, string>> ConverToKeyValue()
        {
            List<KeyValuePair<string, string>> keyValueData = new List<KeyValuePair<string, string>>();

            //keyValueData.Add(new KeyValuePair<string, string>("enddate", System.DateTime.Now.AddHours(1).ToString("yyyy-mm-dd hh:mm")));

            //keyValueData.Add(new KeyValuePair<string, string>("enddate", System.DateTime.Now.AddHours(1).ToString("yyyy-MM-dd")));

            //keyValueData.Add(new KeyValuePair<string, string>("service", "1023"));
            //keyValueData.Add(new KeyValuePair<string, string>("passengersnum", "3"));

            byte servieces = 0;

            foreach(OrderService service in this.OrderServiceList)
            {
                if (service.Checked)
                {
                    servieces = (byte)((byte)servieces | (byte)service.id);
                }
            }

            int i = 0;
            foreach(OrderPoint orderPoint in this._orderItemList.OfType<OrderPoint>().Where(p => p.IsDataReady()))
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

            if (servieces > 0)
            {
                keyValueData.Add(new KeyValuePair<string, string>("service", servieces.ToString()));
            }

            if (this.MapRoute != null)
            {
                keyValueData.Add(new KeyValuePair<string, string>("routemeters", this.MapRoute.LengthInMeters.ToString()));
                keyValueData.Add(new KeyValuePair<string, string>("routetime", this.MapRoute.EstimatedDuration.Minutes.ToString()));
            }

            //YYYY-MM-DD HH:II
            string enddate = string.Format("{0}-{1}-{2} {3}:{4}", this.EndDate.Year, this.EndDate.Month,this.EndDate.Day, this.EndTime.Hour, this.EndTime.Minute);

            keyValueData.Add(new KeyValuePair<string, string>("enddate", enddate));

            return keyValueData;
        }
    }
}
