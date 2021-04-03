using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Gastromio.Core.Application.Commands.ChangeRegularOpeningPeriodOfRestaurant;
using Gastromio.Core.Domain.Model.Restaurants;
using Gastromio.Core.Domain.Model.Users;
using Gastromio.Domain.TestKit.Application.Ports.Persistence;
using Gastromio.Domain.TestKit.Domain.Model.Restaurants;
using Moq;
using Xunit;

namespace Gastromio.Domain.Tests.Application.Commands.ChangeRegularOpeningPeriodOfRestaurant
{
    public class ChangeRegularOpeningPeriodOfRestaurantCommandHandlerTests : CommandHandlerTestBase<ChangeRegularOpeningPeriodOfRestaurantCommandHandler,
        ChangeRegularOpeningPeriodOfRestaurantCommand, bool>
    {
        private readonly Fixture fixture;

        public ChangeRegularOpeningPeriodOfRestaurantCommandHandlerTests()
        {
            fixture = new Fixture(Role.RestaurantAdmin);
        }

        [Fact]
        public async Task HandleAsync_RestaurantNotKnown_ReturnsFailure()
        {
            // Arrange
            fixture.SetupRandomRegularOpeningPeriodAndDay();
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
        public async Task HandleAsync_AllValid_ChangesRegularOpeningPeriodAndReturnsSuccess()
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
                var openingPeriod = fixture.Restaurant.RegularOpeningDays.Select(en => en.Value).First();
                openingPeriod.OpeningPeriods.Should()
                    .BeEquivalentTo(new OpeningPeriod(TimeSpan.FromHours(15), TimeSpan.FromHours(21)));
                fixture.RestaurantRepositoryMock.VerifyStoreAsync(fixture.Restaurant, Times.Once);
            }
        }

        protected override
            CommandHandlerTestFixtureBase<ChangeRegularOpeningPeriodOfRestaurantCommandHandler, ChangeRegularOpeningPeriodOfRestaurantCommand, bool> FixtureBase
        {
            get { return fixture; }
        }

        private sealed class Fixture : CommandHandlerTestFixtureBase<ChangeRegularOpeningPeriodOfRestaurantCommandHandler,
            ChangeRegularOpeningPeriodOfRestaurantCommand, bool>
        {
            public Fixture(Role? minimumRole) : base(minimumRole)
            {
                RestaurantRepositoryMock = new RestaurantRepositoryMock(MockBehavior.Strict);
            }

            public RestaurantRepositoryMock RestaurantRepositoryMock { get; }

            public OpeningPeriod RegularOpeningPeriod { get; private set; }

            public RegularOpeningDay RegularOpeningDay { get; private set; }

            public Restaurant Restaurant { get; private set; }

            public override ChangeRegularOpeningPeriodOfRestaurantCommandHandler CreateTestObject()
            {
                return new ChangeRegularOpeningPeriodOfRestaurantCommandHandler(
                    RestaurantRepositoryMock.Object
                );
            }

            public override ChangeRegularOpeningPeriodOfRestaurantCommand CreateSuccessfulCommand()
            {
                return new ChangeRegularOpeningPeriodOfRestaurantCommand(Restaurant.Id, RegularOpeningDay.DayOfWeek,
                    RegularOpeningPeriod.Start, TimeSpan.FromHours(15), TimeSpan.FromHours(21));
            }

            public void SetupRandomRegularOpeningPeriodAndDay()
            {
                RegularOpeningPeriod = new OpeningPeriod(TimeSpan.FromHours(16), TimeSpan.FromHours(22));

                RegularOpeningDay = new RegularOpeningDayBuilder()
                    .WithDayOfWeek(0)
                    .WithOpeningPeriods(new[] {RegularOpeningPeriod})
                    .Create();
            }

            public void SetupRandomRestaurant(Role? role)
            {
                var builder = new RestaurantBuilder()
                    .WithRegularOpeningDays(new[] {RegularOpeningDay});

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
                SetupRandomRegularOpeningPeriodAndDay();
                SetupRandomRestaurant(role);
                SetupRestaurantRepositoryFindingRestaurant();
                SetupRestaurantRepositoryStoringRestaurant();
            }
        }
    }
}
