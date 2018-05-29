using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using paypart_biller_gateway.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace paypart_biller_gateway.Services
{
    public class BillerMongoRepository : IBillerMongoRepository
    {
        private readonly BillerMongoContext _context = null;

        public BillerMongoRepository(IOptions<Settings> settings)
        {
            _context = new BillerMongoContext(settings);
        }

        public async Task<IEnumerable<Biller>> GetAllBillers()
        {
            return await _context.Billers.Find(_ => true).ToListAsync();
        }

        public async Task<Biller> GetBiller(int id)
        {
            var filter = Builders<Biller>.Filter.Eq("id", id);
            return await _context.Billers
                                 .Find(filter)
                                 .FirstOrDefaultAsync();
        }

        public async Task<List<Biller>> GetBillers(string category_id)
        {
            var filter = Builders<Biller>.Filter.Eq(b => b.category_id, category_id);
            return await _context.Billers
                                 .Find(filter)
                                 .ToListAsync();
        }

        public async Task<Biller> AddBiller(Biller item)
        {
            await _context.Billers.InsertOneAsync(item);
            return await GetBiller(item.id);
        }

        public async Task<DeleteResult> RemoveBiller(int id)
        {
            return await _context.Billers.DeleteOneAsync(
                         Builders<Biller>.Filter.Eq(s => s.id, id));
        }

        public async Task<UpdateResult> UpdateBiller(int id, string title)
        {
            var filter = Builders<Biller>.Filter.Eq(s => s.id, id);
            var update = Builders<Biller>.Update
                                .Set(s => s.title, title)
                                .CurrentDate(s => s.created_on);
            return await _context.Billers.UpdateOneAsync(filter, update);
        }

        public async Task<ReplaceOneResult> UpdateBiller(int id, Biller item)
        {
            return await _context.Billers
                                 .ReplaceOneAsync(n => n.id.Equals(id)
                                                     , item
                                                     , new UpdateOptions { IsUpsert = true });
        }

        public async Task<DeleteResult> RemoveAllBillers()
        {
            return await _context.Billers.DeleteManyAsync(new BsonDocument());
        }

    }
}
