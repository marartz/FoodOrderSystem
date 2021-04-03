using System;
using System.Collections.Generic;
using System.Linq;
using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.Cuisines;
using Gastromio.Core.Domain.Model.PaymentMethods;
using Gastromio.Core.Domain.Model.Users;

namespace Gastromio.Core.Domain.Model.Restaurants
{
    public class RestaurantFactory : IRestaurantFactory
    {
        public Result<Restaurant> Create(
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
            UserId createdBy
        )
        {
            var restaurant = CreateRestaurant(createdBy);

            var tempResult = restaurant.ChangeName(name, createdBy);
            if (tempResult.IsFailure)
                return tempResult.Cast<Restaurant>();

            tempResult = restaurant.ChangeAddress(address, createdBy);
            if (tempResult.IsFailure)
                return tempResult.Cast<Restaurant>();

            tempResult = restaurant.ChangeContactInfo(contactInfo, createdBy);
            if (tempResult.IsFailure)
                return tempResult.Cast<Restaurant>();

            if (regularOpeningDays != null)
            {
                foreach (var openingDay in regularOpeningDays)
                {
                    foreach (var openingPeriod in openingDay.OpeningPeriods)
                    {
                        tempResult = restaurant.AddRegularOpeningPeriod(openingDay.DayOfWeek, openingPeriod, createdBy);
                        if (tempResult.IsFailure)
                            return tempResult.Cast<Restaurant>();
                    }
                }
            }

            if (deviatingOpeningDays != null)
            {
                foreach (var openingDay in deviatingOpeningDays)
                {
                    tempResult = restaurant.AddDeviatingOpeningDay(openingDay.Date, openingDay.Status, createdBy);
                    if (tempResult.IsFailure)
                        return tempResult.Cast<Restaurant>();
                    foreach (var openingPeriod in openingDay.OpeningPeriods)
                    {
                        tempResult = restaurant.AddDeviatingOpeningPeriod(openingDay.Date, openingPeriod, createdBy);
                        if (tempResult.IsFailure)
                            return tempResult.Cast<Restaurant>();
                    }
                }
            }

            tempResult = restaurant.ChangePickupInfo(pickupInfo, createdBy);
            if (tempResult.IsFailure)
                return tempResult.Cast<Restaurant>();

            tempResult = restaurant.ChangeDeliveryInfo(deliveryInfo, createdBy);
            if (tempResult.IsFailure)
                return tempResult.Cast<Restaurant>();

            tempResult = restaurant.ChangeReservationInfo(reservationInfo, createdBy);
            if (tempResult.IsFailure)
                return tempResult.Cast<Restaurant>();

            tempResult = restaurant.ChangeHygienicHandling(hygienicHandling, createdBy);
            if (tempResult.IsFailure)
                return tempResult.Cast<Restaurant>();

            if (cuisines != null)
            {
                foreach (var cuisine in cuisines)
                {
                    tempResult = restaurant.AddCuisine(cuisine, createdBy);
                    if (tempResult.IsFailure)
                        return tempResult.Cast<Restaurant>();
                }
            }

            if (paymentMethods != null)
            {
                foreach (var paymentMethod in paymentMethods)
                {
                    tempResult = restaurant.AddPaymentMethod(paymentMethod, createdBy);
                    if (tempResult.IsFailure)
                        return tempResult.Cast<Restaurant>();
                }
            }

            if (administrators != null)
            {
                foreach (var administrator in administrators)
                {
                    tempResult = restaurant.AddAdministrator(administrator, createdBy);
                    if (tempResult.IsFailure)
                        return tempResult.Cast<Restaurant>();
                }
            }

            return SuccessResult<Restaurant>.Create(restaurant);
        }

        public Result<Restaurant> CreateWithName(
            string name,
            UserId createdBy
        )
        {
            var restaurant = CreateRestaurant(createdBy);

            var tempResult = restaurant.ChangeName(name, createdBy);

            return tempResult.IsFailure ? tempResult.Cast<Restaurant>() : SuccessResult<Restaurant>.Create(restaurant);
        }

        private static Restaurant CreateRestaurant(UserId createdBy)
        {
            var restaurant = new Restaurant(
                new RestaurantId(Guid.NewGuid()),
                null,
                null,
                new Address(null, null, null),
                new ContactInfo(null, null, null, null, null, null, false),
                Enumerable.Empty<RegularOpeningDay>(),
                Enumerable.Empty<DeviatingOpeningDay>(),
                new PickupInfo(false, 0, null, null),
                new DeliveryInfo(false, 0, null, null, null),
                new ReservationInfo(false, null),
                null,
                new HashSet<CuisineId>(),
                new HashSet<PaymentMethodId>(),
                new HashSet<UserId>(),
                null,
                false,
                false,
                SupportedOrderMode.OnlyPhone,
                Enumerable.Empty<ExternalMenu>(),
                DateTimeOffset.UtcNow,
                createdBy,
                DateTimeOffset.UtcNow,
                createdBy
            );
            return restaurant;
        }
    }
}
