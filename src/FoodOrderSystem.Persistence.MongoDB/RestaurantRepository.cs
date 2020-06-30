﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using FoodOrderSystem.Domain.Model.Cuisine;
using FoodOrderSystem.Domain.Model.Order;
using FoodOrderSystem.Domain.Model.PaymentMethod;
using FoodOrderSystem.Domain.Model.Restaurant;
using FoodOrderSystem.Domain.Model.User;
using MongoDB.Bson;
using MongoDB.Driver;

namespace FoodOrderSystem.Persistence.MongoDB
{
    public class RestaurantRepository : IRestaurantRepository
    {
        private readonly IMongoDatabase database;

        public RestaurantRepository(IMongoDatabase database)
        {
            this.database = database;
        }

        public async Task<IEnumerable<Restaurant>> SearchAsync(string searchPhrase, OrderType? orderType,
            CuisineId cuisineId, bool? isActive, CancellationToken cancellationToken = default)
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
            OrderType? orderType, CuisineId cuisineId, bool? isActive, int skip = 0, int take = -1,
            CancellationToken cancellationToken = default)
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

        private static Restaurant FromDocument(RestaurantModel document)
        {
            return new Restaurant(
                new RestaurantId(document.Id),
                document.Name,
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
                        document.ContactInfo.EmailAddress
                    )
                    : null,
                document.OpeningHours?.Select(en => new OpeningPeriod(
                    en.DayOfWeek,
                    TimeSpan.FromMinutes(en.StartTime),
                    TimeSpan.FromMinutes(en.EndTime))).ToList(),
                document.PickupInfo != null
                    ? new PickupInfo(
                        document.PickupInfo.Enabled,
                        document.PickupInfo.AverageTime.HasValue
                            ? TimeSpan.FromMinutes(document.PickupInfo.AverageTime.Value)
                            : (TimeSpan?) null,
                        Converter.ToDecimal(document.PickupInfo.MinimumOrderValue),
                        Converter.ToDecimal(document.PickupInfo.MaximumOrderValue)
                    )
                    : null,
                document.DeliveryInfo != null
                    ? new DeliveryInfo(
                        document.DeliveryInfo.Enabled,
                        document.DeliveryInfo.AverageTime.HasValue
                            ? TimeSpan.FromMinutes(document.DeliveryInfo.AverageTime.Value)
                            : (TimeSpan?) null,
                        Converter.ToDecimal(document.DeliveryInfo.MinimumOrderValue),
                        Converter.ToDecimal(document.DeliveryInfo.MaximumOrderValue),
                        Converter.ToDecimal(document.DeliveryInfo.Costs)
                    )
                    : null,
                document.ReservationInfo != null
                    ? new ReservationInfo(document.ReservationInfo.Enabled)
                    : null,
                document.HygienicHandling,
                new HashSet<CuisineId>(document.Cuisines.Select(en => new CuisineId(en))),
                new HashSet<PaymentMethodId>(document.PaymentMethods.Select(en => new PaymentMethodId(en))),
                new HashSet<UserId>(document.Administrators.Select(en => new UserId(en))),
                document.ImportId,
                document.IsActive,
                document.CreatedOn,
                new UserId(document.CreatedBy),
                document.UpdatedOn,
                new UserId(document.UpdatedBy)
            );
        }

        private static RestaurantModel ToDocument(Restaurant obj)
        {
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
                        EmailAddress = obj.ContactInfo.EmailAddress
                    }
                    : null,
                OpeningHours = obj.OpeningHours?.Select(en => new OpeningPeriodModel
                {
                    DayOfWeek = en.DayOfWeek,
                    StartTime = (int) en.Start.TotalMinutes,
                    EndTime = (int) en.End.TotalMinutes
                }).ToList(),
                PickupInfo = obj.PickupInfo != null
                    ? new PickupInfoModel
                    {
                        Enabled = obj.PickupInfo.Enabled,
                        AverageTime = obj.PickupInfo.AverageTime.HasValue
                            ? (int) obj.PickupInfo.AverageTime.Value.TotalMinutes
                            : (int?) null,
                        MinimumOrderValue = Converter.ToDouble(obj.PickupInfo.MinimumOrderValue),
                        MaximumOrderValue = Converter.ToDouble(obj.PickupInfo.MaximumOrderValue)
                    }
                    : null,
                DeliveryInfo = obj.DeliveryInfo != null
                    ? new DeliveryInfoModel
                    {
                        Enabled = obj.DeliveryInfo.Enabled,
                        AverageTime = obj.DeliveryInfo.AverageTime.HasValue
                            ? (int) obj.DeliveryInfo.AverageTime.Value.TotalMinutes
                            : (int?) null,
                        MinimumOrderValue = Converter.ToDouble(obj.DeliveryInfo.MinimumOrderValue),
                        MaximumOrderValue = Converter.ToDouble(obj.DeliveryInfo.MaximumOrderValue),
                        Costs = Converter.ToDouble(obj.DeliveryInfo.Costs)
                    }
                    : null,
                ReservationInfo = obj.ReservationInfo != null
                    ? new ReservationInfoModel
                    {
                        Enabled = obj.ReservationInfo.Enabled
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
                CreatedOn = obj.CreatedOn,
                CreatedBy = obj.CreatedBy.Value,
                UpdatedOn = obj.UpdatedOn,
                UpdatedBy = obj.UpdatedBy.Value
            };
        }
    }
}