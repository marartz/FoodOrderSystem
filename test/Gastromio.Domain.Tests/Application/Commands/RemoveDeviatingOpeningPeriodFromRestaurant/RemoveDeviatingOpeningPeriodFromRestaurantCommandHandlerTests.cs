using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Gastromio.Core.Application.Commands.RemoveDeviatingOpeningPeriodFromRestaurant;
using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.Restaurants;
using Gastromio.Core.Domain.Model.Users;
using Gastromio.Domain.TestKit.Application.Ports.Persistence;
using Gastromio.Domain.TestKit.Common;
using Gastromio.Domain.TestKit.Domain.Model.Restaurants;
using Moq;
using Xunit;

namespace Gastromio.Domain.Tests.Application.Commands.RemoveDeviatingOpeningPeriodFromRestaurant
{
    public class RemoveDeviatingOpeningPeriodFromRestaurantCommandHandlerTests : CommandHandlerTestBase<RemoveDeviatingOpeningPeriodFromRestaurantCommandHandler,
        RemoveDeviatingOpeningPeriodFromRestaurantCommand, bool>
    {
        private readonly Fixture fixture;

        public RemoveDeviatingOpeningPeriodFromRestaurantCommandHandlerTests()
        {
            fixture = new Fixture(Role.RestaurantAdmin);
        }

        [Fact]
        public async Task HandleAsync_RestaurantNotKnown_ReturnsFailure()
        {
            // Arrange
            fixture.SetupRandomDateOfDeviatingOpeningDay();
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
        public async Task HandleAsync_AllValid_RemovesDeviatingOpeningPeriodAndReturnsSuccess()
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
                fixture.Restaurant.DeviatingOpeningDays.First().Value.Status.Should()
                    .Be(DeviatingOpeningDayStatus.Closed);
                fixture.Restaurant.DeviatingOpeningDays.First().Value.OpeningPeriods.Should().BeEmpty();
                fixture.RestaurantRepositoryMock.VerifyStoreAsync(fixture.Restaurant, Times.Once);
            }
        }

        protected override
            CommandHandlerTestFixtureBase<RemoveDeviatingOpeningPeriodFromRestaurantCommandHandler, RemoveDeviatingOpeningPeriodFromRestaurantCommand, bool> FixtureBase
        {
            get { return fixture; }
        }

        private sealed class Fixture : CommandHandlerTestFixtureBase<RemoveDeviatingOpeningPeriodFromRestaurantCommandHandler,
            RemoveDeviatingOpeningPeriodFromRestaurantCommand, bool>
        {
            public Fixture(Role? minimumRole) : base(minimumRole)
            {
                RestaurantRepositoryMock = new RestaurantRepositoryMock(MockBehavior.Strict);
            }

            public RestaurantRepositoryMock RestaurantRepositoryMock { get; }

            public Date DateOfDeviatingOpeningDay { get; private set; }

            public OpeningPeriod OpeningPeriod { get; private set; }

            public Restaurant Restaurant { get; private set; }

            public override RemoveDeviatingOpeningPeriodFromRestaurantCommandHandler CreateTestObject()
            {
                return new RemoveDeviatingOpeningPeriodFromRestaurantCommandHandler(
                    RestaurantRepositoryMock.Object
                );
            }

            public override RemoveDeviatingOpeningPeriodFromRestaurantCommand CreateSuccessfulCommand()
            {
                return new RemoveDeviatingOpeningPeriodFromRestaurantCommand(
                    Restaurant.Id,
                    DateOfDeviatingOpeningDay,
                    OpeningPeriod.Start
                );
            }

            public void SetupRandomDateOfDeviatingOpeningDay()
            {
                DateOfDeviatingOpeningDay = new DateBuilder().Create();
            }

            public void SetupRandomOpeningPeriod()
            {
                OpeningPeriod = new OpeningPeriodBuilder()
                    .WithValidConstrains()
                    .Create();
            }

            public void SetupRandomRestaurant(Role? role)
            {
                var builder = new RestaurantBuilder()
                    .WithDeviatingOpeningDays(new[]
                    {
                        new DeviatingOpeningDayBuilder()
                            .WithDate(DateOfDeviatingOpeningDay)
                            .WithStatus(DeviatingOpeningDayStatus.Open)
                            .WithOpeningPeriods(new []
                            {
                                OpeningPeriod
                            })
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
                SetupRandomDateOfDeviatingOpeningDay();
                SetupRandomOpeningPeriod();
                SetupRandomRestaurant(role);
                SetupRestaurantRepositoryFindingRestaurant();
                SetupRestaurantRepositoryStoringRestaurant();
            }
        }
    }
}
