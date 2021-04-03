using Gastromio.Core.Application.Queries;
using Gastromio.Core.Domain.Model.Users;
using Gastromio.Domain.TestKit.Domain.Model.Users;

namespace Gastromio.Domain.Tests.Application
{
    public abstract class QueryHandlerTestFixtureBase<THandler, TQuery, TResult>
        where THandler : IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
    {
        protected QueryHandlerTestFixtureBase(Role? minimumRole)
        {
            MinimumRole = minimumRole;

            if (!MinimumRole.HasValue)
                return;

            UserId = new UserIdBuilder()
                .Create();

            UserWithCustomerRole = new UserBuilder()
                .WithId(UserId)
                .WithRole(Role.Customer)
                .Create();

            UserWithRestaurantAdminRole = new UserBuilder()
                .WithId(UserId)
                .WithRole(Role.RestaurantAdmin)
                .Create();

            UserWithSystemAdminRole = new UserBuilder()
                .WithId(UserId)
                .WithRole(Role.SystemAdmin)
                .Create();

            UserWithMinimumRole = new UserBuilder()
                .WithId(UserId)
                .WithRole(MinimumRole.Value)
                .Create();
        }

        public Role? MinimumRole { get; }

        public UserId UserId { get; }

        public User UserWithCustomerRole { get; }

        public User UserWithRestaurantAdminRole { get; }

        public User UserWithSystemAdminRole { get; }

        public User UserWithMinimumRole { get; }

        public abstract THandler CreateTestObject();

        public abstract TQuery CreateSuccessfulQuery();

        public abstract void SetupForSuccessfulQueryExecution(Role? role);
    }
}
