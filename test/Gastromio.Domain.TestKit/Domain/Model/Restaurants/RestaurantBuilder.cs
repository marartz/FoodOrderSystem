using System;
using System.Collections.Generic;
using System.Linq;
using Gastromio.Core.Domain.Model.Cuisines;
using Gastromio.Core.Domain.Model.PaymentMethods;
using Gastromio.Core.Domain.Model.Restaurants;
using Gastromio.Core.Domain.Model.Users;
using Gastromio.Domain.TestKit.Common;

namespace Gastromio.Domain.TestKit.Domain.Model.Restaurants
{
    public class RestaurantBuilder : TestObjectBuilderBase<Restaurant>
    {
        public RestaurantBuilder WithValidConstrains()
        {
            WithLengthConstrainedStringConstructorArgumentFor("name", 0, 100);

            WithConstrainedConstructorArgumentFor("regularOpeningDays", () =>
            {
                var regularOpeningDays = new List<RegularOpeningDay>();
                var randomWeekDay = RandomProvider.Random.Next(0, 6);
                for (var i = 0; i < 3; i++)
                {
                    var weekDay = (randomWeekDay + i) % 7;
                    regularOpeningDays.Add(new RegularOpeningDayBuilder()
                        .WithDayOfWeek(weekDay)
                        .WithOpeningPeriods(new OpeningPeriodBuilder()
                            .WithValidConstrains()
                            .CreateMany(1)
                        )
                        .Create());
                }
                return regularOpeningDays;
            });

            WithConstrainedConstructorArgumentFor("deviatingOpeningDays", () =>
            {
                var deviatingOpeningDays = new List<DeviatingOpeningDay>();
                var randomDate = new DateBuilder().Create();
                for (var i = 0; i < 3; i++)
                {
                    deviatingOpeningDays.Add(new DeviatingOpeningDayBuilder()
                        .WithDate(randomDate.AddDays(i))
                        .WithOpeningPeriods(new OpeningPeriodBuilder()
                            .WithValidConstrains()
                            .CreateMany(1)
                        )
                        .Create());
                }
                return deviatingOpeningDays;
            });

            return this;
        }

        public RestaurantBuilder WithId(RestaurantId id)
        {
            WithConstantConstructorArgumentFor("id", id);
            return this;
        }

        public RestaurantBuilder WithName(string name)
        {
            WithConstantConstructorArgumentFor("name", name);
            return this;
        }

        public RestaurantBuilder WithAlias(string alias)
        {
            WithConstantConstructorArgumentFor("alias", alias);
            return this;
        }

        public RestaurantBuilder WithAddress(Address address)
        {
            WithConstantConstructorArgumentFor("address", address);
            return this;
        }

        public RestaurantBuilder WithContactInfo(ContactInfo contactInfo)
        {
            WithConstantConstructorArgumentFor("contactInfo", contactInfo);
            return this;
        }

        public RestaurantBuilder WithoutRegularOpeningDays()
        {
            return WithRegularOpeningDays(Enumerable.Empty<RegularOpeningDay>());
        }

        public RestaurantBuilder WithRegularOpeningDays(IEnumerable<RegularOpeningDay> regularOpeningDays)
        {
            WithConstantConstructorArgumentFor("regularOpeningDays", regularOpeningDays);
            return this;
        }

        public RestaurantBuilder WithoutDeviatingOpeningDays()
        {
            return WithDeviatingOpeningDays(Enumerable.Empty<DeviatingOpeningDay>());
        }

        public RestaurantBuilder WithDeviatingOpeningDays(IEnumerable<DeviatingOpeningDay> deviatingOpeningDays)
        {
            WithConstantConstructorArgumentFor("deviatingOpeningDays", deviatingOpeningDays);
            return this;
        }

        public RestaurantBuilder WithPickupInfo(PickupInfo pickupInfo)
        {
            WithConstantConstructorArgumentFor("pickupInfo", pickupInfo);
            return this;
        }

        public RestaurantBuilder WithDeliveryInfo(DeliveryInfo deliveryInfo)
        {
            WithConstantConstructorArgumentFor("deliveryInfo", deliveryInfo);
            return this;
        }

        public RestaurantBuilder WithReservationInfo(ReservationInfo reservationInfo)
        {
            WithConstantConstructorArgumentFor("reservationInfo", reservationInfo);
            return this;
        }

        public RestaurantBuilder WithHygienicHandling(string hygienicHandling)
        {
            WithConstantConstructorArgumentFor("hygienicHandling", hygienicHandling);
            return this;
        }

        public RestaurantBuilder WithoutCuisines()
        {
            return WithCuisines(new HashSet<CuisineId>());
        }

        public RestaurantBuilder WithCuisines(ISet<CuisineId> cuisines)
        {
            WithConstantConstructorArgumentFor("cuisines", cuisines);
            return this;
        }

        public RestaurantBuilder WithoutPaymentMethods()
        {
            return WithPaymentMethods(new HashSet<PaymentMethodId>());
        }

        public RestaurantBuilder WithPaymentMethods(ISet<PaymentMethodId> paymentMethods)
        {
            WithConstantConstructorArgumentFor("paymentMethods", paymentMethods);
            return this;
        }

        public RestaurantBuilder WithoutAdministrators()
        {
            return WithAdministrators(new HashSet<UserId>());
        }

        public RestaurantBuilder WithAdministrators(ISet<UserId> administrators)
        {
            WithConstantConstructorArgumentFor("administrators", administrators);
            return this;
        }

        public RestaurantBuilder WithImportId(string importId)
        {
            WithConstantConstructorArgumentFor("importId", importId);
            return this;
        }

        public RestaurantBuilder WithIsActive(bool isActive)
        {
            WithConstantConstructorArgumentFor("isActive", isActive);
            return this;
        }

        public RestaurantBuilder WithNeedsSupport(bool needsSupport)
        {
            WithConstantConstructorArgumentFor("needsSupport", needsSupport);
            return this;
        }

        public RestaurantBuilder WithSupportedOrderMode(SupportedOrderMode supportedOrderMode)
        {
            WithConstantConstructorArgumentFor("supportedOrderMode", supportedOrderMode);
            return this;
        }

        public RestaurantBuilder WithoutExternalMenus()
        {
            return WithExternalMenus(Enumerable.Empty<ExternalMenu>());
        }

        public RestaurantBuilder WithExternalMenus(IEnumerable<ExternalMenu> externalMenus)
        {
            WithConstantConstructorArgumentFor("externalMenus", externalMenus);
            return this;
        }

        public RestaurantBuilder WithCreatedOn(DateTimeOffset createdOn)
        {
            WithConstantConstructorArgumentFor("createdOn", createdOn);
            return this;
        }

        public RestaurantBuilder WithCreatedBy(UserId createdBy)
        {
            WithConstantConstructorArgumentFor("createdBy", createdBy);
            return this;
        }

        public RestaurantBuilder WithUpdatedOn(DateTimeOffset updatedOn)
        {
            WithConstantConstructorArgumentFor("updatedOn", updatedOn);
            return this;
        }

        public RestaurantBuilder WithUpdatedBy(UserId updatedBy)
        {
            WithConstantConstructorArgumentFor("updatedBy", updatedBy);
            return this;
        }
    }
}
