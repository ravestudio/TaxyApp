using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxiApp.DataModel
{
    public class SocketModel
    {
        public string Host { get; set; }
        public string Port { get; set; }

        public SocketModel()
        {
            this.Host = "194.58.102.129";
            this.Port = "9090";
        }
    }
}
