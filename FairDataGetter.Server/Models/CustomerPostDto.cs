using System.ComponentModel.DataAnnotations;

namespace FairDataGetter.Server.Models;

public class CustomerPostDto {
    
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
    public required IFormFile Image { get; init; }

    [Required]
    public int AddressId { get; init; }

    [Required]
    public required IList<string> InterestedProductGroups { get; init; } = new List<string>();

    public int? CompanyId { get; init; }
    
    public Customer ToCustomer(string imageFileName, Address address) {
        return new Customer {
            FirstName = FirstName,
            LastName = LastName,
            Email = Email,
            ImageFileName = imageFileName,
            AddressId = AddressId,
            Address = address,
            InterestedProductGroups = InterestedProductGroups,
            CompanyId = CompanyId
        };
    }
}