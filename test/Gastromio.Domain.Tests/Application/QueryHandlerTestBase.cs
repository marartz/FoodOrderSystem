using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Gastromio.Core.Application.Queries;
using Gastromio.Core.Domain.Model.Users;
using Xunit;

namespace Gastromio.Domain.Tests.Application
{
    public abstract class QueryHandlerTestBase<THandler, TQuery, TResult>
        where THandler : IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
    {
        protected abstract QueryHandlerTestFixtureBase<THandler, TQuery, TResult> FixtureBase
        {
            get;
        }

        [Fact]
        public async Task HandleAsync_QueryNull_ThrowsArgumentNullException()
        {
            // Arrange
            var testObject = FixtureBase.CreateTestObject();

            // Act
            Func<Task> act = async () =>
                await testObject.HandleAsync(default, FixtureBase.UserWithMinimumRole, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task HandleAsync_NotAuthorized_ChecksAuthorizationCorrectly()
        {
            // Arrange
            FixtureBase.SetupForSuccessfulQueryExecution(null);
            var testObject = FixtureBase.CreateTestObject();

            // Act
            var result = await testObject.HandleAsync(FixtureBase.CreateSuccessfulQuery(),
                null, CancellationToken.None);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                if (FixtureBase.MinimumRole.HasValue)
                {
                    result?.IsSuccess.Should().BeFalse();
                }
                else
                {
                    result?.IsSuccess.Should().BeTrue();
                }
            }
        }

        [Fact]
        public async Task HandleAsync_AuthorizedAsCustomer_ChecksAuthorizationCorrectly()
        {
            // Arrange
            FixtureBase.SetupForSuccessfulQueryExecution(Role.Customer);
            var testObject = FixtureBase.CreateTestObject();

            // Act
            var result = await testObject.HandleAsync(FixtureBase.CreateSuccessfulQuery(),
                FixtureBase.UserWithCustomerRole, CancellationToken.None);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                if (FixtureBase.MinimumRole.HasValue && FixtureBase.MinimumRole.Value > Role.Customer)
                {
                    result?.IsSuccess.Should().BeFalse();
                }
                else
                {
                    result?.IsSuccess.Should().BeTrue();
                }
            }
        }

        [Fact]
        public async Task HandleAsync_AuthorizedAsRestaurantAdmin_ChecksAuthorizationCorrectly()
        {
            // Arrange
            FixtureBase.SetupForSuccessfulQueryExecution(Role.RestaurantAdmin);
            var testObject = FixtureBase.CreateTestObject();

            // Act
            var result = await testObject.HandleAsync(FixtureBase.CreateSuccessfulQuery(),
                FixtureBase.UserWithRestaurantAdminRole, CancellationToken.None);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                if (FixtureBase.MinimumRole.HasValue && FixtureBase.MinimumRole.Value > Role.RestaurantAdmin)
                {
                    result?.IsSuccess.Should().BeFalse();
                }
                else
                {
                    result?.IsSuccess.Should().BeTrue();
                }
            }
        }

        [Fact]
        public async Task HandleAsync_AuthorizedAsSystemAdmin_ChecksAuthorizationCorrectly()
        {
            // Arrange
            FixtureBase.SetupForSuccessfulQueryExecution(Role.SystemAdmin);
            var testObject = FixtureBase.CreateTestObject();

            // Act
            var result = await testObject.HandleAsync(FixtureBase.CreateSuccessfulQuery(),
                FixtureBase.UserWithSystemAdminRole, CancellationToken.None);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                if (FixtureBase.MinimumRole.HasValue && FixtureBase.MinimumRole.Value > Role.SystemAdmin)
                {
                    result?.IsSuccess.Should().BeFalse();
                }
                else
                {
                    result?.IsSuccess.Should().BeTrue();
                }
            }
        }
    }
}
