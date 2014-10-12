using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxyApp.Core
{
    public class Session : Singleton<Session>
    {
        private Entities.User user = null;

        public Entities.User GetUser()
        {
            return this.user;
        }

        public void SetUSer(Entities.User user)
        {
            this.user = user;
        }
    }
}
