﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxyApp.DataModel
{
    public class LoginModel
    {
        public string PhoneNumber { get; set; }
        public string PIN { get; set; }

        public LoginModel()
        {
            this.PhoneNumber = "+79136123087";
            this.PIN = "94c0915ab3bcbc61c1c61624dd6d7cd5";
        }
    }
}
