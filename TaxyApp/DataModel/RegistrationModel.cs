using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxyApp.DataModel
{
    public class RegistrationModel
    {
        public string PhoneNumber { get; set; }

        public RegistrationModel()
        {
            this.PhoneNumber = "Phone number";

            this.ReadNumber();
        }

        public void SaveNumber()
        {
            Windows.Storage.ApplicationData.Current.LocalSettings.Values["PhoneNumber"] = this.PhoneNumber;
        }

        public void ReadNumber()
        {
            object phone = Windows.Storage.ApplicationData.Current.LocalSettings.Values["PhoneNumber"];

            if (phone != null)
            {
                this.PhoneNumber = Windows.Storage.ApplicationData.Current.LocalSettings.Values["PhoneNumber"].ToString();
            }
        }
    }
}
