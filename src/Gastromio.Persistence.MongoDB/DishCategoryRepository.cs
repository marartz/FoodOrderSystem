using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Gastromio.Core.Application.Ports.Persistence;
using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.DishCategories;
using Gastromio.Core.Domain.Model.Restaurants;
using Gastromio.Core.Domain.Model.Users;
using MongoDB.Driver;

namespace Gastromio.Persistence.MongoDB
{
    public class DishCategoryRepository : IDishCategoryRepository
    {
        private readonly IMongoDatabase database;

        public DishCategoryRepository(IMongoDatabase database)
        {
            this.database = database;
        }

        public async Task<IEnumerable<DishCategory>> FindByRestaurantIdAsync(RestaurantId restaurantId,
            CancellationToken cancellationToken = default)
        {
            var collection = GetCollection();
            var cursor = await collection.FindAsync(
                Builders<DishCategoryModel>.Filter.Eq(en => en.RestaurantId, restaurantId.Value),
                new FindOptions<DishCategoryModel>
                {
                    Sort = Builders<DishCategoryModel>.Sort.Ascending(en => en.OrderNo)
                },
                cancellationToken);
            return cursor.ToEnumerable().Select(FromDocument);
        }

        public async Task<DishCategory> FindByDishCategoryIdAsync(DishCategoryId dishCategoryId,
            CancellationToken cancellationToken = default)
        {
            var collection = GetCollection();
            var cursor = await collection.FindAsync(
                Builders<DishCategoryModel>.Filter.Eq(en => en.Id, dishCategoryId.Value),
                cancellationToken: cancellationToken);
            var document = await cursor.FirstOrDefaultAsync(cancellationToken);
            return document != null ? FromDocument(document) : null;
        }

        public async Task StoreAsync(DishCategory dishCategory, CancellationToken cancellationToken = default)
        {
            var collection = GetCollection();
            var filter = Builders<DishCategoryModel>.Filter.Eq(en => en.Id, dishCategory.Id.Value);
            var document = ToDocument(dishCategory);
            var options = new ReplaceOptions { IsUpsert = true };
            await collection.ReplaceOneAsync(filter, document, options, cancellationToken);
        }

        public async Task RemoveByRestaurantIdAsync(RestaurantId restaurantId, CancellationToken cancellationToken = default)
        {
            var collection = GetCollection();
            await collection.DeleteManyAsync(Builders<DishCategoryModel>.Filter.Eq(en => en.RestaurantId, restaurantId.Value),
                cancellationToken);
        }

        public async Task RemoveAsync(DishCategoryId dishCategoryId, CancellationToken cancellationToken = default)
        {
            var collection = GetCollection();
            await collection.DeleteOneAsync(Builders<DishCategoryModel>.Filter.Eq(en => en.Id, dishCategoryId.Value),
                cancellationToken);
        }

        private IMongoCollection<DishCategoryModel> GetCollection()
        {
            return database.GetCollection<DishCategoryModel>(Constants.DishCategoryCollectionName);
        }

        private static DishCategory FromDocument(DishCategoryModel model)
        {
            return new DishCategory(
                new DishCategoryId(model.Id),
                new RestaurantId(model.RestaurantId),
                model.Name,
                model.OrderNo,
                model.Enabled ?? true,
                model.CreatedOn.ToDateTimeOffset(TimeSpan.Zero),
                new UserId(model.CreatedBy),
                model.UpdatedOn.ToDateTimeOffset(TimeSpan.Zero),
                new UserId(model.UpdatedBy)
            );
        }

        private static DishCategoryModel ToDocument(DishCategory obj)
        {
            return new DishCategoryModel
            {
                Id = obj.Id.Value,
                RestaurantId = obj.RestaurantId.Value,
                Name = obj.Name,
                OrderNo = obj.OrderNo,
                Enabled = obj.Enabled,
                CreatedOn = obj.CreatedOn.UtcDateTime,
                CreatedBy = obj.CreatedBy.Value,
                UpdatedOn = obj.UpdatedOn.UtcDateTime,
                UpdatedBy = obj.UpdatedBy.Value
            };
        }
    }
}
