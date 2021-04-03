using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Gastromio.Core.Application.Commands.AddAdminToRestaurant;
using Gastromio.Core.Domain.Model.Restaurants;
using Gastromio.Core.Domain.Model.Users;
using Gastromio.Domain.TestKit.Application.Ports.Persistence;
using Gastromio.Domain.TestKit.Domain.Model.Restaurants;
using Moq;
using Xunit;

namespace Gastromio.Domain.Tests.Application.Commands.AddAdminToRestaurant
{
    public class AddAdminToRestaurantCommandHandlerTests : CommandHandlerTestBase<AddAdminToRestaurantCommandHandler,
        AddAdminToRestaurantCommand, bool>
    {
        private readonly Fixture fixture;

        public AddAdminToRestaurantCommandHandlerTests()
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
        public async Task HandleAsync_AllValid_AddsAdminToRestaurantAndReturnsSuccess()
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
                fixture.Restaurant.Administrators.Should().BeEquivalentTo(fixture.UserId, fixture.AdministratorId);
                fixture.RestaurantRepositoryMock.VerifyStoreAsync(fixture.Restaurant, Times.Once);
            }
        }

        protected override
            CommandHandlerTestFixtureBase<AddAdminToRestaurantCommandHandler, AddAdminToRestaurantCommand, bool> FixtureBase
        {
            get { return fixture; }
        }

        private sealed class Fixture : CommandHandlerTestFixtureBase<AddAdminToRestaurantCommandHandler,
            AddAdminToRestaurantCommand, bool>
        {
            public Fixture(Role? minimumRole) : base(minimumRole)
            {
                RestaurantRepositoryMock = new RestaurantRepositoryMock(MockBehavior.Strict);
            }

            public RestaurantRepositoryMock RestaurantRepositoryMock { get; }

            public Restaurant Restaurant { get; private set; }

            public UserId AdministratorId { get; private set; }

            public override AddAdminToRestaurantCommandHandler CreateTestObject()
            {
                return new AddAdminToRestaurantCommandHandler(
                    RestaurantRepositoryMock.Object
                );
            }

            public override AddAdminToRestaurantCommand CreateSuccessfulCommand()
            {
                return new AddAdminToRestaurantCommand(Restaurant.Id, AdministratorId);
            }

            public void SetupRandomRestaurant()
            {
                Restaurant = new RestaurantBuilder()
                    .WithAdministrators(new HashSet<UserId>
                    {
                        UserId
                    })
                    .WithValidConstrains()
                    .Create();
            }

            public void SetupRandomAdministrator()
            {
                AdministratorId = new UserId(Guid.NewGuid());
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
                SetupRandomAdministrator();
                SetupRestaurantRepositoryFindingRestaurant();
                SetupRestaurantRepositoryStoringRestaurant();
            }
        }
    }
}
