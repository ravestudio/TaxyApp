using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxyApp.Core
{
    public class Session : Singleton<Session>
    {
        private DataModel.User user = null;

        public DataModel.User GetUser()
        {
            return this.user;
        }

        public void SetUSer(DataModel.User user)
        {
            this.user = user;
        }
    }
}
