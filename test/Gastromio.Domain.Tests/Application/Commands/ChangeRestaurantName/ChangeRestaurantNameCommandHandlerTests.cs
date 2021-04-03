using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Gastromio.Core.Application.Commands.ChangeRestaurantName;
using Gastromio.Core.Domain.Model.Restaurants;
using Gastromio.Core.Domain.Model.Users;
using Gastromio.Domain.TestKit.Application.Ports.Persistence;
using Gastromio.Domain.TestKit.Common;
using Gastromio.Domain.TestKit.Domain.Model.Restaurants;
using Moq;
using Xunit;

namespace Gastromio.Domain.Tests.Application.Commands.ChangeRestaurantName
{
    public class ChangeRestaurantNameCommandHandlerTests : CommandHandlerTestBase<ChangeRestaurantNameCommandHandler,
        ChangeRestaurantNameCommand, bool>
    {
        private readonly Fixture fixture;

        public ChangeRestaurantNameCommandHandlerTests()
        {
            fixture = new Fixture(Role.SystemAdmin);
        }

        [Fact]
        public async Task HandleAsync_RestaurantNotKnown_ReturnsFailure()
        {
            // Arrange
            fixture.SetupRandomRestaurant(fixture.MinimumRole);
            fixture.SetupRandomName();
            fixture.SetupRestaurantRepositoryNotFindingRestaurantById();

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
        public async Task HandleAsync_AllValid_ChangesNameOfRestaurantAndReturnsSuccess()
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
                fixture.Restaurant.Name.Should().Be(fixture.Name);
                fixture.RestaurantRepositoryMock.VerifyStoreAsync(fixture.Restaurant, Times.Once);
            }
        }

        protected override
            CommandHandlerTestFixtureBase<ChangeRestaurantNameCommandHandler, ChangeRestaurantNameCommand, bool> FixtureBase
        {
            get { return fixture; }
        }

        private sealed class Fixture : CommandHandlerTestFixtureBase<ChangeRestaurantNameCommandHandler,
            ChangeRestaurantNameCommand, bool>
        {
            public Fixture(Role? minimumRole) : base(minimumRole)
            {
                RestaurantRepositoryMock = new RestaurantRepositoryMock(MockBehavior.Strict);
            }

            public RestaurantRepositoryMock RestaurantRepositoryMock { get; }

            public Restaurant Restaurant { get; private set; }

            public string Name { get; private set; }

            public override ChangeRestaurantNameCommandHandler CreateTestObject()
            {
                return new ChangeRestaurantNameCommandHandler(
                    RestaurantRepositoryMock.Object
                );
            }

            public override ChangeRestaurantNameCommand CreateSuccessfulCommand()
            {
                return new ChangeRestaurantNameCommand(Restaurant.Id, Name);
            }

            public void SetupRandomRestaurant(Role? role)
            {
                var builder = new RestaurantBuilder();

                if (role == Role.RestaurantAdmin)
                {
                    builder = builder
                        .WithAdministrators(new HashSet<UserId> {UserId});
                }

                Restaurant = builder
                    .WithValidConstrains()
                    .Create();
            }

            public void SetupRandomName()
            {
                Name = RandomStringBuilder.BuildWithLength(20);
            }

            public void SetupRestaurantRepositoryFindingRestaurantById()
            {
                RestaurantRepositoryMock.SetupFindByRestaurantIdAsync(Restaurant.Id)
                    .ReturnsAsync(Restaurant);
            }

            public void SetupRestaurantRepositoryNotFindingRestaurantById()
            {
                RestaurantRepositoryMock.SetupFindByRestaurantIdAsync(Restaurant.Id)
                    .ReturnsAsync((Restaurant)null);
            }

            public void SetupRestaurantRepositoryFindingRestaurantByName()
            {
                RestaurantRepositoryMock.SetupFindByRestaurantNameAsync(Name)
                    .ReturnsAsync(new []{Restaurant});
            }

            public void SetupRestaurantRepositoryNotFindingRestaurantByName()
            {
                RestaurantRepositoryMock.SetupFindByRestaurantNameAsync(Name)
                    .ReturnsAsync(Enumerable.Empty<Restaurant>());
            }

            public void SetupRestaurantRepositoryStoringRestaurant()
            {
                RestaurantRepositoryMock.SetupStoreAsync(Restaurant)
                    .Returns(Task.CompletedTask);
            }

            public override void SetupForSuccessfulCommandExecution(Role? role)
            {
                SetupRandomRestaurant(role);
                SetupRandomName();
                SetupRestaurantRepositoryFindingRestaurantById();
                SetupRestaurantRepositoryNotFindingRestaurantByName();
                SetupRestaurantRepositoryStoringRestaurant();
            }
        }
    }
}
