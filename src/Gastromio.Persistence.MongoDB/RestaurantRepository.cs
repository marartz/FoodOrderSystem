using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Gastromio.Core.Application.Ports.Persistence;
using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.Cuisines;
using Gastromio.Core.Domain.Model.Orders;
using Gastromio.Core.Domain.Model.PaymentMethods;
using Gastromio.Core.Domain.Model.Restaurants;
using Gastromio.Core.Domain.Model.Users;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Gastromio.Persistence.MongoDB
{
    public class RestaurantRepository : IRestaurantRepository
    {
        private readonly IMongoDatabase database;
        private readonly ILogger<RestaurantRepository> logger;

        public RestaurantRepository(IMongoDatabase database, ILogger<RestaurantRepository> logger)
        {
            this.database = database;
            this.logger = logger;
        }

        public async Task<IEnumerable<Restaurant>> SearchAsync(string searchPhrase, OrderType? orderType,
            CuisineId cuisineId, DateTimeOffset? openingHour, bool? isActive, CancellationToken cancellationToken = default)
        {
            var collection = GetCollection();

            FilterDefinition<RestaurantModel> filter;
            if (!string.IsNullOrEmpty(searchPhrase))
            {
                var bsonRegEx = new BsonRegularExpression($".*{Regex.Escape(searchPhrase)}.*", "i");
                filter = Builders<RestaurantModel>.Filter.Regex(en => en.Name, bsonRegEx);
            }
            else
            {
                filter = new BsonDocument();
            }

            if (orderType.HasValue)
            {
                switch (orderType.Value)
                {
                    case OrderType.Pickup:
                    {
                        filter &= Builders<RestaurantModel>.Filter.Eq(en => en.PickupInfo.Enabled, true);
                        break;
                    }
                    case OrderType.Delivery:
                    {
                        filter &= Builders<RestaurantModel>.Filter.Eq(en => en.DeliveryInfo.Enabled, true);
                        break;
                    }
                    case OrderType.Reservation:
                    {
                        filter &= Builders<RestaurantModel>.Filter.Eq(en => en.ReservationInfo.Enabled, true);
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (cuisineId != null)
            {
                filter &= Builders<RestaurantModel>.Filter.AnyEq(en => en.Cuisines, cuisineId.Value);
            }

            if (openingHour.HasValue)
            {
                filter &= GenerateOpeningHourFilterDefinition(openingHour.Value);
            }

            if (isActive.HasValue)
            {
                filter &= Builders<RestaurantModel>.Filter.Eq(en => en.IsActive, isActive.Value);
            }

            var cursor = await collection.FindAsync(filter,
                new FindOptions<RestaurantModel>
                {
                    Sort = Builders<RestaurantModel>.Sort.Ascending(en => en.Name)
                },
                cancellationToken);
            return cursor.ToEnumerable().Select(FromDocument);
        }

        public async Task<(long total, IEnumerable<Restaurant> items)> SearchPagedAsync(string searchPhrase,
            OrderType? orderType, CuisineId cuisineId, DateTimeOffset? openingHour, bool? isActive, int skip = 0,
            int take = -1, CancellationToken cancellationToken = default)
        {
            var collection = GetCollection();

            FilterDefinition<RestaurantModel> filter;
            if (!string.IsNullOrEmpty(searchPhrase))
            {
                var bsonRegEx = new BsonRegularExpression($".*{Regex.Escape(searchPhrase)}.*", "i");
                filter = Builders<RestaurantModel>.Filter.Regex(en => en.Name, bsonRegEx);
            }
            else
            {
                filter = new BsonDocument();
            }

            if (orderType.HasValue)
            {
                switch (orderType.Value)
                {
                    case OrderType.Pickup:
                    {
                        filter &= Builders<RestaurantModel>.Filter.Eq(en => en.PickupInfo.Enabled, true);
                        break;
                    }
                    case OrderType.Delivery:
                    {
                        filter &= Builders<RestaurantModel>.Filter.Eq(en => en.DeliveryInfo.Enabled, true);
                        break;
                    }
                    case OrderType.Reservation:
                    {
                        filter &= Builders<RestaurantModel>.Filter.Eq(en => en.ReservationInfo.Enabled, true);
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (cuisineId != null)
            {
                filter &= Builders<RestaurantModel>.Filter.AnyEq(en => en.Cuisines, cuisineId.Value);
            }

            if (openingHour.HasValue)
            {
                filter &= GenerateOpeningHourFilterDefinition(openingHour.Value);
            }

            if (isActive.HasValue)
            {
                filter &= Builders<RestaurantModel>.Filter.Eq(en => en.IsActive, isActive.Value);
            }

            var total = await collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);

            if (take == 0)
                return (total, Enumerable.Empty<Restaurant>());

            var findOptions = new FindOptions<RestaurantModel>
            {
                Sort = Builders<RestaurantModel>.Sort.Ascending(en => en.Name)
            };
            if (skip > 0)
                findOptions.Skip = skip;
            if (take >= 0)
                findOptions.Limit = take;

            var cursor = await collection.FindAsync(filter, findOptions, cancellationToken);
            return (total, cursor.ToEnumerable().Select(FromDocument));
        }

        public async Task<Restaurant> FindByRestaurantIdAsync(RestaurantId restaurantId,
            CancellationToken cancellationToken = default)
        {
            var collection = GetCollection();
            var cursor = await collection.FindAsync(
                Builders<RestaurantModel>.Filter.Eq(en => en.Id, restaurantId.Value),
                cancellationToken: cancellationToken);
            var document = await cursor.FirstOrDefaultAsync(cancellationToken);
            return document != null ? FromDocument(document) : null;
        }

        public async Task<IEnumerable<Restaurant>> FindByRestaurantNameAsync(string restaurantName,
            CancellationToken cancellationToken = default)
        {
            var collection = GetCollection();

            var filter = Builders<RestaurantModel>.Filter.Where(en => en.Name.ToLower() == restaurantName.ToLower())
                         | Builders<RestaurantModel>.Filter.Where(en =>
                             en.Alias != null && en.Alias.ToLower() == restaurantName.ToLower());

            var cursor = await collection.FindAsync(filter, cancellationToken: cancellationToken);
            return cursor.ToEnumerable().Select(FromDocument);
        }

        public async Task<Restaurant> FindByImportIdAsync(string importId,
            CancellationToken cancellationToken = default)
        {
            var collection = GetCollection();
            var cursor = await collection.FindAsync(
                Builders<RestaurantModel>.Filter.Eq(en => en.ImportId, importId),
                cancellationToken: cancellationToken);
            var document = await cursor.FirstOrDefaultAsync(cancellationToken);
            return document != null ? FromDocument(document) : null;
        }

        public async Task<IEnumerable<Restaurant>> FindByCuisineIdAsync(CuisineId cuisineId,
            CancellationToken cancellationToken = default)
        {
            var collection = GetCollection();
            var cursor = await collection.FindAsync(
                Builders<RestaurantModel>.Filter.AnyEq(en => en.Cuisines, cuisineId.Value),
                new FindOptions<RestaurantModel>
                {
                    Sort = Builders<RestaurantModel>.Sort.Ascending(en => en.Name)
                },
                cancellationToken);
            return cursor.ToEnumerable().Select(FromDocument);
        }

        public async Task<IEnumerable<Restaurant>> FindByPaymentMethodIdAsync(PaymentMethodId paymentMethodId,
            CancellationToken cancellationToken = default)
        {
            var collection = GetCollection();
            var cursor = await collection.FindAsync(
                Builders<RestaurantModel>.Filter.AnyEq(en => en.PaymentMethods, paymentMethodId.Value),
                new FindOptions<RestaurantModel>
                {
                    Sort = Builders<RestaurantModel>.Sort.Ascending(en => en.Name)
                },
                cancellationToken);
            return cursor.ToEnumerable().Select(FromDocument);
        }

        public async Task<IEnumerable<Restaurant>> FindByUserIdAsync(UserId userId,
            CancellationToken cancellationToken = default)
        {
            var collection = GetCollection();
            var cursor = await collection.FindAsync(
                Builders<RestaurantModel>.Filter.AnyEq(en => en.Administrators, userId.Value),
                new FindOptions<RestaurantModel>
                {
                    Sort = Builders<RestaurantModel>.Sort.Ascending(en => en.Name)
                },
                cancellationToken);
            return cursor.ToEnumerable().Select(FromDocument);
        }

        public async Task<IEnumerable<Restaurant>> FindAllAsync(CancellationToken cancellationToken = default)
        {
            var collection = GetCollection();
            var cursor = await collection.FindAsync(new BsonDocument(),
                new FindOptions<RestaurantModel>
                {
                    Sort = Builders<RestaurantModel>.Sort.Ascending(en => en.Name)
                },
                cancellationToken);
            return cursor.ToEnumerable().Select(FromDocument);
        }

        public async Task StoreAsync(Restaurant restaurant, CancellationToken cancellationToken = default)
        {
            var collection = GetCollection();
            var filter = Builders<RestaurantModel>.Filter.Eq(en => en.Id, restaurant.Id.Value);
            var document = ToDocument(restaurant);
            document.Alias = CreateAlias(document.Name);
            var options = new ReplaceOptions {IsUpsert = true};
            await collection.ReplaceOneAsync(filter, document, options, cancellationToken);
        }

        public async Task RemoveAsync(RestaurantId restaurantId, CancellationToken cancellationToken = default)
        {
            var collection = GetCollection();
            await collection.DeleteOneAsync(Builders<RestaurantModel>.Filter.Eq(en => en.Id, restaurantId.Value),
                cancellationToken);
        }

        private IMongoCollection<RestaurantModel> GetCollection()
        {
            return database.GetCollection<RestaurantModel>(Constants.RestaurantCollectionName);
        }

        private FilterDefinition<RestaurantModel> GenerateOpeningHourFilterDefinition(DateTimeOffset openingHourFilter)
        {
            int filterDayOfWeek;

            switch (openingHourFilter.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    filterDayOfWeek = 0;
                    break;
                case DayOfWeek.Tuesday:
                    filterDayOfWeek = 1;
                    break;
                case DayOfWeek.Wednesday:
                    filterDayOfWeek = 2;
                    break;
                case DayOfWeek.Thursday:
                    filterDayOfWeek = 3;
                    break;
                case DayOfWeek.Friday:
                    filterDayOfWeek = 4;
                    break;
                case DayOfWeek.Saturday:
                    filterDayOfWeek = 5;
                    break;
                case DayOfWeek.Sunday:
                    filterDayOfWeek = 6;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            openingHourFilter = openingHourFilter.ToLocalTime();

            const double earliestOpeningHour = 4d;

            int filterMinutes;

            if (openingHourFilter.Hour < earliestOpeningHour)
            {
                filterDayOfWeek = (filterDayOfWeek - 1) % 7;
                filterMinutes = (openingHourFilter.Hour + 24) * 60 + openingHourFilter.Minute;
            }
            else
            {
                filterMinutes = openingHourFilter.Hour * 60 + openingHourFilter.Minute;
            }

            logger.LogDebug($"Filter: DayOfWeek = {filterDayOfWeek}, FilterMinutes = {filterMinutes}");

            var openingPeriodModelFilter =
                Builders<RegularOpeningPeriodModel>.Filter.Eq(m => m.DayOfWeek, filterDayOfWeek) &
                Builders<RegularOpeningPeriodModel>.Filter.Lte(m => m.StartTime, filterMinutes) &
                Builders<RegularOpeningPeriodModel>.Filter.Gte(m => m.EndTime, filterMinutes);

            return Builders<RestaurantModel>.Filter.ElemMatch(m => m.OpeningHours, openingPeriodModelFilter);
        }

        private static Restaurant FromDocument(RestaurantModel document)
        {
            var regularOpeningDays = document.OpeningHours?
                .GroupBy(en => en.DayOfWeek)
                .Select(group => new RegularOpeningDay(
                        group.Key,
                        group.Select(openingPeriodModel => new OpeningPeriod(
                                TimeSpan.FromMinutes(openingPeriodModel.StartTime),
                                TimeSpan.FromMinutes(openingPeriodModel.EndTime)
                            )
                        )
                    )
                ) ?? Enumerable.Empty<RegularOpeningDay>();

            var deviatingOpeningDays = document.DeviatingOpeningHours?
                .Select(deviatingOpeningDayModel => new DeviatingOpeningDay(
                        new Date(
                            deviatingOpeningDayModel.Date.Year,
                            deviatingOpeningDayModel.Date.Month,
                            deviatingOpeningDayModel.Date.Day
                        ),
                        FromDbDeviatingOpeningDayStatus(deviatingOpeningDayModel.Status),
                        deviatingOpeningDayModel.OpeningPeriods.Select(openingPeriodModel => new OpeningPeriod(
                                TimeSpan.FromMinutes(openingPeriodModel.StartTime),
                                TimeSpan.FromMinutes(openingPeriodModel.EndTime)
                            )
                        )
                    )
                ) ?? Enumerable.Empty<DeviatingOpeningDay>();

            return new Restaurant(
                new RestaurantId(document.Id),
                document.Name,
                document.Alias,
                document.Address != null
                    ? new Address(
                        document.Address.Street,
                        document.Address.ZipCode,
                        document.Address.City
                    )
                    : null,
                document.ContactInfo != null
                    ? new ContactInfo(
                        document.ContactInfo.Phone,
                        document.ContactInfo.Fax,
                        document.ContactInfo.WebSite,
                        document.ContactInfo.ResponsiblePerson,
                        document.ContactInfo.EmailAddress,
                        document.ContactInfo.Mobile,
                        document.ContactInfo.OrderNotificationByMobile
                    )
                    : null,
                regularOpeningDays,
                deviatingOpeningDays,
                document.PickupInfo != null
                    ? new PickupInfo(
                        document.PickupInfo.Enabled,
                        document.PickupInfo.AverageTime,
                        Converter.ToDecimal(document.PickupInfo.MinimumOrderValue),
                        Converter.ToDecimal(document.PickupInfo.MaximumOrderValue)
                    )
                    : null,
                document.DeliveryInfo != null
                    ? new DeliveryInfo(
                        document.DeliveryInfo.Enabled,
                        document.DeliveryInfo.AverageTime,
                        Converter.ToDecimal(document.DeliveryInfo.MinimumOrderValue),
                        Converter.ToDecimal(document.DeliveryInfo.MaximumOrderValue),
                        Converter.ToDecimal(document.DeliveryInfo.Costs)
                    )
                    : null,
                document.ReservationInfo != null
                    ? new ReservationInfo(document.ReservationInfo.Enabled, document.ReservationInfo.ReservationSystemUrl)
                    : null,
                document.HygienicHandling,
                new HashSet<CuisineId>(document.Cuisines.Select(en => new CuisineId(en))),
                new HashSet<PaymentMethodId>(document.PaymentMethods.Select(en => new PaymentMethodId(en))),
                new HashSet<UserId>(document.Administrators.Select(en => new UserId(en))),
                document.ImportId,
                document.IsActive,
                document.NeedsSupport,
                FromDbSupportedOrderMode(document.SupportedOrderMode),
                document.ExternalMenus?.Select(menu => new ExternalMenu(
                    menu.Id,
                    menu.Name,
                    menu.Description,
                    menu.Url
                )).ToList() ?? new List<ExternalMenu>(),
                document.CreatedOn.ToDateTimeOffset(TimeSpan.Zero),
                new UserId(document.CreatedBy),
                document.UpdatedOn.ToDateTimeOffset(TimeSpan.Zero),
                new UserId(document.UpdatedBy)
            );
        }

        private static RestaurantModel ToDocument(Restaurant obj)
        {
            var openingHours = obj.RegularOpeningDays?
                .SelectMany(keyValuePair => keyValuePair.Value.OpeningPeriods
                    .Select(en =>
                        new RegularOpeningPeriodModel
                        {
                            DayOfWeek = keyValuePair.Key,
                            StartTime = (int) en.Start.TotalMinutes,
                            EndTime = (int) en.End.TotalMinutes
                        }
                    )
                )
                .ToList() ?? new List<RegularOpeningPeriodModel>();

            var deviatingOpeningHours = obj.DeviatingOpeningDays?
                .Select(keyValuePair => new DeviatingOpeningDayModel
                    {
                        Date = new DateModel
                        {
                            Year = keyValuePair.Key.Year,
                            Month = keyValuePair.Key.Month,
                            Day = keyValuePair.Key.Day
                        },
                        Status = ToDbDeviatingOpeningDayStatus(keyValuePair.Value.Status),
                        OpeningPeriods = keyValuePair.Value.OpeningPeriods
                            .Select(en => new DeviatingOpeningPeriodModel
                                {
                                    StartTime = (int) en.Start.TotalMinutes,
                                    EndTime = (int) en.End.TotalMinutes
                                }
                            ).ToList()
                    }
                )
                .ToList() ?? new List<DeviatingOpeningDayModel>();

            return new RestaurantModel
            {
                Id = obj.Id.Value,
                Name = obj.Name,
                Address = obj.Address != null
                    ? new AddressModel
                    {
                        Street = obj.Address.Street,
                        ZipCode = obj.Address.ZipCode,
                        City = obj.Address.City
                    }
                    : null,
                ContactInfo = obj.ContactInfo != null
                    ? new ContactInfoModel
                    {
                        Phone = obj.ContactInfo.Phone,
                        Fax = obj.ContactInfo.Fax,
                        WebSite = obj.ContactInfo.WebSite,
                        ResponsiblePerson = obj.ContactInfo.ResponsiblePerson,
                        EmailAddress = obj.ContactInfo.EmailAddress,
                        Mobile = obj.ContactInfo.Mobile,
                        OrderNotificationByMobile = obj.ContactInfo.OrderNotificationByMobile
                    }
                    : null,
                OpeningHours = openingHours,
                DeviatingOpeningHours = deviatingOpeningHours,
                PickupInfo = obj.PickupInfo != null
                    ? new PickupInfoModel
                    {
                        Enabled = obj.PickupInfo.Enabled,
                        AverageTime = obj.PickupInfo.AverageTime,
                        MinimumOrderValue = Converter.ToDouble(obj.PickupInfo.MinimumOrderValue),
                        MaximumOrderValue = Converter.ToDouble(obj.PickupInfo.MaximumOrderValue)
                    }
                    : null,
                DeliveryInfo = obj.DeliveryInfo != null
                    ? new DeliveryInfoModel
                    {
                        Enabled = obj.DeliveryInfo.Enabled,
                        AverageTime = obj.DeliveryInfo.AverageTime,
                        MinimumOrderValue = Converter.ToDouble(obj.DeliveryInfo.MinimumOrderValue),
                        MaximumOrderValue = Converter.ToDouble(obj.DeliveryInfo.MaximumOrderValue),
                        Costs = Converter.ToDouble(obj.DeliveryInfo.Costs)
                    }
                    : null,
                ReservationInfo = obj.ReservationInfo != null
                    ? new ReservationInfoModel
                    {
                        Enabled = obj.ReservationInfo.Enabled,
                        ReservationSystemUrl = obj.ReservationInfo.ReservationSystemUrl
                    }
                    : null,
                HygienicHandling = obj.HygienicHandling,
                Cuisines = obj.Cuisines != null ? obj.Cuisines.Select(en => en.Value).ToList() : new List<Guid>(),
                PaymentMethods = obj.PaymentMethods != null
                    ? obj.PaymentMethods.Select(en => en.Value).ToList()
                    : new List<Guid>(),
                Administrators = obj.Administrators != null
                    ? obj.Administrators.Select(en => en.Value).ToList()
                    : new List<Guid>(),
                ImportId = obj.ImportId,
                IsActive = obj.IsActive,
                NeedsSupport = obj.NeedsSupport,
                SupportedOrderMode = ToDbSupportedOrderMode(obj.SupportedOrderMode),
                ExternalMenus = obj.ExternalMenus != null
                    ? obj.ExternalMenus.Select(en => new ExternalMenuModel
                    {
                        Id = en.Id,
                        Name = en.Name,
                        Description = en.Description,
                        Url = en.Url
                    }).ToList()
                    : new List<ExternalMenuModel>(),
                CreatedOn = obj.CreatedOn.UtcDateTime,
                CreatedBy = obj.CreatedBy.Value,
                UpdatedOn = obj.UpdatedOn.UtcDateTime,
                UpdatedBy = obj.UpdatedBy.Value
            };
        }

        private static string ToDbSupportedOrderMode(SupportedOrderMode supportedOrderMode)
        {
            switch (supportedOrderMode)
            {
                case SupportedOrderMode.OnlyPhone:
                    return "phone";
                case SupportedOrderMode.AtNextShift:
                    return "shift";
                case SupportedOrderMode.Anytime:
                    return "anytime";
                default:
                    throw new InvalidOperationException($"unknown supported order mode: {supportedOrderMode}");
            }
        }

        private static SupportedOrderMode FromDbSupportedOrderMode(string supportedOrderMode)
        {
            if (string.IsNullOrWhiteSpace(supportedOrderMode))
            {
                return SupportedOrderMode.OnlyPhone;
            }

            switch (supportedOrderMode)
            {
                case "phone":
                    return SupportedOrderMode.OnlyPhone;
                case "shift":
                    return SupportedOrderMode.AtNextShift;
                case "anytime":
                    return SupportedOrderMode.Anytime;
                default:
                    throw new InvalidOperationException($"unknown supported order mode: {supportedOrderMode}");
            }
        }

        internal static string CreateAlias(string name)
        {
            if (name == null)
                return null;

            name = name.ToLowerInvariant();

            var sb = new StringBuilder();

            for (var pos = 0; pos < name.Length; pos++)
            {
                var c = name[pos];

                if (char.IsWhiteSpace(c))
                    sb.Append('-');
                else if (char.IsLetter(c))
                    sb.Append(c);
                else if (char.IsDigit(c))
                    sb.Append(c);
            }

            return sb.ToString();
        }

        private static string ToDbDeviatingOpeningDayStatus(DeviatingOpeningDayStatus deviatingOpeningDayStatus)
        {
            switch (deviatingOpeningDayStatus)
            {
                case DeviatingOpeningDayStatus.Open:
                    return "open";
                case DeviatingOpeningDayStatus.Closed:
                    return "closed";
                case DeviatingOpeningDayStatus.FullyBooked:
                    return "fully-booked";
                default:
                    throw new InvalidOperationException($"unknown supported order mode: {deviatingOpeningDayStatus}");
            }
        }

        private static DeviatingOpeningDayStatus FromDbDeviatingOpeningDayStatus(string deviatingOpeningDayStatus)
        {
            if (string.IsNullOrWhiteSpace(deviatingOpeningDayStatus))
            {
                return DeviatingOpeningDayStatus.Open;
            }

            switch (deviatingOpeningDayStatus)
            {
                case "open":
                    return DeviatingOpeningDayStatus.Open;
                case "closed":
                    return DeviatingOpeningDayStatus.Closed;
                case "fully-booked":
                    return DeviatingOpeningDayStatus.FullyBooked;
                default:
                    throw new InvalidOperationException($"unknown supported order mode: {deviatingOpeningDayStatus}");
            }
        }
   }
}
