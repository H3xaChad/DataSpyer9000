using System.ComponentModel.DataAnnotations;

namespace FairDataGetter.Server.Models;

public class CompanyPostDto {
    
    [Required]
    [MaxLength(42)]
    public required string Name { get; init; }

    [Required]
    public required int AddressId { get; init; }

    public Company ToCompany(Address address) {
        return new Company {
            Name = Name,
            AddressId = AddressId,
            Address = address
        };
    }
}