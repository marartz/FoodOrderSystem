using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Gastromio.Core.Application.Commands.ChangeRestaurantServiceInfo;
using Gastromio.Core.Domain.Model.Restaurants;
using Gastromio.Core.Domain.Model.Users;
using Gastromio.Domain.TestKit.Application.Ports.Persistence;
using Gastromio.Domain.TestKit.Common;
using Gastromio.Domain.TestKit.Domain.Model.Restaurants;
using Moq;
using Xunit;

namespace Gastromio.Domain.Tests.Application.Commands.ChangeRestaurantServiceInfo
{
    public class ChangeRestaurantServiceInfoCommandHandlerTests : CommandHandlerTestBase<ChangeRestaurantServiceInfoCommandHandler,
        ChangeRestaurantServiceInfoCommand, bool>
    {
        private readonly Fixture fixture;

        public ChangeRestaurantServiceInfoCommandHandlerTests()
        {
            fixture = new Fixture(Role.RestaurantAdmin);
        }

        [Fact]
        public async Task HandleAsync_RestaurantNotKnown_ReturnsFailure()
        {
            // Arrange
            fixture.SetupRandomRestaurant(fixture.MinimumRole);
            fixture.SetupRandomServiceInfo();
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
        public async Task HandleAsync_AllValid_ChangesServiceInfoOfRestaurantAndReturnsSuccess()
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
                fixture.Restaurant.PickupInfo.Should().BeEquivalentTo(fixture.PickupInfo);
                fixture.Restaurant.DeliveryInfo.Should().BeEquivalentTo(fixture.DeliveryInfo);
                fixture.Restaurant.ReservationInfo.Should().BeEquivalentTo(fixture.ReservationInfo);
                fixture.Restaurant.HygienicHandling.Should().BeEquivalentTo(fixture.HygienicHandling);
                fixture.RestaurantRepositoryMock.VerifyStoreAsync(fixture.Restaurant, Times.Once);
            }
        }

        protected override
            CommandHandlerTestFixtureBase<ChangeRestaurantServiceInfoCommandHandler, ChangeRestaurantServiceInfoCommand, bool> FixtureBase
        {
            get { return fixture; }
        }

        private sealed class Fixture : CommandHandlerTestFixtureBase<ChangeRestaurantServiceInfoCommandHandler,
            ChangeRestaurantServiceInfoCommand, bool>
        {
            public Fixture(Role? minimumRole) : base(minimumRole)
            {
                RestaurantRepositoryMock = new RestaurantRepositoryMock(MockBehavior.Strict);
            }

            public RestaurantRepositoryMock RestaurantRepositoryMock { get; }

            public Restaurant Restaurant { get; private set; }

            public PickupInfo PickupInfo { get; private set; }

            public DeliveryInfo DeliveryInfo { get; private set; }

            public ReservationInfo ReservationInfo { get; private set; }

            public string HygienicHandling { get; private set; }

            public override ChangeRestaurantServiceInfoCommandHandler CreateTestObject()
            {
                return new ChangeRestaurantServiceInfoCommandHandler(
                    RestaurantRepositoryMock.Object
                );
            }

            public override ChangeRestaurantServiceInfoCommand CreateSuccessfulCommand()
            {
                return new ChangeRestaurantServiceInfoCommand(
                    Restaurant.Id,
                    PickupInfo,
                    DeliveryInfo,
                    ReservationInfo,
                    HygienicHandling
                );
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

            public void SetupRandomServiceInfo()
            {
                PickupInfo = new PickupInfoBuilder()
                    .WithValidConstrains()
                    .Create();

                DeliveryInfo = new DeliveryInfoBuilder()
                    .WithValidConstrains()
                    .Create();

                ReservationInfo = new ReservationInfoBuilder()
                    .Create();

                HygienicHandling = RandomStringBuilder.BuildWithLength(50);
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
                SetupRandomRestaurant(role);
                SetupRandomServiceInfo();
                SetupRestaurantRepositoryFindingRestaurant();
                SetupRestaurantRepositoryStoringRestaurant();
            }
        }
    }
}
