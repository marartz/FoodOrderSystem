using Gastromio.Core.Domain.Model.PaymentMethods;
using Gastromio.Core.Domain.Model.Restaurants;

namespace Gastromio.Core.Application.Commands.AddPaymentMethodToRestaurant
{
    public class AddPaymentMethodToRestaurantCommand : ICommand<bool>
    {
        public AddPaymentMethodToRestaurantCommand(RestaurantId restaurantId, PaymentMethodId paymentMethodId)
        {
            RestaurantId = restaurantId;
            PaymentMethodId = paymentMethodId;
        }

        public RestaurantId RestaurantId { get; }
        public PaymentMethodId PaymentMethodId { get; }
    }
}
