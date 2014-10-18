using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxyApp.Core.DataModel.Order
{
    public class OrderPoint
    {
        public int Priority { get; set; }
        public LocationItem Location { get; set; }

        private IDictionary<int, string> orderString = null;

        public OrderPoint()
        {
            orderString = new Dictionary<int, string>();
            orderString.Add(0, "From");
            orderString.Add(1, "Second");
            orderString.Add(2, "Third");
            orderString.Add(3, "Fourth ");

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

            res = string.Format("{0} address",  this.orderString[this.Priority]);            

            return res;
        }

        public bool IsDataReady()
        {
            bool ret = (this.Location != null && this.Location.Ready);
            return ret;
        }
    }
}
