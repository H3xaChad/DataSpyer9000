using FairDataGetter.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace FairDataGetter.Server.Data {
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options) {
        
        public DbSet<Address> Addresses { get; init; }
        public DbSet<Company> Companies { get; init; }
        public DbSet<Customer> Customers { get; init; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {

            modelBuilder.Entity<Company>()
                .HasOne(c => c.Address)
                .WithMany() // No navigation property in Address
                .HasForeignKey(c => c.AddressId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deletion of an Address in use
            
            modelBuilder.Entity<Customer>()
                .HasOne(c => c.Address)
                .WithMany() // No navigation property in Address
                .HasForeignKey(c => c.AddressId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deletion of an Address in use

            modelBuilder.Entity<Customer>()
                .HasOne(c => c.Company)
                .WithMany() // No navigation property in Company
                .HasForeignKey(c => c.CompanyId)
                .OnDelete(DeleteBehavior.SetNull); // Allow Company to be null and prevent cascading delete
        }
    }
}