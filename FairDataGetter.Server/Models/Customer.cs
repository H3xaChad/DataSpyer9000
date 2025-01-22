using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FairDataGetter.Server.Models {
    public class Customer {
        
        [Key]
        public int Id { get; init; }

        [Required]
        [MaxLength(42)]
        public required string FirstName { get; init; }

        [Required]
        [MaxLength(42)]
        public required string LastName { get; init; }

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public required string Email { get; init; }

        [Required]
        [MaxLength(100)]
        public required string ImageFileName { get; init; }
        
        [Required]
        public int AddressId { get; init; }

        [ForeignKey(nameof(AddressId))]
        public required Address Address { get; init; }

        [Required]
        public required IList<string> InterestedProductGroups { get; init; } = new List<string>();
        
        public int? CompanyId { get; init; }

        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; init; }
    }
}
