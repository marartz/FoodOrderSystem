using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Gastromio.Core.Application.Commands.RemoveRegularOpeningPeriodFromRestaurant;
using Gastromio.Core.Domain.Model.Restaurants;
using Gastromio.Core.Domain.Model.Users;
using Gastromio.Domain.TestKit.Application.Ports.Persistence;
using Gastromio.Domain.TestKit.Domain.Model.Restaurants;
using Moq;
using Xunit;

namespace Gastromio.Domain.Tests.Application.Commands.RemoveRegularOpeningPeriodFromRestaurant
{
    public class RemoveRegularOpeningPeriodFromRestaurantCommandHandlerTests : CommandHandlerTestBase<RemoveRegularOpeningPeriodFromRestaurantCommandHandler,
        RemoveRegularOpeningPeriodFromRestaurantCommand, bool>
    {
        private readonly Fixture fixture;

        public RemoveRegularOpeningPeriodFromRestaurantCommandHandlerTests()
        {
            fixture = new Fixture(Role.RestaurantAdmin);
        }

        [Fact]
        public async Task HandleAsync_RestaurantNotKnown_ReturnsFailure()
        {
            // Arrange
            fixture.SetupRandomDateOfRegularOpeningDay();
            fixture.SetupRandomOpeningPeriod();
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
        public async Task HandleAsync_AllValid_RemovesRegularOpeningPeriodAndReturnsSuccess()
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
                fixture.Restaurant.RegularOpeningDays.Should().BeEmpty();
                fixture.RestaurantRepositoryMock.VerifyStoreAsync(fixture.Restaurant, Times.Once);
            }
        }

        protected override
            CommandHandlerTestFixtureBase<RemoveRegularOpeningPeriodFromRestaurantCommandHandler, RemoveRegularOpeningPeriodFromRestaurantCommand, bool> FixtureBase
        {
            get { return fixture; }
        }

        private sealed class Fixture : CommandHandlerTestFixtureBase<RemoveRegularOpeningPeriodFromRestaurantCommandHandler,
            RemoveRegularOpeningPeriodFromRestaurantCommand, bool>
        {
            public Fixture(Role? minimumRole) : base(minimumRole)
            {
                RestaurantRepositoryMock = new RestaurantRepositoryMock(MockBehavior.Strict);
            }

            public RestaurantRepositoryMock RestaurantRepositoryMock { get; }

            public int RegularOpeningDay { get; private set; }

            public OpeningPeriod OpeningPeriod { get; private set; }

            public Restaurant Restaurant { get; private set; }

            public override RemoveRegularOpeningPeriodFromRestaurantCommandHandler CreateTestObject()
            {
                return new RemoveRegularOpeningPeriodFromRestaurantCommandHandler(
                    RestaurantRepositoryMock.Object
                );
            }

            public override RemoveRegularOpeningPeriodFromRestaurantCommand CreateSuccessfulCommand()
            {
                return new RemoveRegularOpeningPeriodFromRestaurantCommand(
                    Restaurant.Id,
                    RegularOpeningDay,
                    OpeningPeriod.Start
                );
            }

            public void SetupRandomDateOfRegularOpeningDay()
            {
                RegularOpeningDay = 0;
            }

            public void SetupRandomRestaurant(Role? role)
            {
                var builder = new RestaurantBuilder()
                    .WithRegularOpeningDays(new[]
                    {
                        new RegularOpeningDayBuilder()
                            .WithDayOfWeek(RegularOpeningDay)
                            .WithOpeningPeriods(new[] {OpeningPeriod})
                            .Create()
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

            public void SetupRandomOpeningPeriod()
            {
                OpeningPeriod = new OpeningPeriodBuilder()
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
                SetupRandomDateOfRegularOpeningDay();
                SetupRandomOpeningPeriod();
                SetupRandomRestaurant(role);
                SetupRestaurantRepositoryFindingRestaurant();
                SetupRestaurantRepositoryStoringRestaurant();
            }
        }
    }
}
