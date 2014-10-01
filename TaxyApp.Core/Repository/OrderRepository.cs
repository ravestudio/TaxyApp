using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxyApp.Core.Repository
{
    public class OrderRepository : Repository<DataModel.OrderModel>
    {
        public override void Create(DataModel.OrderModel model)
        {
            base.Create(model);
        }
    }
}
