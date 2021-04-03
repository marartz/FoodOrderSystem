using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.Cuisines;
using Gastromio.Core.Domain.Model.PaymentMethods;
using Gastromio.Core.Domain.Model.Users;

namespace Gastromio.Core.Domain.Model.Restaurants
{
    public class Restaurant
    {
        private IDictionary<int, RegularOpeningDay> regularOpeningDays;
        private IDictionary<Date, DeviatingOpeningDay> deviatingOpeningDays;
        private ISet<CuisineId> cuisines;
        private ISet<PaymentMethodId> paymentMethods;
        private ISet<UserId> administrators;
        private readonly IDictionary<Guid, ExternalMenu> externalMenus;

        public Restaurant(
            RestaurantId id,
            string name,
            string alias,
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
            string importId,
            bool isActive,
            bool needsSupport,
            SupportedOrderMode supportedOrderMode,
            IEnumerable<ExternalMenu> externalMenus,
            DateTimeOffset createdOn,
            UserId createdBy,
            DateTimeOffset updatedOn,
            UserId updatedBy
        )
        {
            Id = id;
            Name = name;
            Alias = alias;
            Address = address ?? new Address(null, null, null);
            ContactInfo = contactInfo ?? new ContactInfo(null, null, null, null, null, null, false);
            this.regularOpeningDays = regularOpeningDays?.ToDictionary(en => en.DayOfWeek, en => en) ??
                                      new Dictionary<int, RegularOpeningDay>();
            this.deviatingOpeningDays = deviatingOpeningDays?.ToDictionary(en => en.Date, en => en) ??
                                        new Dictionary<Date, DeviatingOpeningDay>();
            PickupInfo = pickupInfo ?? new PickupInfo(false, 0, null, null);
            DeliveryInfo = deliveryInfo ?? new DeliveryInfo(false, 0, null, null, null);
            ReservationInfo = reservationInfo ?? new ReservationInfo(false, null);
            HygienicHandling = hygienicHandling;
            ImportId = importId;
            IsActive = isActive;
            NeedsSupport = needsSupport;
            SupportedOrderMode = supportedOrderMode;
            this.externalMenus = externalMenus?.ToDictionary(menu => menu.Id, menu => menu) ??
                                 new Dictionary<Guid, ExternalMenu>();
            CreatedOn = createdOn;
            CreatedBy = createdBy;
            UpdatedOn = updatedOn;
            UpdatedBy = updatedBy;
            this.cuisines = cuisines ?? new HashSet<CuisineId>();
            this.paymentMethods = paymentMethods ?? new HashSet<PaymentMethodId>();
            this.administrators = administrators ?? new HashSet<UserId>();
        }

        public RestaurantId Id { get; }

        public string Name { get; private set; }

        public string Alias { get; }

        public Address Address { get; private set; }

        public ContactInfo ContactInfo { get; private set; }

        public IReadOnlyDictionary<int, RegularOpeningDay> RegularOpeningDays
        {
            get
            {
                return new ReadOnlyDictionary<int, RegularOpeningDay>(regularOpeningDays);
            }
        }

        public IReadOnlyDictionary<Date, DeviatingOpeningDay> DeviatingOpeningDays
        {
            get
            {
                return new ReadOnlyDictionary<Date, DeviatingOpeningDay>(deviatingOpeningDays);
            }
        }

        public PickupInfo PickupInfo { get; private set; }

        public DeliveryInfo DeliveryInfo { get; private set; }

        public ReservationInfo ReservationInfo { get; private set; }

        public string HygienicHandling { get; private set; }

        public IReadOnlyCollection<CuisineId> Cuisines
        {
            get
            {
                return cuisines != null ? new ReadOnlyCollection<CuisineId>(cuisines.ToList()) : null;
            }
        }

        public IReadOnlyCollection<PaymentMethodId> PaymentMethods
        {
            get
            {
                return paymentMethods != null ? new ReadOnlyCollection<PaymentMethodId>(paymentMethods.ToList()) : null;
            }
        }

