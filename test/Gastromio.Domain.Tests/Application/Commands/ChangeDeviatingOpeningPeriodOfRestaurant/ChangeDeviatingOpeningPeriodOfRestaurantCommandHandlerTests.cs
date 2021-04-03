using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Gastromio.Core.Application.Commands.ChangeDeviatingOpeningPeriodOfRestaurant;
using Gastromio.Core.Domain.Model.Restaurants;
using Gastromio.Core.Domain.Model.Users;
using Gastromio.Domain.TestKit.Application.Ports.Persistence;
using Gastromio.Domain.TestKit.Domain.Model.Restaurants;
using Moq;
using Xunit;

namespace Gastromio.Domain.Tests.Application.Commands.ChangeDeviatingOpeningPeriodOfRestaurant
{
    public class ChangeDeviatingOpeningPeriodOfRestaurantCommandHandlerTests : CommandHandlerTestBase<ChangeDeviatingOpeningPeriodOfRestaurantCommandHandler,
        ChangeDeviatingOpeningPeriodOfRestaurantCommand, bool>
    {
        private readonly Fixture fixture;

        public ChangeDeviatingOpeningPeriodOfRestaurantCommandHandlerTests()
        {
            fixture = new Fixture(Role.RestaurantAdmin);
        }

        [Fact]
        public async Task HandleAsync_RestaurantNotKnown_ReturnsFailure()
        {
            // Arrange
            fixture.SetupRandomDeviatingOpeningPeriodAndDay();
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
        public async Task HandleAsync_AllValid_ChangesDeviatingOpeningPeriodAndReturnsSuccess()
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
                var openingPeriod = fixture.Restaurant.DeviatingOpeningDays.Select(en => en.Value).First();
                openingPeriod.OpeningPeriods.Should()
                    .BeEquivalentTo(new OpeningPeriod(TimeSpan.FromHours(15), TimeSpan.FromHours(21)));
                fixture.RestaurantRepositoryMock.VerifyStoreAsync(fixture.Restaurant, Times.Once);
            }
        }

        protected override
            CommandHandlerTestFixtureBase<ChangeDeviatingOpeningPeriodOfRestaurantCommandHandler, ChangeDeviatingOpeningPeriodOfRestaurantCommand, bool> FixtureBase
        {
            get { return fixture; }
        }

        private sealed class Fixture : CommandHandlerTestFixtureBase<ChangeDeviatingOpeningPeriodOfRestaurantCommandHandler,
            ChangeDeviatingOpeningPeriodOfRestaurantCommand, bool>
        {
            public Fixture(Role? minimumRole) : base(minimumRole)
            {
                RestaurantRepositoryMock = new RestaurantRepositoryMock(MockBehavior.Strict);
            }

            public RestaurantRepositoryMock RestaurantRepositoryMock { get; }

            public OpeningPeriod DeviatingOpeningPeriod { get; private set; }

            public DeviatingOpeningDay DeviatingOpeningDay { get; private set; }

            public Restaurant Restaurant { get; private set; }

            public override ChangeDeviatingOpeningPeriodOfRestaurantCommandHandler CreateTestObject()
            {
                return new ChangeDeviatingOpeningPeriodOfRestaurantCommandHandler(
                    RestaurantRepositoryMock.Object
                );
            }

            public override ChangeDeviatingOpeningPeriodOfRestaurantCommand CreateSuccessfulCommand()
            {
                return new ChangeDeviatingOpeningPeriodOfRestaurantCommand(Restaurant.Id, DeviatingOpeningDay.Date,
                    DeviatingOpeningPeriod.Start, TimeSpan.FromHours(15), TimeSpan.FromHours(21));
            }

            public void SetupRandomDeviatingOpeningPeriodAndDay()
            {
                DeviatingOpeningPeriod = new OpeningPeriod(TimeSpan.FromHours(16), TimeSpan.FromHours(22));

                DeviatingOpeningDay = new DeviatingOpeningDayBuilder()
                    .WithStatus(DeviatingOpeningDayStatus.Closed)
                    .WithOpeningPeriods(new[] {DeviatingOpeningPeriod})
                    .Create();
            }

            public void SetupRandomRestaurant(Role? role)
            {
                var builder = new RestaurantBuilder()
                    .WithDeviatingOpeningDays(new[] {DeviatingOpeningDay});

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
                SetupRandomDeviatingOpeningPeriodAndDay();
                SetupRandomRestaurant(role);
                SetupRestaurantRepositoryFindingRestaurant();
                SetupRestaurantRepositoryStoringRestaurant();
            }
        }
    }
}
