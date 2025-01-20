using System.ComponentModel.DataAnnotations;

namespace FairDataGetter.Server.Models {
    public class Customer {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(42)]
        public required string FirstName { get; set; }

        [Required]
        [MaxLength(42)]
        public required string LastName { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public required string Email { get; set; }

        [Required]
        public required string ImagePath { get; set; }

        [Required]
        public int AddressId { get; set; }

        [Required]
        public required ICollection<ProductGroup> InterestedProductGroups { get; set; } = new List<ProductGroup>();

        // Foreign key to Company (Customer can belong to one Company or none)
        public int? CompanyId { get; set; }

    }
}
