using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxyApp.Core.Entities
{
    public class Order: Entity<int>
    {
        IList<OrderRouteItem> Route = new List<OrderRouteItem>();
    }

    public class OrderRouteItem
    {

    }
}
