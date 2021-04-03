using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Gastromio.Core.Application.Ports.Persistence;
using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.Cuisines;
using Gastromio.Core.Domain.Model.Users;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Gastromio.Persistence.MongoDB
{
    public class CuisineRepository : ICuisineRepository
    {
        private readonly IMongoDatabase database;

        public CuisineRepository(IMongoDatabase database)
        {
            this.database = database;
        }

        public async Task<IEnumerable<Cuisine>> FindAllAsync(CancellationToken cancellationToken = default)
        {
            var collection = GetCollection();
            var cursor = await collection.FindAsync(new BsonDocument(),
                new FindOptions<CuisineModel>
                {
                    Sort = Builders<CuisineModel>.Sort.Ascending(en => en.Name)
                },
                cancellationToken);
            return cursor.ToEnumerable().Select(FromDocument);
        }

        public async Task<Cuisine> FindByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            var collection = GetCollection();

            var cursor = await collection.FindAsync(Builders<CuisineModel>.Filter.Eq(en => en.Name, name),
                cancellationToken: cancellationToken);
            var document = await cursor.FirstOrDefaultAsync(cancellationToken);
            return document != null ? FromDocument(document) : null;
        }

        public async Task<Cuisine> FindByCuisineIdAsync(CuisineId cuisineId, CancellationToken cancellationToken = default)
        {
            var collection = GetCollection();
            var cursor = await collection.FindAsync(Builders<CuisineModel>.Filter.Eq(en => en.Id, cuisineId.Value),
                cancellationToken: cancellationToken);
            var document = await cursor.FirstOrDefaultAsync(cancellationToken);
            return document != null ? FromDocument(document) : null;
        }

        public async Task StoreAsync(Cuisine cuisine, CancellationToken cancellationToken = default)
        {
            var collection = GetCollection();
            var filter = Builders<CuisineModel>.Filter.Eq(en => en.Id, cuisine.Id.Value);
            var document = ToDocument(cuisine);
            var options = new ReplaceOptions { IsUpsert = true };
            await collection.ReplaceOneAsync(filter, document, options, cancellationToken);
        }

        public async Task RemoveAsync(CuisineId cuisineId, CancellationToken cancellationToken = default)
        {
            var collection = GetCollection();
            await collection.DeleteOneAsync(Builders<CuisineModel>.Filter.Eq(en => en.Id, cuisineId.Value),
                cancellationToken);
        }

        private IMongoCollection<CuisineModel> GetCollection()
        {
            return database.GetCollection<CuisineModel>(Constants.CuisineCollectionName);
        }

        private static Cuisine FromDocument(CuisineModel row)
        {
            return new Cuisine(
                new CuisineId(row.Id),
                row.Name,
                row.CreatedOn.ToDateTimeOffset(TimeSpan.Zero),
                new UserId(row.CreatedBy),
                row.UpdatedOn.ToDateTimeOffset(TimeSpan.Zero),
                new UserId(row.UpdatedBy)
            );
        }

        private static CuisineModel ToDocument(Cuisine obj)
        {
            return new CuisineModel
            {
                Id = obj.Id.Value,
                Name = obj.Name,
                CreatedOn = obj.CreatedOn.UtcDateTime,
                CreatedBy = obj.CreatedBy.Value,
                UpdatedOn = obj.UpdatedOn.UtcDateTime,
                UpdatedBy = obj.UpdatedBy.Value
            };
        }
    }
}
