using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxyApp.Core.Repository
{
    public class OrderRepository : Repository<Entities.Order, int>
    {
        public OrderRepository(TaxyApp.Core.WebApiClient apiClient): base(apiClient)
        {

        }

        public override void Create(Entities.Order order)
        {
            base.Create(order);
        }
    }
}
