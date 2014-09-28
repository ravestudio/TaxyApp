using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxyApp.DataModel
{
    public class Session : Singleton<Session>
    {
        private User user = null;

        public User GetUser()
        {
            return this.user;
        }

        public void SetUSer(User user)
        {
            this.user = user;
        }
    }
}
