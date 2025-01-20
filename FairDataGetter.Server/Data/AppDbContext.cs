using FairDataGetter.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace FairDataGetter.Server.Data {
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options) {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<ProductGroup> ProductGroups { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>()
                .HasOne<Company>()
                .WithMany() // No navigation property in Company
                .HasForeignKey(c => c.CompanyId)
                .OnDelete(DeleteBehavior.SetNull); // Optional: Set to null if Company is deleted

            modelBuilder.Entity<Company>()
                .HasOne<Address>()
                .WithMany() // No navigation property in Address
                .HasForeignKey(c => c.AddressId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deletion of an Address in use

            // Many-to-many relationship between Customer and ProductGroup
            modelBuilder.Entity<Customer>()
                .HasMany(c => c.InterestedProductGroups)
                .WithMany() // No navigation property in ProductGroup
                .UsingEntity(j => j.ToTable("CustomerProductGroups")); // Explicitly specify the join table name

        }
    }
}