using Gastromio.Core.Domain.Model.Orders;
using Gastromio.Domain.TestKit.Common;

namespace Gastromio.Domain.TestKit.Domain.Model.Orders
{
    public class CustomerInfoBuilder : TestObjectBuilderBase<CustomerInfo>
    {
        public CustomerInfoBuilder WithGivenName(string givenName)
        {
            WithConstantConstructorArgumentFor("givenName", givenName);
            return this;
        }

        public CustomerInfoBuilder WithLastName(string lastName)
        {
            WithConstantConstructorArgumentFor("lastName", lastName);
            return this;
        }

        public CustomerInfoBuilder WithStreet(string street)
        {
            WithConstantConstructorArgumentFor("street", street);
            return this;
        }

        public CustomerInfoBuilder WithAddAddressInfo(string addAddressInfo)
        {
            WithConstantConstructorArgumentFor("addAddressInfo", addAddressInfo);
            return this;
        }

        public CustomerInfoBuilder WithZipCode(string zipCode)
        {
            WithConstantConstructorArgumentFor("zipCode", zipCode);
            return this;
        }

        public CustomerInfoBuilder WithCity(string city)
        {
            WithConstantConstructorArgumentFor("city", city);
            return this;
        }

        public CustomerInfoBuilder WithPhone(string phone)
        {
            WithConstantConstructorArgumentFor("phone", phone);
            return this;
        }

        public CustomerInfoBuilder WithEmail(string email)
        {
            WithConstantConstructorArgumentFor("email", email);
            return this;
        }
    }
}
