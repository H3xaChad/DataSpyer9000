﻿using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FairDataGetter.Server.Models {
    public class Address {
        
        [Key]
        public int Id { get; init; }

        [Required]
        [MaxLength(100)]
        public required string Street { get; init; }

        [Required]
        [MaxLength(16)]
        public required string HouseNumber { get; init; }

        [Required]
        [MaxLength(50)]
        public required string City { get; init; }

        [Required]
        [MaxLength(10)]
        public required string PostalCode { get; init; }

        [Required]
        [MaxLength(50)]
        public required string Country { get; init; }
    }
}