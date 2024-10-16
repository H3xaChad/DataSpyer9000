using FairDataGetter.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace FairDataGetter.Server.Data {
    public class AppDbContext : DbContext {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<ProductGroup> ProductGroups { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);
        }
    }
}