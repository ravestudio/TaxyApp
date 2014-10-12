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

        public bool IsDataReady()
        {
            return (this.Location != null && !string.IsNullOrEmpty(this.Location.Address));
        }
    }
}
