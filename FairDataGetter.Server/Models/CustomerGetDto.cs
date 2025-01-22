namespace FairDataGetter.Server.Models;

public class CustomerGetDto {
    public required int Id { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }
    public required Address Address { get; init; }
    public required IList<string> InterestedProductGroups { get; init; } = new List<string>();
    public Company? Company { get; init; }
    
    public static CustomerGetDto FromCustomer(Customer customer) {
        return new CustomerGetDto {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Email = customer.Email,
            Address = customer.Address,
            InterestedProductGroups = customer.InterestedProductGroups,
            Company = customer.Company
        };
    }
}