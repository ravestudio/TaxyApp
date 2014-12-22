using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxyApp.Core.Entities
{
    public class User: Entity<int>
    {
        public string token { get; set; }
        public string pin { get; set; }

        public override void ReadData(Windows.Data.Json.JsonObject jsonObj)
        {
            this.Id = int.Parse(jsonObj["response"].GetObject()["idpassenger"].GetString());
            this.token = jsonObj["response"].GetObject()["token"].GetString();
            //this.pin = jsonObj["response"].GetObject()["pin"].GetString();

            base.ReadData(jsonObj);
        }
    }
}
