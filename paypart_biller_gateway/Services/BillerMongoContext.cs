using Microsoft.Extensions.Options;
using MongoDB.Driver;
using paypart_biller_gateway.Models;

namespace paypart_biller_gateway.Services
{
    public class BillerMongoContext
    {
        private readonly IMongoDatabase _database = null;

        public BillerMongoContext(IOptions<Settings> settings)
        {
            var client = new MongoClient(settings.Value.connectionString);
            if (client != null)
                _database = client.GetDatabase(settings.Value.database);
        }

        public IMongoCollection<Biller> Billers
        {
            get
            {
                return _database.GetCollection<Biller>("billers");
            }
        }
    }
}
