using System.Collections.Generic;
using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.Cuisines;
using Gastromio.Core.Domain.Model.PaymentMethods;
using Gastromio.Core.Domain.Model.Restaurants;
using Gastromio.Core.Domain.Model.Users;
using Moq;
using Moq.Language.Flow;

namespace Gastromio.Domain.TestKit.Domain.Model.Restaurants
{
    public class RestaurantFactoryMock : Mock<IRestaurantFactory>
    {
        public RestaurantFactoryMock(MockBehavior behavior) : base(behavior)
        {
        }

        public ISetup<IRestaurantFactory, Result<Restaurant>> SetupCreate(
            string name,
            Address address,
            ContactInfo contactInfo,
            IEnumerable<RegularOpeningDay> regularOpeningDays,
            IEnumerable<DeviatingOpeningDay> deviatingOpeningDays,
            PickupInfo pickupInfo,
            DeliveryInfo deliveryInfo,
            ReservationInfo reservationInfo,
            string hygienicHandling,
            ISet<CuisineId> cuisines,
            ISet<PaymentMethodId> paymentMethods,
            ISet<UserId> administrators,
            UserId createBy
        )
        {
            return Setup(m => m.Create(name, address, contactInfo, regularOpeningDays, deviatingOpeningDays, pickupInfo,
                deliveryInfo, reservationInfo, hygienicHandling, cuisines, paymentMethods, administrators, createBy));
        }

        public ISetup<IRestaurantFactory, Result<Restaurant>> SetupCreateWithName(
            string name,
            UserId createdBy
        )
        {
            return Setup(m => m.CreateWithName(name, createdBy));
        }
    }
}
