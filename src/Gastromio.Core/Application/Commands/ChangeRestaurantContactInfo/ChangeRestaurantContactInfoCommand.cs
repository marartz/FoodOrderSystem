using Gastromio.Core.Application.DTOs;
using Gastromio.Core.Domain.Model.Restaurants;

namespace Gastromio.Core.Application.Commands.ChangeRestaurantContactInfo
{
    public class ChangeRestaurantContactInfoCommand : ICommand<bool>
    {
        public ChangeRestaurantContactInfoCommand(RestaurantId restaurantId, string phone, string fax, string webSite,
            string responsiblePerson, string emailAddress, string mobile, bool orderNotificationByMobile)
        {
            RestaurantId = restaurantId;
            Phone = phone;
            Fax = fax;
            WebSite = webSite;
            ResponsiblePerson = responsiblePerson;
            EmailAddress = emailAddress;
            Mobile = mobile;
            OrderNotificationByMobile = orderNotificationByMobile;
        }

        public RestaurantId RestaurantId { get; }

        public string Phone { get; }

        public string Fax { get; }

        public string WebSite { get; }

        public string ResponsiblePerson { get; }

        public string EmailAddress { get; }

        public string Mobile { get; }

        public bool OrderNotificationByMobile { get; }
    }
}
