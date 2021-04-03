using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Gastromio.Core.Application.Commands.DisableSupportForRestaurant;
using Gastromio.Core.Domain.Model.Restaurants;
using Gastromio.Core.Domain.Model.Users;
using Gastromio.Domain.TestKit.Application.Ports.Persistence;
using Gastromio.Domain.TestKit.Domain.Model.Restaurants;
using Moq;
using Xunit;

namespace Gastromio.Domain.Tests.Application.Commands.DisableSupportForRestaurant
{
    public class DisableSupportForRestaurantCommandHandlerTests : CommandHandlerTestBase<DisableSupportForRestaurantCommandHandler,
        DisableSupportForRestaurantCommand, bool>
    {
        private readonly Fixture fixture;

        public DisableSupportForRestaurantCommandHandlerTests()
        {
            fixture = new Fixture(Role.SystemAdmin);
        }

        [Fact]
        public async Task HandleAsync_RestaurantNotFound_ReturnsFailure()
        {
            // Arrange
            fixture.SetupRandomRestaurant();
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
        public async Task HandleAsync_AllValid_DisablesSupportForRestaurantAndReturnsSuccess()
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
                fixture.Restaurant.NeedsSupport.Should().BeFalse();
                fixture.RestaurantRepositoryMock.VerifyStoreAsync(fixture.Restaurant, Times.Once);
            }
        }

        protected override
            CommandHandlerTestFixtureBase<DisableSupportForRestaurantCommandHandler, DisableSupportForRestaurantCommand, bool> FixtureBase
        {
            get { return fixture; }
        }

        private sealed class Fixture : CommandHandlerTestFixtureBase<DisableSupportForRestaurantCommandHandler,
            DisableSupportForRestaurantCommand, bool>
        {
            public Fixture(Role? minimumRole) : base(minimumRole)
            {
                RestaurantRepositoryMock = new RestaurantRepositoryMock(MockBehavior.Strict);
            }

            public RestaurantRepositoryMock RestaurantRepositoryMock { get; }

            public Restaurant Restaurant { get; private set; }

            public override DisableSupportForRestaurantCommandHandler CreateTestObject()
            {
                return new DisableSupportForRestaurantCommandHandler(
                    RestaurantRepositoryMock.Object
                );
            }

            public override DisableSupportForRestaurantCommand CreateSuccessfulCommand()
            {
                return new DisableSupportForRestaurantCommand(Restaurant.Id);
            }

            public void SetupRandomRestaurant()
            {
                Restaurant = new RestaurantBuilder()
                    .WithAdministrators(new HashSet<UserId>
                    {
                        UserId
                    })
                    .WithNeedsSupport(true)
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
                SetupRandomRestaurant();
                SetupRestaurantRepositoryFindingRestaurant();
                SetupRestaurantRepositoryStoringRestaurant();
            }
        }
    }
}
