using MongoDB.Driver;
using System.Collections.Generic;
using paypart_biller_gateway.Models;
using System.Threading.Tasks;

namespace paypart_biller_gateway.Services
{
    public interface IBillerMongoRepository
    {
        Task<IEnumerable<Biller>> GetAllBillers();
        Task<Biller> GetBiller(int id);
        Task<List<Biller>> GetBillers(string category_id);
        Task<Biller> AddBiller(Biller item);
        Task<DeleteResult> RemoveBiller(int id);
        Task<UpdateResult> UpdateBiller(int id, string title);

        // demo interface - full document update
        Task<ReplaceOneResult> UpdateBiller(int id, Biller item);

        // should be used with high cautious, only in relation with demo setup
        Task<DeleteResult> RemoveAllBillers();

    }
}
