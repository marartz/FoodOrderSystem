using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Gastromio.Core.Application.Ports.Persistence;
using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.RestaurantImages;
using Gastromio.Core.Domain.Model.Restaurants;
using Gastromio.Core.Domain.Model.Users;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Gastromio.Persistence.MongoDB
{
    public class RestaurantImageRepository : IRestaurantImageRepository
    {
        private readonly IMongoDatabase database;

        public RestaurantImageRepository(IMongoDatabase database)
        {
            this.database = database;
        }

        public async Task<RestaurantImage> FindByRestaurantImageIdAsync(RestaurantImageId restaurantImageId,
            CancellationToken cancellationToken = default)
        {
            var collection = GetCollection();
            var cursor = await collection.FindAsync(
                Builders<RestaurantImageModel>.Filter.Eq(en => en.Id, restaurantImageId.Value),
                cancellationToken: cancellationToken);
            var document = await cursor.FirstOrDefaultAsync(cancellationToken);
            return document != null ? FromDocument(document) : null;
        }

        public async Task<RestaurantImage> FindByRestaurantIdAndTypeAsync(RestaurantId restaurantId, string type,
            CancellationToken cancellationToken = default)
        {
            var collection = GetCollection();
            var filter = Builders<RestaurantImageModel>.Filter.Eq(en => en.RestaurantId, restaurantId.Value);
            filter &= Builders<RestaurantImageModel>.Filter.Eq(en => en.Type, type);
            var cursor = await collection.FindAsync(filter, cancellationToken: cancellationToken);
            var document = await cursor.FirstOrDefaultAsync(cancellationToken);
            return document != null ? FromDocument(document) : null;
        }

        public async Task<IEnumerable<RestaurantImage>> FindByRestaurantIdAsync(RestaurantId restaurantId,
            CancellationToken cancellationToken = default)
        {
            var collection = GetCollection();
            var cursor = await collection.FindAsync(
                Builders<RestaurantImageModel>.Filter.Eq(en => en.RestaurantId, restaurantId.Value),
                new FindOptions<RestaurantImageModel>
                {
                    Sort = Builders<RestaurantImageModel>.Sort.Ascending(en => en.Type)
                },
                cancellationToken);
            return cursor.ToEnumerable().Select(FromDocument);
        }

        public async Task<IEnumerable<string>> FindTypesByRestaurantIdAsync(RestaurantId restaurantId, CancellationToken cancellationToken = default)
        {
            var collection = GetCollection();
            var cursor = await collection.FindAsync(
                Builders<RestaurantImageModel>.Filter.Eq(en => en.RestaurantId, restaurantId.Value),
                new FindOptions<RestaurantImageModel>
                {
                    Sort = Builders<RestaurantImageModel>.Sort.Ascending(en => en.Type),
                    Projection = Builders<RestaurantImageModel>.Projection.Include(en => en.Type)
                },
                cancellationToken);
            return cursor.ToEnumerable().Select(en => en.Type);
        }

        public async Task<IDictionary<RestaurantId, IEnumerable<string>>> FindTypesByRestaurantIdsAsync(IEnumerable<RestaurantId> restaurantIds, CancellationToken cancellationToken = default)
        {
            var collection = GetCollection();

            var filter = FilterDefinition<RestaurantImageModel>.Empty;
            foreach (var restaurantId in restaurantIds)
            {
                filter |= Builders<RestaurantImageModel>.Filter.Eq(en => en.RestaurantId, restaurantId.Value);
            }

            var cursor = await collection.FindAsync(
                filter,
                new FindOptions<RestaurantImageModel>
                {
                    Sort = Builders<RestaurantImageModel>.Sort.Ascending(en => en.Type),
                    Projection = Builders<RestaurantImageModel>.Projection.Combine(
                        Builders<RestaurantImageModel>.Projection.Include(en => en.RestaurantId),
                        Builders<RestaurantImageModel>.Projection.Include(en => en.Type)
                    )
                },
                cancellationToken);

            return cursor.ToEnumerable()
                .GroupBy(row => row.RestaurantId)
                .ToDictionary(
                    group => new RestaurantId(group.Key),
                    group => group.Select(en => en.Type)
                );
        }

        public async Task StoreAsync(RestaurantImage restaurantImage, CancellationToken cancellationToken = default)
        {
            var collection = GetCollection();
            var filter = Builders<RestaurantImageModel>.Filter.Eq(en => en.Id, restaurantImage.Id.Value);
            filter &= Builders<RestaurantImageModel>.Filter.Eq(en => en.Type, restaurantImage.ToJson());
            var document = ToDocument(restaurantImage);
            var options = new ReplaceOptions {IsUpsert = true};
            await collection.ReplaceOneAsync(filter, document, options, cancellationToken);
        }

        public async Task RemoveByRestaurantImageId(RestaurantImageId restaurantImageId, string type,
            CancellationToken cancellationToken = default)
        {
            var collection = GetCollection();
            await collection.DeleteOneAsync(
                Builders<RestaurantImageModel>.Filter.Eq(en => en.Id, restaurantImageId.Value), cancellationToken);
        }

        public async Task RemoveByRestaurantIdAndTypeAsync(RestaurantId restaurantId, string type,
            CancellationToken cancellationToken = default)
        {
            var collection = GetCollection();
            var filter = Builders<RestaurantImageModel>.Filter.Eq(en => en.RestaurantId, restaurantId.Value);
            filter &= Builders<RestaurantImageModel>.Filter.Eq(en => en.Type, type);
            await collection.DeleteManyAsync(filter, cancellationToken);
        }

        public async Task RemoveByRestaurantIdAsync(RestaurantId restaurantId, CancellationToken cancellationToken = default)
        {
            var collection = GetCollection();
            var filter = Builders<RestaurantImageModel>.Filter.Eq(en => en.RestaurantId, restaurantId.Value);
            await collection.DeleteManyAsync(filter, cancellationToken);
        }

        private IMongoCollection<RestaurantImageModel> GetCollection()
        {
            return database.GetCollection<RestaurantImageModel>(Constants.RestaurantImageCollectionName);
        }

        private static RestaurantImage FromDocument(RestaurantImageModel document)
        {
            return new RestaurantImage(
                new RestaurantImageId(document.Id),
                new RestaurantId(document.RestaurantId),
                document.Type,
                document.Data,
                document.CreatedOn.ToDateTimeOffset(TimeSpan.Zero),
                new UserId(document.CreatedBy),
                document.UpdatedOn.ToDateTimeOffset(TimeSpan.Zero),
                new UserId(document.UpdatedBy)
            );
        }

        private static RestaurantImageModel ToDocument(RestaurantImage obj)
        {
            return new RestaurantImageModel
            {
                Id = obj.Id.Value,
                RestaurantId = obj.RestaurantId.Value,
                Type = obj.Type,
                Data = obj.Data,
                CreatedOn = obj.CreatedOn.UtcDateTime,
                CreatedBy = obj.CreatedBy.Value,
                UpdatedOn = obj.UpdatedOn.UtcDateTime,
                UpdatedBy = obj.UpdatedBy.Value
            };
        }
    }
}
