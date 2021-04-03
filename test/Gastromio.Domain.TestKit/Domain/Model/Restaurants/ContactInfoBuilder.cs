using Gastromio.Core.Domain.Model.Restaurants;
using Gastromio.Domain.TestKit.Common;

namespace Gastromio.Domain.TestKit.Domain.Model.Restaurants
{
    public class ContactInfoBuilder : TestObjectBuilderBase<ContactInfo>
    {
        public ContactInfoBuilder WithValidConstrains()
        {
            WithPhone("+49 1234 56780");
            WithFax("+49 1234 56781");
            WithWebSite("http://www.musterrestaurant.de");
            WithEmailAddress("max@musterrestaurant.de");
            WithMobile("+49 1234 56782");
            return this;
        }

        public ContactInfoBuilder WithPhone(string phone)
        {
            WithConstantConstructorArgumentFor("phone", phone);
            return this;
        }

        public ContactInfoBuilder WithFax(string fax)
        {
            WithConstantConstructorArgumentFor("fax", fax);
            return this;
        }

        public ContactInfoBuilder WithWebSite(string webSite)
        {
            WithConstantConstructorArgumentFor("webSite", webSite);
            return this;
        }

        public ContactInfoBuilder WithResponsiblePerson(string responsiblePerson)
        {
            WithConstantConstructorArgumentFor("responsiblePerson", responsiblePerson);
            return this;
        }

        public ContactInfoBuilder WithEmailAddress(string emailAddress)
        {
            WithConstantConstructorArgumentFor("emailAddress", emailAddress);
            return this;
        }

        public ContactInfoBuilder WithMobile(string mobile)
        {
            WithConstantConstructorArgumentFor("mobile", mobile);
            return this;
        }

        public ContactInfoBuilder WithOrderNotificationByMobile(bool orderNotificationByMobile)
        {
            WithConstantConstructorArgumentFor("orderNotificationByMobile", orderNotificationByMobile);
            return this;
        }

    }
}
