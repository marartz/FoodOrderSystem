namespace Gastromio.Core.Domain.Model.Orders
{
    public class CustomerInfo
    {
        public CustomerInfo(
            string givenName,
            string lastName,
            string street,
            string addAddressInfo,
            string zipCode,
            string city,
            string phone,
            string email
        )
        {
            GivenName = givenName;
            LastName = lastName;
            Street = street;
            AddAddressInfo = addAddressInfo;
            ZipCode = zipCode;
            City = city;
            Phone = phone;
            Email = email;
        }

        public string GivenName { get; }
        public string LastName { get; }
        public string Street { get; }
        public string AddAddressInfo { get; }
        public string ZipCode { get; }
        public string City { get; }
        public string Phone { get; }
        public string Email { get; }
    }
}
