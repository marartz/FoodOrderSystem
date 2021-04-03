namespace Gastromio.Core.Domain.Model.Restaurants
{
    public class Address
    {
        public Address(string street, string zipCode, string city)
        {
            Street = street;
            ZipCode = zipCode;
            City = city;
        }

        public string Street { get; }

        public string ZipCode { get; }

        public string City { get; }
    }
}
