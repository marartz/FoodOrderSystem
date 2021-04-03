using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Gastromio.Core.Application.Ports.Persistence;
using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.Dishes;
using Gastromio.Core.Domain.Model.Orders;
using Gastromio.Core.Domain.Model.PaymentMethods;
using Gastromio.Core.Domain.Model.Restaurants;
using Gastromio.Core.Domain.Model.Users;
using MongoDB.Driver;

namespace Gastromio.Persistence.MongoDB
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IMongoDatabase database;

        public OrderRepository(IMongoDatabase database)
        {
            this.database = database;
        }

        public async Task<IEnumerable<Order>> FindByRestaurantIdAsync(RestaurantId restaurantId,
            CancellationToken cancellationToken = default)
        {
            var collection = GetCollection();
            var cursor = await collection.FindAsync(
                Builders<OrderModel>.Filter.Eq(en => en.CartInfo.RestaurantId, restaurantId.Value),
                new FindOptions<OrderModel>
                {
                    Sort = Builders<OrderModel>.Sort.Ascending(en => en.Id)
                },
                cancellationToken);
            return cursor.ToEnumerable().Select(FromDocument);
        }

        public async Task<IEnumerable<Order>> FindByPendingCustomerNotificationAsync(
            CancellationToken cancellationToken = default)
        {
            var collection = GetCollection();

            var filter = Builders<OrderModel>.Filter.Exists(en => en.CustomerNotificationInfo, false) |
                         Builders<OrderModel>.Filter.Exists(en => en.CustomerNotificationInfo.Status, false) |
                         Builders<OrderModel>.Filter.Eq(en => en.CustomerNotificationInfo.Status, false);

            var cursor = await collection.FindAsync(filter, new FindOptions<OrderModel>
                {
                    Sort = Builders<OrderModel>.Sort.Ascending(en => en.Id)
                },
                cancellationToken);
            return cursor.ToEnumerable().Select(FromDocument);
        }

        public async Task<IEnumerable<Order>> FindByPendingRestaurantEmailNotificationAsync(
            CancellationToken cancellationToken = default)
        {
            var collection = GetCollection();

            var filter = Builders<OrderModel>.Filter.Exists(en => en.RestaurantNotificationInfo, false) |
                         Builders<OrderModel>.Filter.Exists(en => en.RestaurantNotificationInfo.Status, false) |
                         Builders<OrderModel>.Filter.Eq(en => en.RestaurantNotificationInfo.Status, false);

            var cursor = await collection.FindAsync(filter, new FindOptions<OrderModel>
                {
                    Sort = Builders<OrderModel>.Sort.Ascending(en => en.Id)
                },
                cancellationToken);
            return cursor.ToEnumerable().Select(FromDocument);
        }

        public async Task<IEnumerable<Order>> FindByPendingRestaurantMobileNotificationAsync(
            CancellationToken cancellationToken = default)
        {
            var collection = GetCollection();

            var filter = Builders<OrderModel>.Filter.Exists(en => en.RestaurantMobileNotificationInfo, false) |
                         Builders<OrderModel>.Filter.Exists(en => en.RestaurantMobileNotificationInfo.Status, false) |
                         Builders<OrderModel>.Filter.Eq(en => en.RestaurantMobileNotificationInfo.Status, false);

            var cursor = await collection.FindAsync(filter, new FindOptions<OrderModel>
                {
                    Sort = Builders<OrderModel>.Sort.Ascending(en => en.Id)
                },
                cancellationToken);
            return cursor.ToEnumerable().Select(FromDocument);
        }

        public async Task<Order> FindByOrderIdAsync(OrderId orderId, CancellationToken cancellationToken = default)
        {
            var collection = GetCollection();
            var cursor = await collection.FindAsync(Builders<OrderModel>.Filter.Eq(en => en.Id, orderId.Value),
                cancellationToken: cancellationToken);
            var document = await cursor.FirstOrDefaultAsync(cancellationToken);
            return document != null ? FromDocument(document) : null;
        }

        public async Task StoreAsync(Order order, CancellationToken cancellationToken = default)
        {
            var collection = GetCollection();
            var filter = Builders<OrderModel>.Filter.Eq(en => en.Id, order.Id.Value);
            var document = ToDocument(order);
            var options = new ReplaceOptions {IsUpsert = true};
            await collection.ReplaceOneAsync(filter, document, options, cancellationToken);
        }

        public async Task RemoveByRestaurantIdAsync(RestaurantId restaurantId,
            CancellationToken cancellationToken = default)
        {
            var collection = GetCollection();
            await collection.DeleteManyAsync(
                Builders<OrderModel>.Filter.Eq(en => en.CartInfo.RestaurantId, restaurantId.Value),
                cancellationToken);
        }

        public async Task RemoveAsync(OrderId orderId, CancellationToken cancellationToken = default)
        {
            var collection = GetCollection();
            await collection.DeleteOneAsync(Builders<OrderModel>.Filter.Eq(en => en.Id, orderId.Value),
                cancellationToken);
        }

        private IMongoCollection<OrderModel> GetCollection()
        {
            return database.GetCollection<OrderModel>(Constants.OrderCollectionName);
        }

        private static OrderType FromDbOrderType(string orderType)
        {
            switch (orderType)
            {
                case "pickup":
                    return OrderType.Pickup;
                case "delivery":
                    return OrderType.Delivery;
                case "reservation":
                    return OrderType.Reservation;
                default:
                    throw new InvalidOperationException($"unknown order type: {orderType}");
            }
        }

        private static Order FromDocument(OrderModel row)
        {
            return new Order(
                new OrderId(row.Id),
                row.CustomerInfo != null
                    ? new CustomerInfo(
                        row.CustomerInfo.GivenName,
                        row.CustomerInfo.LastName,
                        row.CustomerInfo.Street,
                        row.CustomerInfo.AddAddressInfo,
                        row.CustomerInfo.ZipCode,
                        row.CustomerInfo.City,
                        row.CustomerInfo.Phone,
                        row.CustomerInfo.Email
                    )
                    : null,
                row.CartInfo != null
                    ? new CartInfo(
                        FromDbOrderType(row.CartInfo.OrderType),
                        new RestaurantId(row.CartInfo.RestaurantId),
                        row.CartInfo.RestaurantName,
                        row.CartInfo.RestaurantInfo,
                        row.CartInfo.RestaurantPhone,
                        row.CartInfo.RestaurantEmail,
                        row.CartInfo.RestaurantMobile,
                        row.CartInfo.RestaurantNeedsSupport,
                        row.CartInfo.OrderedDishes?.Select(en => new OrderedDishInfo(
                            en.ItemId,
                            new DishId(en.DishId),
                            en.DishName,
                            en.VariantId,
                            en.VariantName,
                            (decimal) en.VariantPrice,
                            en.Count,
                            en.Remarks
                        )).ToList()
                    )
                    : null,
                row.Comments,
                new PaymentMethodId(row.PaymentMethodId),
                row.PaymentMethodName,
                row.PaymentMethodDescription,
                (decimal) row.Costs,
                (decimal) row.TotalPrice,
                row.ServiceTime?.ToDateTimeOffset(TimeSpan.Zero),
                row.CustomerNotificationInfo != null
                    ? new NotificationInfo(
                        row.CustomerNotificationInfo.Status,
                        row.CustomerNotificationInfo.Attempt,
                        row.CustomerNotificationInfo.Message,
                        row.CustomerNotificationInfo.Timestamp.ToDateTimeOffset(TimeSpan.Zero)
                    )
                    : null,
                row.RestaurantNotificationInfo != null
                    ? new NotificationInfo(
                        row.RestaurantNotificationInfo.Status,
                        row.RestaurantNotificationInfo.Attempt,
                        row.RestaurantNotificationInfo.Message,
                        row.RestaurantNotificationInfo.Timestamp.ToDateTimeOffset(TimeSpan.Zero)
                    )
                    : null,
                row.RestaurantMobileNotificationInfo != null
                    ? new NotificationInfo(
                        row.RestaurantMobileNotificationInfo.Status,
                        row.RestaurantMobileNotificationInfo.Attempt,
                        row.RestaurantMobileNotificationInfo.Message,
                        row.RestaurantMobileNotificationInfo.Timestamp.ToDateTimeOffset(TimeSpan.Zero)
                    )
                    : null,
                row.CreatedOn.ToDateTimeOffset(TimeSpan.Zero),
                row.UpdatedOn?.ToDateTimeOffset(TimeSpan.Zero),
                row.UpdatedBy.HasValue ? new UserId(row.UpdatedBy.Value) : null
            );
        }

        private static string ToDbOrderType(OrderType orderType)
        {
            switch (orderType)
            {
                case OrderType.Pickup:
                    return "pickup";
                case OrderType.Delivery:
                    return "delivery";
                case OrderType.Reservation:
                    return "reservation";
                default:
                    throw new InvalidOperationException($"unknown order type: {orderType}");
            }
        }

        private static OrderModel ToDocument(Order obj)
        {
            return new OrderModel
            {
                Id = obj.Id.Value,
                CustomerInfo = obj.CustomerInfo != null
                    ? new CustomerInfoModel
                    {
                        GivenName = obj.CustomerInfo.GivenName,
                        LastName = obj.CustomerInfo.LastName,
                        Street = obj.CustomerInfo.Street,
                        AddAddressInfo = obj.CustomerInfo.AddAddressInfo,
                        ZipCode = obj.CustomerInfo.ZipCode,
                        City = obj.CustomerInfo.City,
                        Phone = obj.CustomerInfo.Phone,
                        Email = obj.CustomerInfo.Email
                    }
                    : null,
                CartInfo = obj.CartInfo != null
                    ? new CartInfoModel
                    {
                        OrderType = ToDbOrderType(obj.CartInfo.OrderType),
                        RestaurantId = obj.CartInfo.RestaurantId.Value,
                        RestaurantName = obj.CartInfo.RestaurantName,
                        RestaurantInfo = obj.CartInfo.RestaurantInfo,
                        RestaurantEmail = obj.CartInfo.RestaurantEmail,
                        RestaurantPhone = obj.CartInfo.RestaurantPhone,
                        RestaurantMobile = obj.CartInfo.RestaurantMobile,
                        RestaurantNeedsSupport = obj.CartInfo.RestaurantNeedsSupport,
                        OrderedDishes = obj.CartInfo.OrderedDishes?.Select(en => new OrderedDishInfoModel
                        {
                            ItemId = en.ItemId,
                            DishId = en.DishId.Value,
                            DishName = en.DishName,
                            VariantId = en.VariantId,
                            VariantName = en.VariantName,
                            VariantPrice = (double) en.VariantPrice,
                            Count = en.Count,
                            Remarks = en.Remarks
                        }).ToList()
                    }
                    : null,
                Comments = obj.Comments,
                PaymentMethodId = obj.PaymentMethodId.Value,
                PaymentMethodName = obj.PaymentMethodName,
                PaymentMethodDescription = obj.PaymentMethodDescription,
                Costs = (double) obj.Costs,
                TotalPrice = (double) obj.TotalPrice,
                ServiceTime = obj.ServiceTime?.UtcDateTime,
                CustomerNotificationInfo = obj.CustomerNotificationInfo != null
                    ? new NotificationInfoModel
                    {
                        Status = obj.CustomerNotificationInfo.Status,
                        Attempt = obj.CustomerNotificationInfo.Attempt,
                        Message = obj.CustomerNotificationInfo.Message,
                        Timestamp = obj.CustomerNotificationInfo.Timestamp.UtcDateTime
                    }
                    : null,
                RestaurantNotificationInfo = obj.RestaurantEmailNotificationInfo != null
                    ? new NotificationInfoModel
                    {
                        Status = obj.RestaurantEmailNotificationInfo.Status,
                        Attempt = obj.RestaurantEmailNotificationInfo.Attempt,
                        Message = obj.RestaurantEmailNotificationInfo.Message,
                        Timestamp = obj.RestaurantEmailNotificationInfo.Timestamp.UtcDateTime
                    }
                    : null,
                RestaurantMobileNotificationInfo = obj.RestaurantMobileNotificationInfo != null
                    ? new NotificationInfoModel
                    {
                        Status = obj.RestaurantMobileNotificationInfo.Status,
                        Attempt = obj.RestaurantMobileNotificationInfo.Attempt,
                        Message = obj.RestaurantMobileNotificationInfo.Message,
                        Timestamp = obj.RestaurantMobileNotificationInfo.Timestamp.UtcDateTime
                    }
                    : null,
                CreatedOn = obj.CreatedOn.UtcDateTime,
                UpdatedOn = obj.UpdatedOn?.UtcDateTime,
                UpdatedBy = obj.UpdatedBy?.Value
            };
        }
    }
}