        public IReadOnlyCollection<UserId> Administrators
        {
            get
            {
                return administrators != null ? new ReadOnlyCollection<UserId>(administrators.ToList()) : null;
            }
        }

        public string ImportId { get; private set; }

        public bool IsActive { get; private set; }

        public bool NeedsSupport { get; private set; }

        public SupportedOrderMode SupportedOrderMode { get; private set; }

        public IReadOnlyCollection<ExternalMenu> ExternalMenus
        {
            get
            {
                return externalMenus.Values.ToImmutableList();
            }
        }

        public DateTimeOffset CreatedOn { get; }

        public UserId CreatedBy { get; }

        public DateTimeOffset UpdatedOn { get; private set; }

        public UserId UpdatedBy { get; private set; }

        public Result<bool> ChangeName(string name, UserId changedBy)
        {
            if (string.IsNullOrEmpty(name))
                return FailureResult<bool>.Create(FailureResultCode.RestaurantNameRequired);
            if (name.Length > 100)
                return FailureResult<bool>.Create(FailureResultCode.RestaurantNameTooLong, 100);

            Name = name;
            UpdatedOn = DateTimeOffset.UtcNow;
            UpdatedBy = changedBy;

            return SuccessResult<bool>.Create(true);
        }

        public Result<bool> ChangeAddress(Address address, UserId changedBy)
        {
            if (address == null)
                return FailureResult<bool>.Create(FailureResultCode.RestaurantAddressRequired);

            if (string.IsNullOrEmpty(address.Street))
                return FailureResult<bool>.Create(FailureResultCode.RestaurantStreetRequired);
            if (address.Street.Length > 100)
                return FailureResult<bool>.Create(FailureResultCode.RestaurantStreetTooLong, 100);
            if (!Validators.IsValidStreet(address.Street))
                return FailureResult<bool>.Create(FailureResultCode.RestaurantStreetInvalid, address.Street);

            if (string.IsNullOrEmpty(address.ZipCode))
                return FailureResult<bool>.Create(FailureResultCode.RestaurantZipCodeRequired);
            if (address.ZipCode.Length != 5 || address.ZipCode.Any(en => !char.IsDigit(en)))
                return FailureResult<bool>.Create(FailureResultCode.RestaurantZipCodeInvalid, address.ZipCode);
            if (!Validators.IsValidZipCode(address.ZipCode))
                return FailureResult<bool>.Create(FailureResultCode.RestaurantZipCodeInvalid, address.ZipCode);

            if (string.IsNullOrEmpty(address.City))
                return FailureResult<bool>.Create(FailureResultCode.RestaurantCityRequired);
            if (address.City.Length > 50)
                return FailureResult<bool>.Create(FailureResultCode.RestaurantCityTooLong, 50);

            Address = address;
            UpdatedOn = DateTimeOffset.UtcNow;
            UpdatedBy = changedBy;

            return SuccessResult<bool>.Create(true);
        }

        public Result<bool> ChangeContactInfo(ContactInfo contactInfo, UserId changedBy)
        {
            if (string.IsNullOrEmpty(contactInfo.Phone))
                return FailureResult<bool>.Create(FailureResultCode.RestaurantPhoneRequired);
            if (!Validators.IsValidPhoneNumber(contactInfo.Phone))
                return FailureResult<bool>.Create(FailureResultCode.RestaurantPhoneInvalid, contactInfo.Phone);

            if (!string.IsNullOrEmpty(contactInfo.Fax) && !Validators.IsValidPhoneNumber(contactInfo.Fax))
                return FailureResult<bool>.Create(FailureResultCode.RestaurantFaxInvalid, contactInfo.Fax);

            if (!string.IsNullOrEmpty(contactInfo.WebSite) && !Validators.IsValidWebsite(contactInfo.WebSite))
                return FailureResult<bool>.Create(FailureResultCode.RestaurantWebSiteInvalid, contactInfo.WebSite);

            if (string.IsNullOrEmpty(contactInfo.ResponsiblePerson))
                return FailureResult<bool>.Create(FailureResultCode.RestaurantResponsiblePersonRequired);

            if (string.IsNullOrEmpty(contactInfo.EmailAddress))
                return FailureResult<bool>.Create(FailureResultCode.RestaurantEmailRequired);
            if (!Validators.IsValidEmailAddress(contactInfo.EmailAddress))
                return FailureResult<bool>.Create(FailureResultCode.RestaurantEmailInvalid, contactInfo.EmailAddress);

            if (!string.IsNullOrEmpty(contactInfo.Mobile) && !Validators.IsValidPhoneNumber(contactInfo.Mobile))
                return FailureResult<bool>.Create(FailureResultCode.RestaurantMobileInvalid, contactInfo.Mobile);

            ContactInfo = contactInfo;
            UpdatedOn = DateTimeOffset.UtcNow;
            UpdatedBy = changedBy;

            return SuccessResult<bool>.Create(true);
        }

