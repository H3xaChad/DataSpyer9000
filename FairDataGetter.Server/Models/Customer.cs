// Models/User.cs
using System.ComponentModel.DataAnnotations;

namespace FairDataGetter.Server.Models {
    public class Customer {
        [Key]
        public int Id { get; set; }

        [MaxLength(42)]
        public required string FirstName { get; set; }

        [MaxLength(42)]
        public required string LastName { get; set; }

        [EmailAddress]
        [MaxLength(100)]
        public required string Email { get; set; }

        public required string Image { get; set; }
    }
}
