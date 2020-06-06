﻿using FoodOrderSystem.Domain.Model.Cuisine;
using FoodOrderSystem.Domain.Model.PaymentMethod;
using FoodOrderSystem.Domain.Model.Restaurant;
using FoodOrderSystem.Domain.Model.User;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FoodOrderSystem.Persistence
{
    public class RestaurantRepository : IRestaurantRepository
    {
        private readonly SystemDbContext dbContext;

        public RestaurantRepository(SystemDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public Task<IEnumerable<Restaurant>> SearchAsync(string searchPhrase, CancellationToken cancellationToken = default)
        {
            return Task.Factory.StartNew(() =>
            {
                var rows = dbContext.Restaurants.Where(en => EF.Functions.Like(en.Name, $"%{searchPhrase}%")).OrderBy(en => en.Name).ToList();
                return rows.Select(FromRow);
            }, cancellationToken);
        }

        public Task<IEnumerable<Restaurant>> FindAllAsync(CancellationToken cancellationToken = default)
        {
            return Task.Factory.StartNew(() =>
            {
                var rows = dbContext.Restaurants.OrderBy(en => en.Name).ToList();
                return rows.Select(FromRow);
            }, cancellationToken);
        }

        public Task<Restaurant> FindByRestaurantIdAsync(RestaurantId restaurantId, CancellationToken cancellationToken = default)
        {
            return Task.Factory.StartNew(() =>
            {
                var row = dbContext.Restaurants.FirstOrDefault(en => en.Id == restaurantId.Value);
                if (row == null)
                    return null;
                return FromRow(row);
            }, cancellationToken);
        }

        public Task<IEnumerable<Restaurant>> FindByCuisineIdAsync(CuisineId cuisineId, CancellationToken cancellationToken = default)
        {
            return Task.Factory.StartNew(() =>
            {
                var rows = dbContext.Restaurants
                    .Where(rest => rest.RestaurantCuisines.Any(restcuisine => restcuisine.CuisineId == cuisineId.Value))
                    .OrderBy(en => en.Name)
                    .ToList();
                return rows.Select(FromRow);
            }, cancellationToken);
        }

        public Task<IEnumerable<Restaurant>> FindByPaymentMethodIdAsync(PaymentMethodId paymentMethodId, CancellationToken cancellationToken = default)
        {
            return Task.Factory.StartNew(() =>
            {
                var rows = dbContext.Restaurants
                    .Where(rest => rest.RestaurantPaymentMethods.Any(restpaymentmethod => restpaymentmethod.PaymentMethodId == paymentMethodId.Value))
                    .OrderBy(en => en.Name)
                    .ToList();
                return rows.Select(FromRow);
            }, cancellationToken);
        }

        public Task<IEnumerable<Restaurant>> FindByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
        {
            return Task.Factory.StartNew(() =>
            {
                var rows = dbContext.Restaurants
                    .Where(rest => rest.RestaurantUsers.Any(restuser => restuser.UserId == userId.Value))
                    .OrderBy(en => en.Name)
                    .ToList();
                return rows.Select(FromRow);
            }, cancellationToken);
        }


        public Task StoreAsync(Restaurant restaurant, CancellationToken cancellationToken = default)
        {
            return Task.Factory.StartNew(() =>
            {
                var dbSet = dbContext.Restaurants;

                var row = dbSet.FirstOrDefault(x => x.Id == restaurant.Id.Value);
                if (row != null)
                {
                    ToRow(restaurant, row);
                    dbSet.Update(row);
                }
                else
                {
                    row = new RestaurantRow();
                    ToRow(restaurant, row);
                    dbSet.Add(row);
                }

                dbContext.SaveChanges();
            }, cancellationToken);
        }

        public Task RemoveAsync(RestaurantId restaurantId, CancellationToken cancellationToken = default)
        {
            return Task.Factory.StartNew(() =>
            {
                var dbSet = dbContext.Restaurants;

                var row = dbSet.FirstOrDefault(en => en.Id == restaurantId.Value);
                if (row != null)
                {
                    dbSet.Remove(row);
                    dbContext.SaveChanges();
                }
            }, cancellationToken);
        }

        private static Restaurant FromRow(RestaurantRow row)
        {
            return new Restaurant(new RestaurantId(row.Id),
                row.Name,
                row.Image,
                new Address(row.AddressStreet, row.AddressZipCode, row.AddressCity),
                row.DeliveryTimes.Select(en => new DeliveryTime(en.DayOfWeek, TimeSpan.FromMinutes(en.StartTime), TimeSpan.FromMinutes(en.EndTime))).ToList(),
                row.MinimumOrderValue,
                row.DeliveryCosts,
                row.Phone,
                row.WebSite,
                row.Imprint,
                row.OrderEmailAddress,
                new HashSet<CuisineId>(row.RestaurantCuisines.Select(en => new CuisineId(en.CuisineId))),
                new HashSet<PaymentMethodId>(row.RestaurantPaymentMethods.Select(en => new PaymentMethodId(en.PaymentMethodId))),
                new HashSet<UserId>(row.RestaurantUsers.Select(en => new UserId(en.UserId)))
            );
        }

        private static void ToRow(Restaurant obj, RestaurantRow row)
        {
            // TODO
            row.Id = obj.Id.Value;
            row.Name = obj.Name;
            row.Image = obj.Image;
            if (obj.Address != null)
            {
                row.AddressStreet = obj.Address.Street;
                row.AddressZipCode = obj.Address.ZipCode;
                row.AddressCity = obj.Address.City;
            }
            row.DeliveryTimes = new List<DeliveryTimeRow>();
            if (obj.DeliveryTimes != null && obj.DeliveryTimes.Count > 0)
            {
                foreach (var deliveryTime in obj.DeliveryTimes)
                {
                    row.DeliveryTimes.Add(new DeliveryTimeRow
                    {
                        RestaurantId = obj.Id.Value,
                        DayOfWeek = deliveryTime.DayOfWeek,
                        StartTime = (int)deliveryTime.Start.TotalMinutes,
                        EndTime = (int)deliveryTime.End.TotalMinutes
                    });
                }
            }
            row.MinimumOrderValue = obj.MinimumOrderValue;
            row.DeliveryCosts = obj.DeliveryCosts;
            row.Phone = obj.Phone;
            row.WebSite = obj.WebSite;
            row.Imprint = obj.Imprint;
            row.OrderEmailAddress = obj.OrderEmailAddress;
            row.RestaurantCuisines = new List<RestaurantCuisineRow>();
            if (obj.Cuisines != null && obj.Cuisines.Count > 0)
            {
                foreach (var cuisine in obj.Cuisines)
                {
                    row.RestaurantCuisines.Add(new RestaurantCuisineRow
                    {
                        RestaurantId = obj.Id.Value,
                        CuisineId = cuisine.Value
                    });
                }
            }
            row.RestaurantPaymentMethods = new List<RestaurantPaymentMethodRow>();
            if (obj.PaymentMethods != null && obj.PaymentMethods.Count > 0)
            {
                foreach (var paymentMethod in obj.PaymentMethods)
                {
                    row.RestaurantPaymentMethods.Add(new RestaurantPaymentMethodRow
                    {
                        RestaurantId = obj.Id.Value,
                        PaymentMethodId = paymentMethod.Value
                    });
                }
            }
            row.RestaurantUsers = new List<RestaurantUserRow>();
            if (obj.Administrators != null && obj.Administrators.Count > 0)
            {
                foreach (var administrator in obj.Administrators)
                {
                    row.RestaurantUsers.Add(new RestaurantUserRow
                    {
                        RestaurantId = obj.Id.Value,
                        UserId = administrator.Value
                    });
                }
            }
        }
    }
}