        public bool IsOpen(DateTimeOffset dateTime)
        {
            return FindOpeningPeriod(dateTime) != null;
        }

        public Result<bool> AddRegularOpeningPeriod(int dayOfWeek, OpeningPeriod openingPeriod, UserId changedBy)
        {
            if (!regularOpeningDays.TryGetValue(dayOfWeek, out var openingDay))
            {
                openingDay = new RegularOpeningDay(dayOfWeek, Enumerable.Empty<OpeningPeriod>());
                var result = openingDay.AddPeriod(openingPeriod);
                if (result.IsFailure)
                    return result.Cast<bool>();
                regularOpeningDays.Add(dayOfWeek, result.Value);
            }
            else
            {
                var result = openingDay.AddPeriod(openingPeriod);
                if (result.IsFailure)
                    return result.Cast<bool>();
                regularOpeningDays[dayOfWeek] = result.Value;
            }

            UpdatedOn = DateTimeOffset.UtcNow;
            UpdatedBy = changedBy;

            return SuccessResult<bool>.Create(true);
        }

        public Result<bool> RemoveRegularOpeningPeriod(int dayOfWeek, TimeSpan start, UserId changedBy)
        {
            if (!regularOpeningDays.TryGetValue(dayOfWeek, out var openingDay) || openingDay == null)
            {
                return SuccessResult<bool>.Create(true);
            }

            var changedOpeningDay = openingDay.RemovePeriod(start);
            if (changedOpeningDay.OpeningPeriods.Count > 0)
            {
                regularOpeningDays[dayOfWeek] = changedOpeningDay;
            }
            else
            {
                regularOpeningDays.Remove(dayOfWeek);
            }

            UpdatedOn = DateTimeOffset.UtcNow;
            UpdatedBy = changedBy;

            return SuccessResult<bool>.Create(true);
        }

        public Result<bool> AddDeviatingOpeningDay(Date date, DeviatingOpeningDayStatus status, UserId changedBy)
        {
            if (!deviatingOpeningDays.TryGetValue(date, out var openingDay))
            {
                openingDay = new DeviatingOpeningDay(date, status, Enumerable.Empty<OpeningPeriod>());
                deviatingOpeningDays.Add(date, openingDay);
            }

            UpdatedOn = DateTimeOffset.UtcNow;
            UpdatedBy = changedBy;

            return SuccessResult<bool>.Create(true);
        }

        public Result<bool> ChangeDeviatingOpeningDayStatus(Date date, DeviatingOpeningDayStatus status, UserId changedBy)
        {
            if (!deviatingOpeningDays.TryGetValue(date, out var openingDay))
            {
                return FailureResult<bool>.Create(FailureResultCode.RestaurantDeviatingOpeningDayDoesNotExist);
            }

            if (openingDay.OpeningPeriods?.Count > 0)
            {
                return FailureResult<bool>.Create(FailureResultCode.RestaurantDeviatingOpeningDayHasStillOpenPeriods);
            }

            deviatingOpeningDays[date] = new DeviatingOpeningDay(
                date,
                status,
                openingDay.OpeningPeriods
            );

            UpdatedOn = DateTimeOffset.UtcNow;
            UpdatedBy = changedBy;

            return SuccessResult<bool>.Create(true);
        }

