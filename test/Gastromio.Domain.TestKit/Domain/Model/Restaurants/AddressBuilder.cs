using Gastromio.Core.Domain.Model.Restaurants;
using Gastromio.Domain.TestKit.Common;

namespace Gastromio.Domain.TestKit.Domain.Model.Restaurants
{
    public class AddressBuilder : TestObjectBuilderBase<Address>
    {
        public AddressBuilder WithValidConstrains()
        {
            WithStreet("Musterstrasse 1");
            WithZipCode("12345");
            WithCity("Musterstadt");
            return this;
        }

        public AddressBuilder WithStreet(string street)
        {
            WithConstantConstructorArgumentFor("street", street);
            return this;
        }

        public AddressBuilder WithZipCode(string zipCode)
        {
            WithConstantConstructorArgumentFor("zipCode", zipCode);
            return this;
        }

        public AddressBuilder WithCity(string city)
        {
            WithConstantConstructorArgumentFor("city", city);
            return this;
        }

    }
}
