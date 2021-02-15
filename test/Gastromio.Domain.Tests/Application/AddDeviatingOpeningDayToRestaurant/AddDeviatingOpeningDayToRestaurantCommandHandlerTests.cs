using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Gastromio.Core.Application.Commands.AddDeviatingOpeningDayToRestaurant;
using Gastromio.Core.Domain.Model.Restaurants;
using Gastromio.Core.Domain.Model.Users;
using Gastromio.Domain.TestKit.Domain.Model.Restaurants;
using Moq;
using Xunit;

namespace Gastromio.Domain.Tests.Application.AddDeviatingOpeningDayToRestaurant
{
    public class AddDeviatingOpeningDayToRestaurantCommandHandlerTests : CommandHandlerTestBase<AddDeviatingOpeningDayToRestaurantCommandHandler,
        AddDeviatingOpeningDayToRestaurantCommand, bool>
    {
        private readonly Fixture fixture;

        public AddDeviatingOpeningDayToRestaurantCommandHandlerTests()
        {
            fixture = new Fixture(Role.SystemAdmin);
        }

        [Fact]
        public async Task HandleAsync_RestaurantNotKnown_ReturnsFailure()
        {
            // Arrange
            fixture.SetupRandomRestaurant();
            fixture.SetupRandomDeviatingOpeningDay();
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
        public async Task HandleAsync_AllValid_AddsDeviatingDaysReturnsSuccess()
        {
            // Arrange
            fixture.SetupForSuccessfulCommandExecution();

            var testObject = fixture.CreateTestObject();
            var command = fixture.CreateSuccessfulCommand();

            // Act
            var result = await testObject.HandleAsync(command, fixture.UserWithMinimumRole, CancellationToken.None);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                fixture.Restaurant.DeviatingOpeningDays.Select(en => en.Value).Should()
                    .BeEquivalentTo(fixture.DeviatingOpeningDay);
            }
        }

        protected override
            CommandHandlerTestFixtureBase<AddDeviatingOpeningDayToRestaurantCommandHandler, AddDeviatingOpeningDayToRestaurantCommand, bool> FixtureBase
        {
            get { return fixture; }
        }

        private sealed class Fixture : CommandHandlerTestFixtureBase<AddDeviatingOpeningDayToRestaurantCommandHandler,
            AddDeviatingOpeningDayToRestaurantCommand, bool>
        {
            public Fixture(Role? minimumRole) : base(minimumRole)
            {
                RestaurantRepositoryMock = new RestaurantRepositoryMock(MockBehavior.Strict);
            }

            public RestaurantRepositoryMock RestaurantRepositoryMock { get; }

            public Restaurant Restaurant { get; private set; }

            public DeviatingOpeningDay DeviatingOpeningDay { get; private set; }

            public override AddDeviatingOpeningDayToRestaurantCommandHandler CreateTestObject()
            {
                return new AddDeviatingOpeningDayToRestaurantCommandHandler(
                    RestaurantRepositoryMock.Object
                );
            }

            public override AddDeviatingOpeningDayToRestaurantCommand CreateSuccessfulCommand()
            {
                return new AddDeviatingOpeningDayToRestaurantCommand(Restaurant.Id, DeviatingOpeningDay.Date,
                    DeviatingOpeningDay.Status);
            }

            public void SetupRandomRestaurant()
            {
                Restaurant = new RestaurantBuilder()
                    .WithoutDeviatingOpeningDays()
                    .WithValidConstrains()
                    .Create();
            }

            public void SetupRandomDeviatingOpeningDay()
            {
                DeviatingOpeningDay = new DeviatingOpeningDayBuilder()
                    .WithoutOpeningPeriods()
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

            public override void SetupForSuccessfulCommandExecution()
            {
                SetupRandomRestaurant();
                SetupRandomDeviatingOpeningDay();
                SetupRestaurantRepositoryFindingRestaurant();
                SetupRestaurantRepositoryStoringRestaurant();
            }
        }
    }
}
