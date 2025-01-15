using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairDataGetter.Client.Class
{
    public class Address
    {
        public int Id { get; set; }
        public required string Street { get; set; }
        public required string HouseNumber { get; set; }
        public required string City { get; set; }
        public required string PostalCode { get; set; }
        public required string Country { get; set; }
    }

    public class Company
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required Address Address { get; set; }
    }

    public class ProductGroup
    {
        public int Id { get; set; }
        public required string Name { get; set; }
    }

    public class Customer
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string ImagePath { get; set; }
        public required Address Address { get; set; }
        public ICollection<ProductGroup> InterestedProductGroups { get; set; }
        public required bool IsCorporateCustomer { get; set; }

    }
}
