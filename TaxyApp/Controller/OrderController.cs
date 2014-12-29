using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxyApp.Core.DataModel;
using TaxyApp.Core.DataModel.Order;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace TaxyApp.Controller
{
    public class OrderController
    {
        public OrderDetail OrderModel { get; set; }
        public SearchModel SearchModel { get; set; }
        public SetLocationCommand SetLocation { get; set; }
        public SelectItemCommand SelectItem { get; set; }

        public OrderController()
        {
            this.OrderModel = new OrderDetail();
            this.SearchModel = new SearchModel();

            this.SetLocation = new SetLocationCommand(this);
            this.SelectItem = new SelectItemCommand(this);
        }

        public async void CreateOrder()
        {
            TaxyApp.Core.Entities.User user = TaxyApp.Core.Session.Instance.GetUser();

            var postData = this.OrderModel.ConverToKeyValue();

            //var postData = new List<KeyValuePair<string, string>>();

            postData.Add(new KeyValuePair<string, string>("idpassenger", user.Id.ToString()));
            postData.Add(new KeyValuePair<string, string>("token", user.token));
            postData.Add(new KeyValuePair<string, string>("idcompany", "1"));



            TaxyApp.Core.WebApiClient client = new TaxyApp.Core.WebApiClient();

            string url = "http://serv.giddix.ru/api/passenger_setorder/";

            string data = await client.GetData(url, postData);
        }

        public async Task<IList<TaxyApp.Core.Entities.Order>> GetUserOrders()
        {
            TaxyApp.Core.WebApiClient client = new TaxyApp.Core.WebApiClient();       

            TaxyApp.Core.Entities.User user = TaxyApp.Core.Session.Instance.GetUser();

            TaxyApp.Core.Repository.OrderRepository orderRepository = new Core.Repository.OrderRepository(client);

            IList<TaxyApp.Core.Entities.Order> orderList = await orderRepository.GetUserOrders(user);

            return orderList;
        }
    }

    public class SetLocationCommand : System.Windows.Input.ICommand
    {
        private OrderController _controller = null;

        public SetLocationCommand(OrderController controller)
        {
            this._controller = controller;
        }
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            Windows.UI.Xaml.Controls.ItemClickEventArgs e = (Windows.UI.Xaml.Controls.ItemClickEventArgs)parameter;


            if (e.ClickedItem as TaxyApp.Core.DataModel.Order.OrderPoint != null)
            {
                TaxyApp.Core.DataModel.Order.OrderPoint orderPoint = (TaxyApp.Core.DataModel.Order.OrderPoint)e.ClickedItem;


                Frame rootFrame = Window.Current.Content as Frame;

                _controller.SearchModel.SelectedPoint = orderPoint;

                rootFrame.Navigate(typeof(AddPointPage));

            }
            else
            {
                TaxyApp.Core.DataModel.Order.OrderItem item = (TaxyApp.Core.DataModel.Order.OrderItem)e.ClickedItem;

                if (item.Cmd == "Services")
                {
                    _controller.OrderModel.ShowServices();
                }

                if (item.Cmd == "Now")
                {
                    _controller.OrderModel.ShowDateTime();
                }

            }

        }
    }

    public class SelectItemCommand : System.Windows.Input.ICommand
    {
        private OrderController _controller = null;

        public SelectItemCommand(OrderController controller)
        {
            this._controller = controller;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            Windows.UI.Xaml.Controls.ItemClickEventArgs e = (Windows.UI.Xaml.Controls.ItemClickEventArgs)parameter;

            LocationItem location = (LocationItem)e.ClickedItem;

            _controller.SearchModel.SelectedPoint.Location = location;

            Frame rootFrame = Window.Current.Content as Frame;

            this._controller.OrderModel.UpdatePoints();

            rootFrame.GoBack();
        }
    }
}
