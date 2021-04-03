namespace Gastromio.Core.Domain.Model.Restaurants
{
    public class ContactInfo
    {
        public ContactInfo(string phone, string fax, string webSite, string responsiblePerson, string emailAddress,
            string mobile, bool orderNotificationByMobile)
        {
            Phone = phone;
            Fax = fax;
            WebSite = webSite;
            ResponsiblePerson = responsiblePerson;
            EmailAddress = emailAddress;
            Mobile = mobile;
            OrderNotificationByMobile = orderNotificationByMobile;
        }

        public string Phone { get; }

        public string Fax { get; }

        public string WebSite { get; }

        public string ResponsiblePerson { get; }

        public string EmailAddress { get; }

        public string Mobile { get; }

        public bool OrderNotificationByMobile { get; }
    }
}
