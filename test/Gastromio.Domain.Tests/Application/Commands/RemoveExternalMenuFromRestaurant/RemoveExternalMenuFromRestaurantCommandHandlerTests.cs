using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Gastromio.Core.Application.Commands.RemoveExternalMenuFromRestaurant;
using Gastromio.Core.Domain.Model.Restaurants;
using Gastromio.Core.Domain.Model.Users;
using Gastromio.Domain.TestKit.Application.Ports.Persistence;
using Gastromio.Domain.TestKit.Domain.Model.Restaurants;
using Moq;
using Xunit;

namespace Gastromio.Domain.Tests.Application.Commands.RemoveExternalMenuFromRestaurant
{
    public class RemoveExternalMenuFromRestaurantCommandHandlerTests : CommandHandlerTestBase<RemoveExternalMenuFromRestaurantCommandHandler,
        RemoveExternalMenuFromRestaurantCommand, bool>
    {
        private readonly Fixture fixture;

        public RemoveExternalMenuFromRestaurantCommandHandlerTests()
        {
            fixture = new Fixture(Role.RestaurantAdmin);
        }

        [Fact]
        public async Task HandleAsync_RestaurantNotKnown_ReturnsFailure()
        {
            // Arrange
            fixture.SetupRandomExternalMenu();
            fixture.SetupRandomRestaurant(fixture.MinimumRole);
            fixture.SetupRestaurantRepositoryNotFindingRestaurant();

            var testObject = fixture.CreateTestObject();
            var command = fixture.CreateSuccessfulCommand();

            // Act
            var result = await testObject.HandleAsync(command, fixture.UserWithMinimumRole, CancellationToken.None);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public async Task HandleAsync_AllValid_RemovesExternalMenuFromRestaurantAndReturnsSuccess()
        {
            // Arrange
            fixture.SetupForSuccessfulCommandExecution(fixture.MinimumRole);

            var testObject = fixture.CreateTestObject();
            var command = fixture.CreateSuccessfulCommand();

            // Act
            var result = await testObject.HandleAsync(command, fixture.UserWithMinimumRole, CancellationToken.None);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                fixture.Restaurant.ExternalMenus.Should().BeEmpty();
                fixture.RestaurantRepositoryMock.VerifyStoreAsync(fixture.Restaurant, Times.Once);
            }
        }

        protected override
            CommandHandlerTestFixtureBase<RemoveExternalMenuFromRestaurantCommandHandler, RemoveExternalMenuFromRestaurantCommand, bool> FixtureBase
        {
            get { return fixture; }
        }

        private sealed class Fixture : CommandHandlerTestFixtureBase<RemoveExternalMenuFromRestaurantCommandHandler,
            RemoveExternalMenuFromRestaurantCommand, bool>
        {
            public Fixture(Role? minimumRole) : base(minimumRole)
            {
                RestaurantRepositoryMock = new RestaurantRepositoryMock(MockBehavior.Strict);
            }

            public RestaurantRepositoryMock RestaurantRepositoryMock { get; }

            public Restaurant Restaurant { get; private set; }

            public ExternalMenu ExternalMenu { get; private set; }

            public override RemoveExternalMenuFromRestaurantCommandHandler CreateTestObject()
            {
                return new RemoveExternalMenuFromRestaurantCommandHandler(
                    RestaurantRepositoryMock.Object
                );
            }

            public override RemoveExternalMenuFromRestaurantCommand CreateSuccessfulCommand()
            {
                return new RemoveExternalMenuFromRestaurantCommand(Restaurant.Id, ExternalMenu.Id);
            }

            public void SetupRandomExternalMenu()
            {
                ExternalMenu = new ExternalMenuBuilder()
                    .Create();
            }

            public void SetupRandomRestaurant(Role? role)
            {
                var builder = new RestaurantBuilder()
                    .WithExternalMenus(new[] {ExternalMenu});

                if (role == Role.RestaurantAdmin)
                {
                    builder = builder
                        .WithAdministrators(new HashSet<UserId> {UserId});
                }

                Restaurant = builder
                    .WithValidConstrains()
                    .Create();
            }

            public void SetupRestaurantRepositoryFindingRestaurant()
            {
                RestaurantRepositoryMock.SetupFindByRestaurantIdAsync(Restaurant.Id)
                    .ReturnsAsync(Restaurant);
            }

            public void SetupRestaurantRepositoryNotFindingRestaurant()
            {
                RestaurantRepositoryMock.SetupFindByRestaurantIdAsync(Restaurant.Id)
                    .ReturnsAsync((Restaurant)null);
            }

            public void SetupRestaurantRepositoryStoringRestaurant()
            {
                RestaurantRepositoryMock.SetupStoreAsync(Restaurant)
                    .Returns(Task.CompletedTask);
            }

            public override void SetupForSuccessfulCommandExecution(Role? role)
            {
                SetupRandomExternalMenu();
                SetupRandomRestaurant(role);
                SetupRestaurantRepositoryFindingRestaurant();
                SetupRestaurantRepositoryStoringRestaurant();
            }
        }
    }
}
