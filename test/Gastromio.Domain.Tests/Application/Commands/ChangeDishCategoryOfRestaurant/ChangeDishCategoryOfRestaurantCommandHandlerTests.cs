using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Gastromio.Core.Application.Commands.ChangeDishCategoryOfRestaurant;
using Gastromio.Core.Domain.Model.DishCategories;
using Gastromio.Core.Domain.Model.Restaurants;
using Gastromio.Core.Domain.Model.Users;
using Gastromio.Domain.TestKit.Application.Ports.Persistence;
using Gastromio.Domain.TestKit.Domain.Model.DishCategories;
using Gastromio.Domain.TestKit.Domain.Model.Restaurants;
using Moq;
using Xunit;

namespace Gastromio.Domain.Tests.Application.Commands.ChangeDishCategoryOfRestaurant
{
    public class ChangeDishCategoryOfRestaurantCommandHandlerTests : CommandHandlerTestBase<
        ChangeDishCategoryOfRestaurantCommandHandler, ChangeDishCategoryOfRestaurantCommand, bool>
    {
        private readonly Fixture fixture;

        public ChangeDishCategoryOfRestaurantCommandHandlerTests()
        {
            fixture = new Fixture(Role.RestaurantAdmin);
        }

        [Fact]
        public async Task HandleAsync_RestaurantNotKnown_ReturnsFailure()
        {
            // Arrange
            fixture.SetupRandomRestaurant(fixture.MinimumRole);
            fixture.SetupRandomDishCategory();
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
        public async Task HandleAsync_DishCategoryNotKnown_ReturnsFailure()
        {
            // Arrange
            fixture.SetupRandomRestaurant(fixture.MinimumRole);
            fixture.SetupRandomDishCategory();
            fixture.SetupRestaurantRepositoryFindingRestaurant();
            fixture.SetupDishCategoryRepositoryNotFindingDishCategoryForRestaurant();

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
        public async Task HandleAsync_AllValid_AddsDishCategoryToRestaurantAndReturnsSuccess()
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
                fixture.DishCategory.Name.Should().Be("changed");
                fixture.DishCategoryRepositoryMock.VerifyStoreAsync(fixture.DishCategory, Times.Once);
            }
        }

        protected override CommandHandlerTestFixtureBase<ChangeDishCategoryOfRestaurantCommandHandler,
            ChangeDishCategoryOfRestaurantCommand, bool> FixtureBase
        {
            get { return fixture; }
        }

        private sealed class Fixture : CommandHandlerTestFixtureBase<ChangeDishCategoryOfRestaurantCommandHandler,
            ChangeDishCategoryOfRestaurantCommand, bool>
        {
            public Fixture(Role? minimumRole) : base(minimumRole)
            {
                RestaurantRepositoryMock = new RestaurantRepositoryMock(MockBehavior.Strict);
                DishCategoryRepositoryMock = new DishCategoryRepositoryMock(MockBehavior.Strict);
            }

            public RestaurantRepositoryMock RestaurantRepositoryMock { get; }

            public DishCategoryRepositoryMock DishCategoryRepositoryMock { get; }

            public Restaurant Restaurant { get; private set; }

            public DishCategory DishCategory { get; private set; }

            public override ChangeDishCategoryOfRestaurantCommandHandler CreateTestObject()
            {
                return new ChangeDishCategoryOfRestaurantCommandHandler(
                    RestaurantRepositoryMock.Object,
                    DishCategoryRepositoryMock.Object
                );
            }

            public override ChangeDishCategoryOfRestaurantCommand CreateSuccessfulCommand()
            {
                return new ChangeDishCategoryOfRestaurantCommand(Restaurant.Id, DishCategory.Id, "changed");
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

            public void SetupRandomDishCategory()
            {
                DishCategory = new DishCategoryBuilder()
                    .WithRestaurantId(Restaurant.Id)
                    .WithName("test")
                    .WithOrderNo(0)
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
                    .ReturnsAsync((Restaurant) null);
            }

            public void SetupDishCategoryRepositoryFindingDishCategoryForRestaurant()
            {
                DishCategoryRepositoryMock.SetupFindByRestaurantIdAsync(Restaurant.Id)
                    .ReturnsAsync(new[] {DishCategory});
            }

            public void SetupDishCategoryRepositoryNotFindingDishCategoryForRestaurant()
            {
                DishCategoryRepositoryMock.SetupFindByRestaurantIdAsync(Restaurant.Id)
                    .ReturnsAsync(Enumerable.Empty<DishCategory>());
            }

            public void SetupDishCategoryRepositoryStoringDishCategory()
            {
                DishCategoryRepositoryMock.SetupStoreAsync(DishCategory)
                    .Returns(Task.CompletedTask);
            }

            public override void SetupForSuccessfulCommandExecution(Role? role)
            {
                SetupRandomRestaurant(role);
                SetupRandomDishCategory();
                SetupRestaurantRepositoryFindingRestaurant();
                SetupDishCategoryRepositoryFindingDishCategoryForRestaurant();
                SetupDishCategoryRepositoryStoringDishCategory();
            }
        }
    }
}
