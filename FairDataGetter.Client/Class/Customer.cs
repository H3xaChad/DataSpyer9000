namespace FairDataGetter.Client.Class
{
    public class Customer
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string ImageBase64 { get; set; }
        public required Address Address { get; set; }
        public List<ProductGroup> InterestedProductGroups { get; set; }
        public required bool IsCorporateCustomer { get; set; }

    }
}
