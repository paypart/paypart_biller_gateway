using System.Collections.Generic;
using paypart_biller_gateway.Models;
using System.Threading.Tasks;

namespace paypart_biller_gateway.Services
{
    public interface IBillerSqlServerRepository
    {
        Task<List<Biller>> GetAllBillers();
        Task<Biller> GetBiller(int id);
        Task<List<Biller>> GetBillers(string category_id);
        Task<Biller> AddBiller(Biller item);
        Task<BillerContact> AddBillerContact(BillerContact billercontact);
        Task<Biller> UpdateBiller(Biller biller);
        //Task<DeleteResult> RemoveBiller(string id);
        //Task<UpdateResult> UpdateBiller(string id, string title);

        // demo interface - full document update
        //Task<ReplaceOneResult> UpdateBiller(string id, Biller item);

        // should be used with high cautious, only in relation with demo setup
        //Task<DeleteResult> RemoveAllBillers();

    }
}
