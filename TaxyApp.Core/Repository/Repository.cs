using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxyApp.Core.Repository
{
    public class Repository<G>
        where G : class
    {
        public virtual G GetById(int id)
        {
            return null;
        }

        public virtual IEnumerable<G> GetAll()
        {
            return null;
        }
        public virtual void Create(G model)
        {

        }
    }
}
