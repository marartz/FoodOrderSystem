using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Gastromio.Core.Application.Commands.RemoveDishCategoryFromRestaurant;
using Gastromio.Core.Domain.Model.DishCategories;
using Gastromio.Core.Domain.Model.Restaurants;
using Gastromio.Core.Domain.Model.Users;
using Gastromio.Domain.TestKit.Application.Ports.Persistence;
using Gastromio.Domain.TestKit.Domain.Model.DishCategories;
using Gastromio.Domain.TestKit.Domain.Model.Dishes;
using Gastromio.Domain.TestKit.Domain.Model.Restaurants;
using Moq;
using Xunit;

namespace Gastromio.Domain.Tests.Application.Commands.RemoveDishCategoryFromRestaurant
{
    public class RemoveDishCategoryFromRestaurantCommandHandlerTests : CommandHandlerTestBase<
        RemoveDishCategoryFromRestaurantCommandHandler, RemoveDishCategoryFromRestaurantCommand, bool>
    {
        private readonly Fixture fixture;

        public RemoveDishCategoryFromRestaurantCommandHandlerTests()
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
        public async Task HandleAsync_AllValid_RemovesDishCategoryFromRestaurantAndReturnsSuccess()
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
                fixture.DishRepositoryMock.VerifyRemoveByDishCategoryIdAsync(fixture.DishCategory.Id, Times.Once);
                fixture.DishCategoryRepositoryMock.VerifyRemoveAsync(fixture.DishCategory.Id, Times.Once);
            }
        }

        protected override
            CommandHandlerTestFixtureBase<RemoveDishCategoryFromRestaurantCommandHandler, RemoveDishCategoryFromRestaurantCommand,
                bool> FixtureBase
        {
            get { return fixture; }
        }

        private sealed class Fixture : CommandHandlerTestFixtureBase<RemoveDishCategoryFromRestaurantCommandHandler,
            RemoveDishCategoryFromRestaurantCommand, bool>
        {
            public Fixture(Role? minimumRole) : base(minimumRole)
            {
                RestaurantRepositoryMock = new RestaurantRepositoryMock(MockBehavior.Strict);
                DishCategoryRepositoryMock = new DishCategoryRepositoryMock(MockBehavior.Strict);
                DishRepositoryMock = new DishRepositoryMock(MockBehavior.Strict);
            }

            public RestaurantRepositoryMock RestaurantRepositoryMock { get; }

            public DishCategoryRepositoryMock DishCategoryRepositoryMock { get; }

            public DishRepositoryMock DishRepositoryMock { get; }

            public Restaurant Restaurant { get; private set; }

            public DishCategory DishCategory { get; private set; }

            public override RemoveDishCategoryFromRestaurantCommandHandler CreateTestObject()
            {
                return new RemoveDishCategoryFromRestaurantCommandHandler(
                    RestaurantRepositoryMock.Object,
                    DishCategoryRepositoryMock.Object,
                    DishRepositoryMock.Object
                );
            }

            public override RemoveDishCategoryFromRestaurantCommand CreateSuccessfulCommand()
            {
                return new RemoveDishCategoryFromRestaurantCommand(Restaurant.Id, DishCategory.Id);
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
                    .WithOrderNo(0)
                    .WithCreatedBy(UserId)
                    .WithCreatedOn(DateTimeOffset.Now)
                    .WithUpdatedBy(UserId)
                    .WithUpdatedOn(DateTimeOffset.Now)
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

            public void SetupDishCategoryRepositoryFindingDishCategoriesForRestaurant()
            {
                DishCategoryRepositoryMock.SetupFindByRestaurantIdAsync(Restaurant.Id)
                    .ReturnsAsync(new[] {DishCategory});
            }

            public void SetupDishRepositoryRemovingDishesOfDishCategory()
            {
                DishRepositoryMock.SetupRemoveByDishCategoryIdAsync(DishCategory.Id)
                    .Returns(Task.CompletedTask);
            }

            public void SetupDishCategoryRepositoryRemovingDishCategory()
            {
                DishCategoryRepositoryMock.SetupRemoveAsync(DishCategory.Id)
                    .Returns(Task.CompletedTask);
            }

            public override void SetupForSuccessfulCommandExecution(Role? role)
            {
                SetupRandomRestaurant(role);
                SetupRandomDishCategory();
                SetupRestaurantRepositoryFindingRestaurant();
                SetupDishCategoryRepositoryFindingDishCategoriesForRestaurant();
                SetupDishRepositoryRemovingDishesOfDishCategory();
                SetupDishCategoryRepositoryRemovingDishCategory();
            }
        }
    }
}
