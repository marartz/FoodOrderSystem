using Gastromio.Core.Domain.Model.PaymentMethods;
using Gastromio.Core.Domain.Model.Restaurants;

namespace Gastromio.Core.Application.Commands.RemovePaymentMethodFromRestaurant
{
    public class RemovePaymentMethodFromRestaurantCommand : ICommand<bool>
    {
        public RemovePaymentMethodFromRestaurantCommand(RestaurantId restaurantId, PaymentMethodId paymentMethodId)
        {
            RestaurantId = restaurantId;
            PaymentMethodId = paymentMethodId;
        }

        public RestaurantId RestaurantId { get; }
        public PaymentMethodId PaymentMethodId { get; }
    }
}
