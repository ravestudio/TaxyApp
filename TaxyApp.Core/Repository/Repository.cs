using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxyApp.Core.Repository
{
    public class Repository<G, Key>
        where G : Entities.Entity<Key>, new()
    {

        protected TaxyApp.Core.WebApiClient _apiClient = null;

        public Repository(TaxyApp.Core.WebApiClient apiClient)
        {
            this._apiClient = apiClient;
        }

        public virtual G GetById(Key id)
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

        public G GetObject(Windows.Data.Json.IJsonValue jsonValue)
        {
            G obj = null;

            var jsonObj = jsonValue.GetObject();

            obj = new G();
            obj.ReadData(jsonObj);

            return obj;
        }
    }
}
