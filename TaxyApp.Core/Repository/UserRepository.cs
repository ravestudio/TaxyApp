using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxyApp.Core.Repository
{
    public class UserRepository: Repository<Entities.User, int>
    {
        public UserRepository(TaxyApp.Core.WebApiClient apiClient) : base(apiClient)
        {

        }

        public async Task<Entities.User> GetUser(string PhoneNumber, string PIN)
        {
            string url = "http://serv.giddix.ru/api/passenger_auth/";

            var postData = new List<KeyValuePair<string, string>>();

            postData.Add(new KeyValuePair<string, string>("phone", PhoneNumber));
            postData.Add(new KeyValuePair<string, string>("pin", PIN));
            postData.Add(new KeyValuePair<string, string>("idcompany", "1"));

            //string data = string.Format("phone={0}&pin={1}&idcompany={2}", model.PhoneNumber, model.PIN, 1);

            string data = await this._apiClient.GetData(url, postData);

            TaxyApp.Core.Entities.User user = this._apiClient.GetObject<TaxyApp.Core.Entities.User>(data);

            return user;
        }
    }
}
