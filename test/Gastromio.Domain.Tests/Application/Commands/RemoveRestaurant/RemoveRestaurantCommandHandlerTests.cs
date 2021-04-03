using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Gastromio.Core.Application.Commands.RemoveRestaurant;
using Gastromio.Core.Domain.Model.Restaurants;
using Gastromio.Core.Domain.Model.Users;
using Gastromio.Domain.TestKit.Application.Ports.Persistence;
using Gastromio.Domain.TestKit.Domain.Model.DishCategories;
using Gastromio.Domain.TestKit.Domain.Model.Dishes;
using Gastromio.Domain.TestKit.Domain.Model.RestaurantImages;
using Gastromio.Domain.TestKit.Domain.Model.Restaurants;
using Moq;
using Xunit;

namespace Gastromio.Domain.Tests.Application.Commands.RemoveRestaurant
{
    public class RemoveRestaurantCommandHandlerTests : CommandHandlerTestBase<RemoveRestaurantCommandHandler,
        RemoveRestaurantCommand, bool>
    {
        private readonly Fixture fixture;

        public RemoveRestaurantCommandHandlerTests()
        {
            fixture = new Fixture(Role.SystemAdmin);
        }

        [Fact]
        public async Task HandleAsync_AllValid_RemovesRestaurantAndDependingAggregatesAndReturnsSuccess()
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
                fixture.DishRepositoryMock.VerifyRemoveByRestaurantIdAsync(fixture.Restaurant.Id, Times.Once);
                fixture.DishCategoryRepositoryMock.VerifyRemoveByRestaurantIdAsync(fixture.Restaurant.Id, Times.Once);
                fixture.RestaurantImageRepositoryMock.VerifyRemoveByRestaurantIdAsync(fixture.Restaurant.Id, Times.Once);
                fixture.RestaurantRepositoryMock.VerifyRemoveAsync(fixture.Restaurant.Id, Times.Once);
            }
        }

        protected override
            CommandHandlerTestFixtureBase<RemoveRestaurantCommandHandler, RemoveRestaurantCommand, bool> FixtureBase
        {
            get { return fixture; }
        }

        private sealed class Fixture : CommandHandlerTestFixtureBase<RemoveRestaurantCommandHandler,
            RemoveRestaurantCommand, bool>
        {
            public Fixture(Role? minimumRole) : base(minimumRole)
            {
                RestaurantRepositoryMock = new RestaurantRepositoryMock(MockBehavior.Strict);
                RestaurantImageRepositoryMock = new RestaurantImageRepositoryMock(MockBehavior.Strict);
                DishCategoryRepositoryMock = new DishCategoryRepositoryMock(MockBehavior.Strict);
                DishRepositoryMock = new DishRepositoryMock(MockBehavior.Strict);
            }

            public Restaurant Restaurant { get; private set; }

            public RestaurantRepositoryMock RestaurantRepositoryMock { get; }

            public RestaurantImageRepositoryMock RestaurantImageRepositoryMock { get; }

            public DishCategoryRepositoryMock DishCategoryRepositoryMock { get; }

            public DishRepositoryMock DishRepositoryMock { get; }

            public override RemoveRestaurantCommandHandler CreateTestObject()
            {
                return new RemoveRestaurantCommandHandler(
                    RestaurantRepositoryMock.Object,
                    RestaurantImageRepositoryMock.Object,
                    DishCategoryRepositoryMock.Object,
                    DishRepositoryMock.Object
                );
            }

            public override RemoveRestaurantCommand CreateSuccessfulCommand()
            {
                return new RemoveRestaurantCommand(Restaurant.Id);
            }

            public void SetupRandomRestaurant()
            {
                Restaurant = new RestaurantBuilder()
                    .WithName("test")
                    .Create();
            }

            public void SetupDishRepositoryRemovingDishesOfRestaurant()
            {
                DishRepositoryMock.SetupRemoveByRestaurantIdAsync(Restaurant.Id)
                    .Returns(Task.CompletedTask);
            }

            public void SetupDishCategoryRepositoryRemovingDishCategoriesOfRestaurant()
            {
                DishCategoryRepositoryMock.SetupRemoveByRestaurantIdAsync(Restaurant.Id)
                    .Returns(Task.CompletedTask);
            }

            public void SetupRestaurantImageRepositoryRemovingImagesOfRestaurant()
            {
                RestaurantImageRepositoryMock.SetupRemoveByRestaurantIdAsync(Restaurant.Id)
                    .Returns(Task.CompletedTask);
            }

            public void SetupRestaurantRepositoryRemovingRestaurant()
            {
                RestaurantRepositoryMock.SetupRemoveAsync(Restaurant.Id)
                    .Returns(Task.CompletedTask);
            }

            public override void SetupForSuccessfulCommandExecution(Role? role)
            {
                SetupRandomRestaurant();
                SetupDishRepositoryRemovingDishesOfRestaurant();
                SetupDishCategoryRepositoryRemovingDishCategoriesOfRestaurant();
                SetupRestaurantImageRepositoryRemovingImagesOfRestaurant();
                SetupRestaurantRepositoryRemovingRestaurant();
            }
        }
    }
}
