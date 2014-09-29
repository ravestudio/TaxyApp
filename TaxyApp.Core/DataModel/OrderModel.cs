using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.ObjectModel;

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
            //pointfrom.Location = new LocationItem() {  Address = "Омск, Жуковского 33"};

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
    }

    public class SetLocationCommand : System.Windows.Input.ICommand
    {

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            Windows.UI.Xaml.Input.TappedRoutedEventArgs e = (Windows.UI.Xaml.Input.TappedRoutedEventArgs)parameter;

            var element = e.OriginalSource as Windows.UI.Xaml.FrameworkElement;

            OrderPoint orderPoint = (OrderPoint)element.DataContext;

        }
    }
}
