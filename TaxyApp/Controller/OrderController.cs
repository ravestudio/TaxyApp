﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxyApp.Core.DataModel;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace TaxyApp.Controller
{
    public class OrderController
    {
        public OrderModel OrderModel { get; set; }
        public SearchModel SearchModel { get; set; }
        public SetLocationCommand SetLocation { get; set; }
        public SelectItemCommand SelectItem { get; set; }

        public OrderController()
        {
            this.OrderModel = new OrderModel();
            this.SearchModel = new SearchModel();

            this.SetLocation = new SetLocationCommand(this);
            this.SelectItem = new SelectItemCommand(this);
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
            //Windows.UI.Xaml.Input.TappedRoutedEventArgs e = (Windows.UI.Xaml.Input.TappedRoutedEventArgs)parameter;

            //var element = e.OriginalSource as Windows.UI.Xaml.FrameworkElement;

            OrderPoint orderPoint = (OrderPoint)parameter;

            Frame rootFrame = Window.Current.Content as Frame;

            _controller.SearchModel.SelectedLocation = orderPoint.Location;

            rootFrame.Navigate(typeof(AddPointPage));

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
            Windows.UI.Xaml.Input.TappedRoutedEventArgs e = (Windows.UI.Xaml.Input.TappedRoutedEventArgs)parameter;

            var element = e.OriginalSource as Windows.UI.Xaml.FrameworkElement;

            //Windows.UI.Xaml.Input.TappedRoutedEventArgs e = (Windows.UI.Xaml.Input.TappedRoutedEventArgs)parameter;

            //Windows.UI.Xaml.Controls.Control textblock = (Windows.UI.Xaml.Controls.Control)e.OriginalSource;

            LocationItem location = (LocationItem)element.DataContext;

            _controller.SearchModel.SelectedLocation.Address = location.Address;
            _controller.SearchModel.SelectedLocation.Point = location.Point;

            Frame rootFrame = Window.Current.Content as Frame;

            rootFrame.GoBack();
        }
    }
}
