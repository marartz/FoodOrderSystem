using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Gastromio.Core.Application.Commands.RemoveDeviatingOpeningDayFromRestaurant;
using Gastromio.Core.Domain.Model.Restaurants;
using Gastromio.Core.Domain.Model.Users;
using Gastromio.Domain.TestKit.Application.Ports.Persistence;
using Gastromio.Domain.TestKit.Domain.Model.Restaurants;
using Moq;
using Xunit;

namespace Gastromio.Domain.Tests.Application.Commands.RemoveDeviatingOpeningDayFromRestaurant
{
    public class RemoveDeviatingOpeningDayFromRestaurantCommandHandlerTests : CommandHandlerTestBase<RemoveDeviatingOpeningDayFromRestaurantCommandHandler,
        RemoveDeviatingOpeningDayFromRestaurantCommand, bool>
    {
        private readonly Fixture fixture;

        public RemoveDeviatingOpeningDayFromRestaurantCommandHandlerTests()
        {
            fixture = new Fixture(Role.RestaurantAdmin);
        }

        [Fact]
        public async Task HandleAsync_RestaurantNotKnown_ReturnsFailure()
        {
            // Arrange
            fixture.SetupRandomDeviatingOpeningDay();
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
        public async Task HandleAsync_AllValid_RemovesDeviatingDaysAndReturnsSuccess()
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
                fixture.Restaurant.DeviatingOpeningDays.Should().BeEmpty();
                fixture.RestaurantRepositoryMock.VerifyStoreAsync(fixture.Restaurant, Times.Once);
            }
        }

        protected override
            CommandHandlerTestFixtureBase<RemoveDeviatingOpeningDayFromRestaurantCommandHandler, RemoveDeviatingOpeningDayFromRestaurantCommand, bool> FixtureBase
        {
            get { return fixture; }
        }

        private sealed class Fixture : CommandHandlerTestFixtureBase<RemoveDeviatingOpeningDayFromRestaurantCommandHandler,
            RemoveDeviatingOpeningDayFromRestaurantCommand, bool>
        {
            public Fixture(Role? minimumRole) : base(minimumRole)
            {
                RestaurantRepositoryMock = new RestaurantRepositoryMock(MockBehavior.Strict);
            }

            public RestaurantRepositoryMock RestaurantRepositoryMock { get; }

            public DeviatingOpeningDay DeviatingOpeningDay { get; private set; }

            public Restaurant Restaurant { get; private set; }

            public override RemoveDeviatingOpeningDayFromRestaurantCommandHandler CreateTestObject()
            {
                return new RemoveDeviatingOpeningDayFromRestaurantCommandHandler(
                    RestaurantRepositoryMock.Object
                );
            }

            public override RemoveDeviatingOpeningDayFromRestaurantCommand CreateSuccessfulCommand()
            {
                return new RemoveDeviatingOpeningDayFromRestaurantCommand(Restaurant.Id, DeviatingOpeningDay.Date);
            }

            public void SetupRandomDeviatingOpeningDay()
            {
                DeviatingOpeningDay = new DeviatingOpeningDayBuilder()
                    .WithoutOpeningPeriods()
                    .Create();
            }

            public void SetupRandomRestaurant(Role? role)
            {
                var builder = new RestaurantBuilder()
                    .WithDeviatingOpeningDays(new[]
                    {
                        DeviatingOpeningDay
                    });

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
                SetupRandomDeviatingOpeningDay();
                SetupRandomRestaurant(role);
                SetupRestaurantRepositoryFindingRestaurant();
                SetupRestaurantRepositoryStoringRestaurant();
            }
        }
    }
}
