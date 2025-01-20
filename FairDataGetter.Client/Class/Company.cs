namespace FairDataGetter.Client.Class
{
    public class Company
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required Address Address { get; set; }
    }
}
