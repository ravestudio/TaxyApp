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
            this.PhoneNumber = "79659755758";
            this.PIN = "79787711";
        }
    }
}
