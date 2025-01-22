using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FairDataGetter.Server.Models {
    public class Company {
        
        [Key]
        public int Id { get; init; }

        [Required]
        [MaxLength(42)]
        public required string Name { get; init; }
        
        [JsonIgnore]
        [Required]
        public int AddressId { get; init; }
        
        [ForeignKey(nameof(AddressId))]
        public required Address Address { get; init; }
    }
}