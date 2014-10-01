using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxyApp.Core.DataModel
{
    public class Entity
    {
        public Entity()
        {

        }

        public virtual void ReadData(Windows.Data.Json.JsonObject jsonObj)
        {

        }
    }

    public class User : Entity
    {
        public int passengerid { get; set; }
        public string token { get; set; }
        public string pin { get; set; }

        public override void ReadData(Windows.Data.Json.JsonObject jsonObj)
        {
            this.passengerid = int.Parse(jsonObj["response"].GetObject()["passengerid"].GetString());
            this.token = jsonObj["response"].GetObject()["token"].GetString();
            this.pin = jsonObj["response"].GetObject()["pin"].GetString();

            base.ReadData(jsonObj);
        }


    }
}
