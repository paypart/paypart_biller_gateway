using paypart_biller_gateway.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace paypart_biller_gateway.Services
{
    public class BillerSqlServerRepository : IBillerSqlServerRepository
    {
        private readonly BillerSqlServerContext _context = null;

        public BillerSqlServerRepository(BillerSqlServerContext context)
        {
            _context = context;
        }

        public async Task<List<Biller>> GetAllBillers()
        {
            return await _context.Billers.Include(b => b.billercontact).ToListAsync();
        }

        public async Task<Biller> GetBiller(int id)
        {
            return await _context.Billers.Include(b => b.billercontact).Where(c => c.id == id)
                                 .FirstOrDefaultAsync();
        }
        public async Task<BillerContact> GetBillerContact(int id)
        {
            return await _context.BillersContact.Where(c => c.id == id)
                                 .FirstOrDefaultAsync();
        }
        public async Task<List<Biller>> GetBillers(string category_id)
        {
            return await _context.Billers.Where(c => c.category_id == category_id)
                                 .ToListAsync();
        }

        public async Task<Biller> AddBiller(Biller item)
        {
            await _context.Billers.AddAsync(item);
            await _context.SaveChangesAsync();
            return await GetBiller(item.id);
        }
        public async Task<BillerContact> AddBillerContact(BillerContact billercontact)
        {
            await _context.BillersContact.AddAsync(billercontact);
            await _context.SaveChangesAsync();
            return await GetBillerContact(billercontact.id);
        }
        public async Task<Biller> UpdateBiller(Biller biller)
        {
            Biller b = await GetBiller(biller.id);
            BillerContact bc = await GetBillerContact(biller.billercontact.id);
           
            bc.billerid = biller.billercontact.billerid;
            bc.city = biller.billercontact.city;
            bc.countryid = biller.billercontact.countryid;
            bc.email = biller.billercontact.email;
            bc.emailaux = biller.billercontact.emailaux;
            bc.phone = biller.billercontact.phone;
            bc.phoneaux = biller.billercontact.phoneaux;
            bc.postcode = biller.billercontact.postcode;
            bc.stateid = biller.billercontact.stateid;
            bc.street = biller.billercontact.street;

            b.category_id = biller.category_id;
            b.logo = !string.IsNullOrEmpty(biller.logo) ? biller.logo : b.logo;
            b.title = biller.title;

            await _context.SaveChangesAsync();

            b.billercontact = bc;
            return b;
        }
        //public async Task<DeleteResult> RemoveBiller(string id)
        //{
        //    return await _context.Billers.Remove(
        //                 Builders<Biller>.Filter.Eq(s => s._id, id));
        //}

        //public async Task<UpdateResult> UpdateBiller(string id, string title)
        //{
        //    var filter = Builders<Biller>.Filter.Eq(s => s._id.ToString(), id);
        //    var update = Builders<Biller>.Update
        //                        .Set(s => s.title, title)
        //                        .CurrentDate(s => s.createdOn);
        //    return await _context.Billers.UpdateOneAsync(filter, update);
        //}

        //public async Task<ReplaceOneResult> UpdateBiller(string id, Biller item)
        //{
        //    return await _context.Billers
        //                         .ReplaceOneAsync(n => n._id.Equals(id)
        //                                             , item
        //                                             , new UpdateOptions { IsUpsert = true });
        //}

        //public async Task<DeleteResult> RemoveAllBillers()
        //{
        //    return await _context.Billers.DeleteManyAsync(new BsonDocument());
        //}
    }
}
