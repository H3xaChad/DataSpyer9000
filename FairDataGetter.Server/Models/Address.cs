using System.ComponentModel.DataAnnotations;

namespace FairDataGetter.Server.Models {
    public class Address {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public required string Street { get; set; }

        [Required]
        [MaxLength(50)]
        public required string City { get; set; }

        [Required]
        [MaxLength(10)]
        public required string PostalCode { get; set; }

        [Required]
        [MaxLength(50)]
        public required string Country { get; set; }
    }
}