        public Result<bool> RemoveDeviatingOpeningDay(Date date, UserId changedBy)
        {
            if (!deviatingOpeningDays.ContainsKey(date))
            {
                return SuccessResult<bool>.Create(true);
            }

            deviatingOpeningDays.Remove(date);

            UpdatedOn = DateTimeOffset.UtcNow;
            UpdatedBy = changedBy;

            return SuccessResult<bool>.Create(true);
        }

        public Result<bool> AddDeviatingOpeningPeriod(Date date, OpeningPeriod openingPeriod, UserId changedBy)
        {
            if (!deviatingOpeningDays.TryGetValue(date, out var openingDay))
            {
                return FailureResult<bool>.Create(FailureResultCode.RestaurantDeviatingOpeningDayDoesNotExist);
            }

            var result = openingDay.AddPeriod(openingPeriod);
            if (result.IsFailure)
                return result.Cast<bool>();
            deviatingOpeningDays[date] = result.Value;

            UpdatedOn = DateTimeOffset.UtcNow;
            UpdatedBy = changedBy;

            return SuccessResult<bool>.Create(true);
        }

        public Result<bool> RemoveDeviatingOpeningPeriod(Date date, TimeSpan start, UserId changedBy)
        {
            if (!deviatingOpeningDays.TryGetValue(date, out var openingDay))
            {
                return SuccessResult<bool>.Create(true);
            }

            var changedOpeningDay = openingDay.RemovePeriod(start);
            if (changedOpeningDay.OpeningPeriods.Count > 0)
            {
                deviatingOpeningDays[date] = new DeviatingOpeningDay(date, DeviatingOpeningDayStatus.Open, changedOpeningDay.OpeningPeriods);
            }
            else
            {
                deviatingOpeningDays[date] = new DeviatingOpeningDay(date, DeviatingOpeningDayStatus.Closed, changedOpeningDay.OpeningPeriods);
            }

            UpdatedOn = DateTimeOffset.UtcNow;
            UpdatedBy = changedBy;

            return SuccessResult<bool>.Create(true);
        }

        public Result<bool> RemoveAllOpeningDays(UserId changedBy)
        {
            regularOpeningDays = new Dictionary<int, RegularOpeningDay>();
            deviatingOpeningDays = new Dictionary<Date, DeviatingOpeningDay>();
            UpdatedOn = DateTimeOffset.UtcNow;
            UpdatedBy = changedBy;
            return SuccessResult<bool>.Create(true);
        }

        public bool IsOrderPossibleAt(DateTimeOffset orderDateTime)
        {
            if (orderDateTime < DateTimeOffset.Now)
            {
                orderDateTime = DateTimeOffset.Now;
            }

            if (SupportedOrderMode == SupportedOrderMode.OnlyPhone)
            {
                return false;
            }

            var openingPeriodOfOrderDateTime = FindOpeningPeriod(orderDateTime);
            if (openingPeriodOfOrderDateTime == null)
            {
                return false;
            }

            if (SupportedOrderMode != SupportedOrderMode.AtNextShift)
            {
                return true;
            }

            if (orderDateTime.ToUtcDate() > DateTimeOffset.UtcNow.ToUtcDate())
            {
                return true;
            }

            var openingPeriodOfNow = FindOpeningPeriod(DateTimeOffset.Now);

            var restaurantIsCurrentlyNotOpen = openingPeriodOfNow == null;
            if (restaurantIsCurrentlyNotOpen)
            {
                return true;
            }

            return openingPeriodOfNow != openingPeriodOfOrderDateTime;
        }

