using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Gastromio.Core.Application.Commands.AddDishCategoryToRestaurant;
using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.DishCategories;
using Gastromio.Core.Domain.Model.Restaurants;
using Gastromio.Core.Domain.Model.Users;
using Gastromio.Domain.TestKit.Application.Ports.Persistence;
using Gastromio.Domain.TestKit.Domain.Model.DishCategories;
using Gastromio.Domain.TestKit.Domain.Model.Restaurants;
using Moq;
using Xunit;

namespace Gastromio.Domain.Tests.Application.Commands.AddDishCategoryToRestaurant
{
    public class AddDishCategoryToRestaurantCommandHandlerTests : CommandHandlerTestBase<
        AddDishCategoryToRestaurantCommandHandler,
        AddDishCategoryToRestaurantCommand, Guid>
    {
        private readonly Fixture fixture;

        public AddDishCategoryToRestaurantCommandHandlerTests()
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
                fixture.DishCategoryRepositoryMock.VerifyStoreAsync(fixture.DishCategory, Times.Once);
            }
        }

        protected override
            CommandHandlerTestFixtureBase<AddDishCategoryToRestaurantCommandHandler, AddDishCategoryToRestaurantCommand,
                Guid> FixtureBase
        {
            get { return fixture; }
        }

        private sealed class Fixture : CommandHandlerTestFixtureBase<AddDishCategoryToRestaurantCommandHandler,
            AddDishCategoryToRestaurantCommand, Guid>
        {
            public Fixture(Role? minimumRole) : base(minimumRole)
            {
                RestaurantRepositoryMock = new RestaurantRepositoryMock(MockBehavior.Strict);
                DishCategoryRepositoryMock = new DishCategoryRepositoryMock(MockBehavior.Strict);
                DishCategoryFactoryMock = new DishCategoryFactoryMock(MockBehavior.Strict);
            }

            public RestaurantRepositoryMock RestaurantRepositoryMock { get; }

            public DishCategoryRepositoryMock DishCategoryRepositoryMock { get; }

            public DishCategoryFactoryMock DishCategoryFactoryMock { get; }

            public Restaurant Restaurant { get; private set; }

            public DishCategory DishCategory { get; private set; }

            public override AddDishCategoryToRestaurantCommandHandler CreateTestObject()
            {
                return new AddDishCategoryToRestaurantCommandHandler(
                    RestaurantRepositoryMock.Object,
                    DishCategoryRepositoryMock.Object,
                    DishCategoryFactoryMock.Object
                );
            }

            public override AddDishCategoryToRestaurantCommand CreateSuccessfulCommand()
            {
                return new AddDishCategoryToRestaurantCommand(Restaurant.Id, DishCategory.Name, null);
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

            public void SetupDishCategoryRepositoryFindingNoDishCategoriesForRestaurant()
            {
                DishCategoryRepositoryMock.SetupFindByRestaurantIdAsync(Restaurant.Id)
                    .ReturnsAsync(Enumerable.Empty<DishCategory>());
            }

            public void SetupDishCategoryRepositoryStoringDishCategory()
            {
                DishCategoryRepositoryMock.SetupStoreAsync(DishCategory)
                    .Returns(Task.CompletedTask);
            }

            public void SetupDishCategoryFactoryCreatingDishCategory()
            {
                DishCategoryFactoryMock
                    .SetupCreate(
                        Restaurant.Id,
                        DishCategory.Name,
                        DishCategory.OrderNo,
                        DishCategory.CreatedBy
                    )
                    .Returns(SuccessResult<DishCategory>.Create(DishCategory));
            }

            public override void SetupForSuccessfulCommandExecution(Role? role)
            {
                SetupRandomRestaurant(role);
                SetupRandomDishCategory();
                SetupRestaurantRepositoryFindingRestaurant();
                SetupDishCategoryRepositoryFindingNoDishCategoriesForRestaurant();
                SetupDishCategoryFactoryCreatingDishCategory();
                SetupDishCategoryRepositoryStoringDishCategory();
            }
        }
    }
}
