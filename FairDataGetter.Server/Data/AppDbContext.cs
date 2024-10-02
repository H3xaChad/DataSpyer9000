using FairDataGetter.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace FairDataGetter.Server.Data {
    public class AppDbContext : DbContext {
        public DbSet<Customer> Customers { get; set; }

        // Constructor
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);
        }
    }
}