        public Result<bool> ChangePickupInfo(PickupInfo pickupInfo, UserId changedBy)
        {
            if (!pickupInfo.Enabled)
            {
                PickupInfo = pickupInfo;
                UpdatedOn = DateTimeOffset.UtcNow;
                UpdatedBy = changedBy;
                return SuccessResult<bool>.Create(true);
            }

            if (pickupInfo.AverageTime.HasValue && pickupInfo.AverageTime.Value < 5)
                return FailureResult<bool>.Create(FailureResultCode.RestaurantAveragePickupTimeTooLow);
            if (pickupInfo.AverageTime.HasValue && pickupInfo.AverageTime.Value > 120)
                return FailureResult<bool>.Create(FailureResultCode.RestaurantAveragePickupTimeTooHigh);

            if (pickupInfo.MinimumOrderValue.HasValue && pickupInfo.MinimumOrderValue < 0)
                return FailureResult<bool>.Create(FailureResultCode.RestaurantMinimumPickupOrderValueTooLow);
            if (pickupInfo.MinimumOrderValue.HasValue && pickupInfo.MinimumOrderValue > 50)
                return FailureResult<bool>.Create(FailureResultCode.RestaurantMinimumPickupOrderValueTooHigh);

            if (pickupInfo.MaximumOrderValue.HasValue && pickupInfo.MaximumOrderValue < 0)
                return FailureResult<bool>.Create(FailureResultCode.RestaurantMaximumPickupOrderValueTooLow);

            PickupInfo = pickupInfo;
            UpdatedOn = DateTimeOffset.UtcNow;
            UpdatedBy = changedBy;

            return SuccessResult<bool>.Create(true);
        }

        public Result<bool> ChangeDeliveryInfo(DeliveryInfo deliveryInfo, UserId changedBy)
        {
            if (!deliveryInfo.Enabled)
            {
                DeliveryInfo = deliveryInfo;
                UpdatedOn = DateTimeOffset.UtcNow;
                UpdatedBy = changedBy;
                return SuccessResult<bool>.Create(true);
            }

            if (deliveryInfo.AverageTime.HasValue && deliveryInfo.AverageTime.Value < 5)
                return FailureResult<bool>.Create(FailureResultCode.RestaurantAverageDeliveryTimeTooLow);
            if (deliveryInfo.AverageTime.HasValue && deliveryInfo.AverageTime.Value > 120)
                return FailureResult<bool>.Create(FailureResultCode.RestaurantAverageDeliveryTimeTooHigh);

            if (deliveryInfo.MinimumOrderValue.HasValue && deliveryInfo.MinimumOrderValue < 0)
                return FailureResult<bool>.Create(FailureResultCode.RestaurantMinimumDeliveryOrderValueTooLow);
            if (deliveryInfo.MinimumOrderValue.HasValue && deliveryInfo.MinimumOrderValue > 50)
                return FailureResult<bool>.Create(FailureResultCode.RestaurantMinimumDeliveryOrderValueTooHigh);

            if (deliveryInfo.MaximumOrderValue.HasValue && deliveryInfo.MaximumOrderValue < 0)
                return FailureResult<bool>.Create(FailureResultCode.RestaurantMaximumDeliveryOrderValueTooLow);

            if (deliveryInfo.Costs.HasValue && deliveryInfo.Costs < 0)
                return FailureResult<bool>.Create(FailureResultCode.RestaurantDeliveryCostsTooLow);
            if (deliveryInfo.Costs.HasValue && deliveryInfo.Costs > 10)
                return FailureResult<bool>.Create(FailureResultCode.RestaurantDeliveryCostsTooHigh);

            DeliveryInfo = deliveryInfo;
            UpdatedOn = DateTimeOffset.UtcNow;
            UpdatedBy = changedBy;

            return SuccessResult<bool>.Create(true);
        }

        public Result<bool> ChangeReservationInfo(ReservationInfo reservationInfo, UserId changedBy)
        {
            ReservationInfo = reservationInfo;
            UpdatedOn = DateTimeOffset.UtcNow;
            UpdatedBy = changedBy;
            return SuccessResult<bool>.Create(true);
        }

