using System.ComponentModel.DataAnnotations;

namespace FairDataGetter.Server.Models {
    public class ProductGroup {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(42)]
        public required string Name { get; set; }
    }
}