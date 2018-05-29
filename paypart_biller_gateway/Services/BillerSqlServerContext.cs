using Microsoft.EntityFrameworkCore;
using paypart_biller_gateway.Models;

namespace paypart_biller_gateway.Services
{
    public class BillerSqlServerContext : DbContext
    {
        public BillerSqlServerContext(DbContextOptions<BillerSqlServerContext> options) : base(options)
        {

        }

        public DbSet<Biller> Billers { get; set; }
        public DbSet<BillerCategory> BillersCategory { get; set; }
        public DbSet<BillerContact> BillersContact { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Biller>().ToTable("Biller");
            modelBuilder.Entity<BillerCategory>().ToTable("BillerCategory");
            modelBuilder.Entity<BillerContact>().ToTable("BillerContact");

        }
    }
}