        public Result<bool> ChangeHygienicHandling(string hygienicHandling, UserId changedBy)
        {
            HygienicHandling = hygienicHandling;
            UpdatedOn = DateTimeOffset.UtcNow;
            UpdatedBy = changedBy;
            return SuccessResult<bool>.Create(true);
        }

        public Result<bool> AddCuisine(CuisineId cuisineId, UserId changedBy)
        {
            if (cuisines != null && cuisines.Contains(cuisineId))
                return SuccessResult<bool>.Create(true);
            if (cuisines == null)
                cuisines = new HashSet<CuisineId>();
            cuisines.Add(cuisineId);
            UpdatedOn = DateTimeOffset.UtcNow;
            UpdatedBy = changedBy;
            return SuccessResult<bool>.Create(true);
        }

        public Result<bool> RemoveCuisine(CuisineId cuisineId, UserId changedBy)
        {
            if (cuisines == null || !cuisines.Contains(cuisineId))
                return SuccessResult<bool>.Create(true);
            cuisines.Remove(cuisineId);
            UpdatedOn = DateTimeOffset.UtcNow;
            UpdatedBy = changedBy;
            return SuccessResult<bool>.Create(true);
        }

        public Result<bool> AddPaymentMethod(PaymentMethodId paymentMethodId, UserId changedBy)
        {
            if (paymentMethods != null && paymentMethods.Contains(paymentMethodId))
                return SuccessResult<bool>.Create(true);
            if (paymentMethods == null)
                paymentMethods = new HashSet<PaymentMethodId>();
            paymentMethods.Add(paymentMethodId);
            UpdatedOn = DateTimeOffset.UtcNow;
            UpdatedBy = changedBy;
            return SuccessResult<bool>.Create(true);
        }

        public Result<bool> RemovePaymentMethod(PaymentMethodId paymentMethodId, UserId changedBy)
        {
            if (paymentMethodId == PaymentMethodId.Cash)
                return FailureResult<bool>.Create(FailureResultCode.RestaurantWithoutCashPaymentNotAllowed);

            if (paymentMethods == null || !paymentMethods.Contains(paymentMethodId))
                return SuccessResult<bool>.Create(true);
            paymentMethods.Remove(paymentMethodId);
            UpdatedOn = DateTimeOffset.UtcNow;
            UpdatedBy = changedBy;
            return SuccessResult<bool>.Create(true);
        }

        public bool HasAdministrator(UserId userId)
        {
            return administrators != null && administrators.Contains(userId);
        }

        public Result<bool> AddAdministrator(UserId userId, UserId changedBy)
        {
            if (administrators != null && administrators.Contains(userId))
                return SuccessResult<bool>.Create(true);
            if (administrators == null)
                administrators = new HashSet<UserId>();
            administrators.Add(userId);
            UpdatedOn = DateTimeOffset.UtcNow;
            UpdatedBy = changedBy;
            return SuccessResult<bool>.Create(true);
        }

        public Result<bool> RemoveAdministrator(UserId userId, UserId changedBy)
        {
            if (administrators == null || !administrators.Contains(userId))
                return SuccessResult<bool>.Create(true);
            administrators.Remove(userId);
            UpdatedOn = DateTimeOffset.UtcNow;
            UpdatedBy = changedBy;
            return SuccessResult<bool>.Create(true);
        }

        public Result<bool> ChangeImportId(string importId, UserId changedBy)
        {
            ImportId = importId;
            UpdatedOn = DateTimeOffset.UtcNow;
            UpdatedBy = changedBy;
            return SuccessResult<bool>.Create(true);
        }

        public Result<bool> Deactivate(UserId changedBy)
        {
            if (!IsActive)
                return SuccessResult<bool>.Create(true);
            IsActive = false;
            UpdatedOn = DateTimeOffset.UtcNow;
            UpdatedBy = changedBy;
            return SuccessResult<bool>.Create(true);
        }

        public Result<bool> Activate(UserId changedBy)
        {
            if (IsActive)
                return SuccessResult<bool>.Create(true);
            IsActive = true;
            UpdatedOn = DateTimeOffset.UtcNow;
            UpdatedBy = changedBy;
            return SuccessResult<bool>.Create(true);
        }

