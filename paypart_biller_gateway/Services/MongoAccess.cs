using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using paypart_biller_gateway.Models;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Text;

namespace paypart_biller_gateway.Services
{
    public class MongoAccess
    {
        public string collection;
        IOptions<Settings> settings;

        public MongoAccess(string _collection, IOptions<Settings> _settings)
        {
            collection = _collection;
            settings = _settings;
        }

        public async Task post(Biller billers)
        {
            string content_type = "application/json";
            string response = string.Empty;
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Collection", "billers");
                    string request = JsonHelper.toJson(billers);
                    var content = new StringContent(request, Encoding.UTF8, content_type);
                    //var result = await client.PostAsync(settings.Value.mongo_url, content);
                    //response = await result.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
