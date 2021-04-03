using Gastromio.Core.Domain.Model.Restaurants;

namespace Gastromio.Core.Application.Commands.ChangeRestaurantAddress
{
    public class ChangeRestaurantAddressCommand : ICommand<bool>
    {
        public ChangeRestaurantAddressCommand(RestaurantId restaurantId, string street, string zipCode, string city)
        {
            RestaurantId = restaurantId;
            Street = street;
            ZipCode = zipCode;
            City = city;
        }

        public RestaurantId RestaurantId { get; }

        public string Street { get; }
        
        public string ZipCode { get; }
        
        public string City { get; }
    }
}
