using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Gastromio.Core.Application.Ports.Persistence;
using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.Users;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Gastromio.Persistence.MongoDB
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoDatabase database;

        public UserRepository(IMongoDatabase database)
        {
            this.database = database;
        }

        public async Task<IEnumerable<User>> FindAllAsync(CancellationToken cancellationToken = default)
        {
            var collection = GetCollection();

            var options = new FindOptions<UserModel>
            {
                Sort = Builders<UserModel>.Sort.Ascending(en => en.Email),
            };

            var cursor = await collection.FindAsync(new BsonDocument(), options, cancellationToken);
            return cursor.ToEnumerable().Select(FromDocument);
        }

        public async Task<IEnumerable<User>> SearchAsync(string searchPhrase, CancellationToken cancellationToken = default)
        {
            var collection = GetCollection();

            FilterDefinition<UserModel> filter;
            if (!string.IsNullOrEmpty(searchPhrase))
            {
                var bsonRegEx = new BsonRegularExpression($".*{Regex.Escape(searchPhrase)}.*", "i");
                filter = Builders<UserModel>.Filter.Regex(en => en.Email, bsonRegEx);
            }
            else
            {
                filter = new BsonDocument();
            }

            var options = new FindOptions<UserModel>
            {
                Sort = Builders<UserModel>.Sort.Ascending(en => en.Email)
            };

            var cursor = await collection.FindAsync(filter, options, cancellationToken);
            return cursor.ToEnumerable().Select(FromDocument);
        }

        public async Task<(long total, IEnumerable<User> items)> SearchPagedAsync(string searchPhrase, Role? role,
            int skip, int take, CancellationToken cancellationToken = default)
        {
            var collection = GetCollection();

            FilterDefinition<UserModel> filter;
            if (!string.IsNullOrEmpty(searchPhrase))
            {
                var bsonRegEx = new BsonRegularExpression($".*{Regex.Escape(searchPhrase)}.*", "i");
                filter = Builders<UserModel>.Filter.Regex(en => en.Email, bsonRegEx);
            }
            else
            {
                filter = new BsonDocument();
            }

            if (role.HasValue)
            {
                filter = filter & Builders<UserModel>.Filter.Eq(en => en.Role, ToDbRole(role.Value));
            }

            var total = await collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);

            if (take == 0)
                return (total, Enumerable.Empty<User>());

            var findOptions = new FindOptions<UserModel>
            {
                Sort = Builders<UserModel>.Sort.Ascending(en => en.Email)
            };
            if (skip > 0)
                findOptions.Skip = skip;
            if (take >= 0)
                findOptions.Limit = take;

            var cursor = await collection.FindAsync(filter, findOptions, cancellationToken);
            return (total, cursor.ToEnumerable().Select(FromDocument));
        }

        public async Task<IEnumerable<User>> FindByRoleAsync(Role role, CancellationToken cancellationToken = default)
        {
            var collection = GetCollection();
            var cursor = await collection.FindAsync(Builders<UserModel>.Filter.Eq(en => en.Role, ToDbRole(role)),
                new FindOptions<UserModel>
                {
                    Sort = Builders<UserModel>.Sort.Ascending(en => en.Email)
                },
                cancellationToken);
            return cursor.ToEnumerable().Select(FromDocument);
        }

        public async Task<User> FindByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            var collection = GetCollection();
            var cursor = await collection.FindAsync(Builders<UserModel>.Filter.Eq(en => en.Email, email),
                cancellationToken: cancellationToken);
            var document = await cursor.FirstOrDefaultAsync(cancellationToken);
            return document != null ? FromDocument(document) : null;
        }

        public async Task<User> FindByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
        {
            var collection = GetCollection();
            var cursor = await collection.FindAsync(Builders<UserModel>.Filter.Eq(en => en.Id, userId.Value),
                cancellationToken: cancellationToken);
            var document = await cursor.FirstOrDefaultAsync(cancellationToken);
            return document != null ? FromDocument(document) : null;
        }

        public async Task<IEnumerable<User>> FindByUserIdsAsync(IEnumerable<UserId> userIds, CancellationToken cancellationToken = default)
        {
            var collection = GetCollection();

            var filter = FilterDefinition<UserModel>.Empty;
            foreach (var userId in userIds)
            {
                filter |= Builders<UserModel>.Filter.Eq(en => en.Id, userId.Value);
            }

            var cursor = await collection.FindAsync(filter, cancellationToken: cancellationToken);

            return (await cursor.ToListAsync(cancellationToken)).Select(FromDocument);
        }

        public async Task StoreAsync(User user, CancellationToken cancellationToken = default)
        {
            var collection = GetCollection();
            var filter = Builders<UserModel>.Filter.Eq(en => en.Id, user.Id.Value);
            var document = ToDocument(user);
            var options = new ReplaceOptions { IsUpsert = true };
            await collection.ReplaceOneAsync(filter, document, options, cancellationToken);
        }

        public async Task RemoveAsync(UserId userId, CancellationToken cancellationToken = default)
        {
            var collection = GetCollection();
            await collection.DeleteOneAsync(Builders<UserModel>.Filter.Eq(en => en.Id, userId.Value),
                cancellationToken);
        }

        private IMongoCollection<UserModel> GetCollection()
        {
            return database.GetCollection<UserModel>(Constants.UserCollectionName);
        }

        private static Role FromDbRole(string role)
        {
            switch (role)
            {
                case "Customer":
                    return Role.Customer;
                case "RestaurantAdmin":
                    return Role.RestaurantAdmin;
                case "SystemAdmin":
                    return Role.SystemAdmin;
                default:
                    throw new InvalidOperationException($"unknown role with id: {role}");
            }
        }

        private static User FromDocument(UserModel model)
        {
            var role = FromDbRole(model.Role);

            return new User(new UserId(model.Id),
                role,
                model.Email,
                model.PasswordSalt,
                model.PasswordHash,
                model.PasswordResetCode,
                model.PasswordResetExpiration?.ToDateTimeOffset(TimeSpan.Zero),
                model.CreatedOn.ToDateTimeOffset(TimeSpan.Zero),
                new UserId(model.CreatedBy),
                model.UpdatedOn.ToDateTimeOffset(TimeSpan.Zero),
                new UserId(model.UpdatedBy)
            );
        }

        private static string ToDbRole(Role role)
        {
            switch (role)
            {
                case Role.Customer:
                    return "Customer";
                case Role.RestaurantAdmin:
                    return "RestaurantAdmin";
                case Role.SystemAdmin:
                    return "SystemAdmin";
                default:
                    throw new InvalidOperationException($"unknown role: {role}");
            }
        }

        private static UserModel ToDocument(User obj)
        {
            return new UserModel
            {
                Id = obj.Id.Value,
                Role = ToDbRole(obj.Role),
                Email = obj.Email,
                PasswordSalt = obj.PasswordSalt,
                PasswordHash = obj.PasswordHash,
                PasswordResetCode = obj.PasswordResetCode,
                PasswordResetExpiration = obj.PasswordResetExpiration?.UtcDateTime,
                CreatedOn = obj.CreatedOn.UtcDateTime,
                CreatedBy = obj.CreatedBy.Value,
                UpdatedOn = obj.UpdatedOn.UtcDateTime,
                UpdatedBy = obj.UpdatedBy.Value
            };
        }
    }
}
