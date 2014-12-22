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
        private AuthenticationController authenticationController = null;

        public OrderController GetOrderController()
        {
            if (this.orderController == null)
            {
                this.orderController = new OrderController();
            }

            return this.orderController;
        }

        public AuthenticationController GetAuthenticationController()
        {
            if (this.authenticationController == null)
            {
                this.authenticationController = new AuthenticationController();
            }

            return this.authenticationController;
        }
    }
}
