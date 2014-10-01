using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxyApp.Controller
{
    public class ControllerFactory : TaxyApp.Core.Singleton<ControllerFactory>
    {
        private OrderController orderController = null;

        public OrderController GetOrderController()
        {
            if (this.orderController == null)
            {
                this.orderController = new OrderController();
            }

            return this.orderController;
        }
    }
}
