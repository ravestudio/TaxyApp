using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxiApp.DataModel
{
    public class LoginModel
    {
        public string PhoneNumber { get; set; }
        public string PIN { get; set; }

        public LoginModel()
        {
            this.PhoneNumber = string.Empty;
            this.PIN = string.Empty;

            this.ReadData();
        }

        public void SaveData()
        {
            Windows.Storage.ApplicationData.Current.LocalSettings.Values["pin"] = this.PIN;
        }

        public void ReadData()
        {
            object phone = Windows.Storage.ApplicationData.Current.LocalSettings.Values["PhoneNumber"];
            object pin = Windows.Storage.ApplicationData.Current.LocalSettings.Values["pin"];

            if (phone != null)
            {
                this.PhoneNumber = Windows.Storage.ApplicationData.Current.LocalSettings.Values["PhoneNumber"].ToString();
            }

            if (pin != null)
            {
                this.PIN = Windows.Storage.ApplicationData.Current.LocalSettings.Values["pin"].ToString();
            }
        }
    }
}