        public Result<bool> DisableSupport(UserId changedBy)
        {
            if (!NeedsSupport)
                return SuccessResult<bool>.Create(true);
            NeedsSupport = false;
            UpdatedOn = DateTimeOffset.UtcNow;
            UpdatedBy = changedBy;
            return SuccessResult<bool>.Create(true);
        }

        public Result<bool> EnableSupport(UserId changedBy)
        {
            if (NeedsSupport)
                return SuccessResult<bool>.Create(true);
            NeedsSupport = true;
            UpdatedOn = DateTimeOffset.UtcNow;
            UpdatedBy = changedBy;
            return SuccessResult<bool>.Create(true);
        }

        public Result<bool> ChangeSupportedOrderMode(SupportedOrderMode supportedOrderMode, UserId changedBy)
        {
            SupportedOrderMode = supportedOrderMode;
            UpdatedOn = DateTimeOffset.UtcNow;
            UpdatedBy = changedBy;
            return SuccessResult<bool>.Create(true);
        }

        public Result<bool> SetExternalMenu(ExternalMenu menu, UserId changedBy)
        {
            if (menu == null)
                throw new ArgumentNullException(nameof(menu));
            if (string.IsNullOrWhiteSpace(menu.Name))
                return FailureResult<bool>.Create(FailureResultCode.ExternalMenuHasNoName);
            if (string.IsNullOrWhiteSpace(menu.Description))
                return FailureResult<bool>.Create(FailureResultCode.ExternalMenuHasNoDescription);
            if (string.IsNullOrWhiteSpace(menu.Url))
                return FailureResult<bool>.Create(FailureResultCode.ExternalMenuHasNoUrl);
            if (!externalMenus.ContainsKey(menu.Id))
            {
                externalMenus.Add(menu.Id, menu);
            }
            else
            {
                externalMenus[menu.Id] = menu;
            }

            return SuccessResult<bool>.Create(true);
        }

        public Result<bool> RemoveExternalMenu(Guid menuId, UserId changedBy)
        {
            if (menuId == default)
                throw new ArgumentException($"is default", nameof(menuId));
            if (!externalMenus.ContainsKey(menuId))
                return FailureResult<bool>.Create(FailureResultCode.ExternalMenuDoesNotExist);
            externalMenus.Remove(menuId);
            return SuccessResult<bool>.Create(true);
        }

        private (int DayOfWeek, TimeSpan Time) GetDayOfWeekAndTimeInfoFromDateTime(DateTimeOffset dateTime)
        {
            var dayOfWeek = ((int) dateTime.DayOfWeek - 1) % 7;
            if (dayOfWeek < 0)
            {
                dayOfWeek += 7;
            }

            dateTime = dateTime.ToLocalTime();

            var time = dateTime.Hour * 60 + dateTime.Minute;
            if (dateTime.Hour < OpeningPeriod.EarliestOpeningTime)
            {
                dayOfWeek = (dayOfWeek - 1) % 7;
                if (dayOfWeek < 0)
                {
                    dayOfWeek += 7;
                }

                time += 24 * 60;
            }

            return (dayOfWeek, TimeSpan.FromMinutes(time));
        }

        private OpeningPeriod FindOpeningPeriod(DateTimeOffset dateTime)
        {
            var (dayOfWeek, time) = GetDayOfWeekAndTimeInfoFromDateTime(dateTime);

            var date = new Date(dateTime.Year, dateTime.Month, dateTime.Day);
            if (deviatingOpeningDays != null &&
                deviatingOpeningDays.TryGetValue(date, out var deviatingOpeningDay))
            {
                return deviatingOpeningDay.FindPeriodAtTime(time);
            }

            if (regularOpeningDays != null &&
                regularOpeningDays.TryGetValue(dayOfWeek, out var regularOpeningDay))
            {
                return regularOpeningDay.FindPeriodAtTime(time);
            }

            return null;
        }
   }
}
