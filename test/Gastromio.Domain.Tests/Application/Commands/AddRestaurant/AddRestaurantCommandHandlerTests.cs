using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Gastromio.Core.Application.Commands.AddRestaurant;
using Gastromio.Core.Application.DTOs;
using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.Restaurants;
using Gastromio.Core.Domain.Model.Users;
using Gastromio.Domain.TestKit.Application.Ports.Persistence;
using Gastromio.Domain.TestKit.Domain.Model.Cuisines;
using Gastromio.Domain.TestKit.Domain.Model.PaymentMethods;
using Gastromio.Domain.TestKit.Domain.Model.RestaurantImages;
using Gastromio.Domain.TestKit.Domain.Model.Restaurants;
using Gastromio.Domain.TestKit.Domain.Model.Users;
using Moq;
using Xunit;

namespace Gastromio.Domain.Tests.Application.Commands.AddRestaurant
{
    public class AddRestaurantCommandHandlerTests : CommandHandlerTestBase<AddRestaurantCommandHandler,
        AddRestaurantCommand, RestaurantDTO>
    {
        private readonly Fixture fixture;

        public AddRestaurantCommandHandlerTests()
        {
            fixture = new Fixture(Role.SystemAdmin);
        }

        [Fact]
        public async Task HandleAsync_RestaurantCreationWithFailure_ReturnsFailure()
        {
            // Arrange
            fixture.SetupRandomRestaurantToCreate();
            fixture.SetupRestaurantRepositoryNotFindingRestaurantByName();
            fixture.SetupCuisineRepositoryFindingCuisines();
            fixture.SetupPaymentMethodRepositoryFindingPaymentMethods();
            fixture.SetupRestaurantFactoryNotCreatingRestaurant();

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
        public async Task HandleAsync_RestaurantAlreadyKnown_ReturnsFailure()
        {
            // Arrange
            fixture.SetupRandomRestaurantToCreate();
            fixture.SetupRestaurantRepositoryFindingRestaurantByName();

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
        public async Task HandleAsync_AllValid_CreatesRestaurantAndReturnsSuccess()
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
                fixture.RestaurantRepositoryMock.VerifyStoreAsync(fixture.Restaurant, Times.Once);
            }
        }

        protected override
            CommandHandlerTestFixtureBase<AddRestaurantCommandHandler, AddRestaurantCommand, RestaurantDTO> FixtureBase
        {
            get { return fixture; }
        }

        private sealed class Fixture : CommandHandlerTestFixtureBase<AddRestaurantCommandHandler,
            AddRestaurantCommand, RestaurantDTO>
        {
            public Fixture(Role? minimumRole) : base(minimumRole)
            {
                RestaurantFactoryMock = new RestaurantFactoryMock(MockBehavior.Strict);
                RestaurantRepositoryMock = new RestaurantRepositoryMock(MockBehavior.Strict);
                RestaurantImageRepositoryMock = new RestaurantImageRepositoryMock(MockBehavior.Strict);
                CuisineRepositoryMock = new CuisineRepositoryMock(MockBehavior.Strict);
                PaymentMethodRepositoryMock = new PaymentMethodRepositoryMock(MockBehavior.Strict);
                UserRepositoryMock = new UserRepositoryMock(MockBehavior.Strict);
            }

            public Restaurant Restaurant { get; private set; }

            public RestaurantFactoryMock RestaurantFactoryMock { get; }

            public RestaurantRepositoryMock RestaurantRepositoryMock { get; }

            public RestaurantImageRepositoryMock RestaurantImageRepositoryMock { get; }

            public CuisineRepositoryMock CuisineRepositoryMock { get; }

            public PaymentMethodRepositoryMock PaymentMethodRepositoryMock { get; }

            public UserRepositoryMock UserRepositoryMock { get; }

            public override AddRestaurantCommandHandler CreateTestObject()
            {
                return new AddRestaurantCommandHandler(
                    RestaurantFactoryMock.Object,
                    RestaurantRepositoryMock.Object,
                    RestaurantImageRepositoryMock.Object,
                    CuisineRepositoryMock.Object,
                    PaymentMethodRepositoryMock.Object,
                    UserRepositoryMock.Object
                );
            }

            public override AddRestaurantCommand CreateSuccessfulCommand()
            {
                return new AddRestaurantCommand("test");
            }

            public void SetupRandomRestaurantToCreate()
            {
                Restaurant = new RestaurantBuilder()
                    .WithName("test")
                    .Create();
            }

            public void SetupRestaurantRepositoryFindingRestaurantByName()
            {
                var restaurant = new RestaurantBuilder()
                    .WithName("test")
                    .Create();

                RestaurantRepositoryMock.SetupFindByRestaurantNameAsync("test")
                    .ReturnsAsync(new[] {restaurant});
            }

            public void SetupRestaurantRepositoryNotFindingRestaurantByName()
            {
                RestaurantRepositoryMock.SetupFindByRestaurantNameAsync("test")
                    .ReturnsAsync(Enumerable.Empty<Restaurant>());
            }

            public void SetupCuisineRepositoryFindingCuisines()
            {
                var cuisines = Restaurant.Cuisines.Select(
                    cuisineId => new CuisineBuilder().WithId(cuisineId).Create()
                );
                CuisineRepositoryMock.SetupFindAllAsync()
                    .ReturnsAsync(cuisines);
            }

            public void SetupPaymentMethodRepositoryFindingPaymentMethods()
            {
                var paymentMethods = Restaurant.PaymentMethods.Select(
                    paymentMethodId => new PaymentMethodBuilder().WithId(paymentMethodId).Create()
                );
                PaymentMethodRepositoryMock.SetupFindAllAsync()
                    .ReturnsAsync(paymentMethods);
            }

            public void SetupRestaurantRepositoryStoringRestaurant()
            {
                RestaurantRepositoryMock.Setup(m => m.StoreAsync(It.IsAny<Restaurant>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);
            }

            public void SetupRestaurantFactoryCreatingRestaurant()
            {
                RestaurantFactoryMock.SetupCreateWithName("test", UserWithMinimumRole.Id)
                    .Returns(SuccessResult<Restaurant>.Create(Restaurant));
            }

            public void SetupRestaurantFactoryNotCreatingRestaurant()
            {
                RestaurantFactoryMock.SetupCreateWithName("test", UserWithMinimumRole.Id)
                    .Returns(FailureResult<Restaurant>.Create(FailureResultCode.RestaurantNameRequired));
            }

            public void SetupUserRepositoryFindingAdministrators()
            {
                var administrators = Restaurant.Administrators.Select(
                    userId => new UserBuilder().WithId(userId).Create()
                );

                UserRepositoryMock.SetupFindByUserIdsAsync(Restaurant.Administrators)
                    .ReturnsAsync(administrators);
            }

            public void SetupRestaurantImageRepositoryFindingTypes()
            {
                RestaurantImageRepositoryMock.SetupFindTypesByRestaurantIdsAsync(new[] {Restaurant.Id})
                    .ReturnsAsync(new Dictionary<RestaurantId, IEnumerable<string>>());
            }

            public override void SetupForSuccessfulCommandExecution(Role? role)
            {
                SetupRandomRestaurantToCreate();
                SetupRestaurantRepositoryNotFindingRestaurantByName();
                SetupCuisineRepositoryFindingCuisines();
                SetupPaymentMethodRepositoryFindingPaymentMethods();
                SetupRestaurantFactoryCreatingRestaurant();
                SetupRestaurantRepositoryStoringRestaurant();
                SetupUserRepositoryFindingAdministrators();
                SetupRestaurantImageRepositoryFindingTypes();
            }
        }
    }
}
