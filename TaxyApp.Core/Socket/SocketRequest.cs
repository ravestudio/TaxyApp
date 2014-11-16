using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxyApp.Core.Socket
{
    public class SocketRequest
    {
        public string client_type { get; set; }
        public int clientid { get; set; }
        public string token { get; set; }
        public string request { get; set; }
    }
}
