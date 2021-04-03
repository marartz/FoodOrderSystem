using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Gastromio.Core.Application.Commands.IncOrderOfDish;
using Gastromio.Core.Domain.Model.DishCategories;
using Gastromio.Core.Domain.Model.Dishes;
using Gastromio.Core.Domain.Model.Restaurants;
using Gastromio.Core.Domain.Model.Users;
using Gastromio.Domain.TestKit.Application.Ports.Persistence;
using Gastromio.Domain.TestKit.Domain.Model.Dishes;
using Moq;
using Xunit;

namespace Gastromio.Domain.Tests.Application.Commands.IncOrderOfDish
{
    public class IncOrderOfDishCommandHandlerTests : CommandHandlerTestBase<
        IncOrderOfDishCommandHandler, IncOrderOfDishCommand, bool>
    {
        private readonly Fixture fixture;

        public IncOrderOfDishCommandHandlerTests()
        {
            fixture = new Fixture(Role.RestaurantAdmin);
        }

        [Fact]
        public async Task HandleAsync_DishNotKnown_ReturnsFailure()
        {
            // Arrange
            fixture.SetupDishes(3);
            fixture.SetupCurrentDish(1);
            fixture.SetupDishRepositoryNotFindingDishByDishId();

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
        public async Task HandleAsync_ThreeDishes_CurrentHasIndex0_ChangesNothingAndReturnsSuccess()
        {
            // Arrange
            fixture.SetupDishes(3);
            fixture.SetupCurrentDish(0);
            fixture.SetupDishRepositoryFindingDishByDishId();
            fixture.SetupDishRepositoryFindingDishByDishCategoryId();
            fixture.SetupDishRepositoryStoringDish(0);
            fixture.SetupDishRepositoryStoringDish(1);

            var testObject = fixture.CreateTestObject();
            var command = fixture.CreateSuccessfulCommand();

            // Act
            var result = await testObject.HandleAsync(command, fixture.UserWithMinimumRole, CancellationToken.None);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                fixture.Dishes[0].OrderNo.Should().Be(1);
                fixture.Dishes[1].OrderNo.Should().Be(0);
                fixture.Dishes[2].OrderNo.Should().Be(2);
                fixture.DishRepositoryMock.VerifyStoreAsync(fixture.Dishes[0], Times.Once);
                fixture.DishRepositoryMock.VerifyStoreAsync(fixture.Dishes[1], Times.Once);
                fixture.DishRepositoryMock.VerifyStoreAsync(fixture.Dishes[2], Times.Never);
            }
        }

        [Fact]
        public async Task HandleAsync_ThreeDishes_CurrentHasIndex1_ChangesDishOrderAndReturnsSuccess()
        {
            // Arrange
            fixture.SetupDishes(3);
            fixture.SetupCurrentDish(1);
            fixture.SetupDishRepositoryFindingDishByDishId();
            fixture.SetupDishRepositoryFindingDishByDishCategoryId();
            fixture.SetupDishRepositoryStoringDish(1);
            fixture.SetupDishRepositoryStoringDish(2);

            var testObject = fixture.CreateTestObject();
            var command = fixture.CreateSuccessfulCommand();

            // Act
            var result = await testObject.HandleAsync(command, fixture.UserWithMinimumRole, CancellationToken.None);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                fixture.Dishes[0].OrderNo.Should().Be(0);
                fixture.Dishes[1].OrderNo.Should().Be(2);
                fixture.Dishes[2].OrderNo.Should().Be(1);
                fixture.DishRepositoryMock.VerifyStoreAsync(fixture.Dishes[0], Times.Never);
                fixture.DishRepositoryMock.VerifyStoreAsync(fixture.Dishes[1], Times.Once);
                fixture.DishRepositoryMock.VerifyStoreAsync(fixture.Dishes[2], Times.Once);
            }
        }

        [Fact]
        public async Task HandleAsync_ThreeDishes_CurrentHasIndex2_ChangesDishOrderAndReturnsSuccess()
        {
            // Arrange
            fixture.SetupDishes(3);
            fixture.SetupCurrentDish(2);
            fixture.SetupDishRepositoryFindingDishByDishId();
            fixture.SetupDishRepositoryFindingDishByDishCategoryId();

            var testObject = fixture.CreateTestObject();
            var command = fixture.CreateSuccessfulCommand();

            // Act
            var result = await testObject.HandleAsync(command, fixture.UserWithMinimumRole, CancellationToken.None);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                fixture.Dishes[0].OrderNo.Should().Be(0);
                fixture.Dishes[1].OrderNo.Should().Be(1);
                fixture.Dishes[2].OrderNo.Should().Be(2);
                fixture.DishRepositoryMock.VerifyStoreAsync(fixture.Dishes[0], Times.Never);
                fixture.DishRepositoryMock.VerifyStoreAsync(fixture.Dishes[1], Times.Never);
                fixture.DishRepositoryMock.VerifyStoreAsync(fixture.Dishes[2], Times.Never);
            }
        }

        [Fact]
        public async Task HandleAsync_OneDish_ChangesNothingAndReturnsSuccess()
        {
            // Arrange
            fixture.SetupDishes(1);
            fixture.SetupCurrentDish(0);
            fixture.SetupDishRepositoryFindingDishByDishId();
            fixture.SetupDishRepositoryFindingDishByDishCategoryId();

            var testObject = fixture.CreateTestObject();
            var command = fixture.CreateSuccessfulCommand();

            // Act
            var result = await testObject.HandleAsync(command, fixture.UserWithMinimumRole, CancellationToken.None);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                fixture.Dishes[0].OrderNo.Should().Be(0);
                fixture.DishRepositoryMock.VerifyStoreAsync(fixture.Dishes[0], Times.Never);
            }
        }

        protected override
            CommandHandlerTestFixtureBase<IncOrderOfDishCommandHandler, IncOrderOfDishCommand, bool> FixtureBase
        {
            get { return fixture; }
        }

        private sealed class Fixture : CommandHandlerTestFixtureBase<IncOrderOfDishCommandHandler,
            IncOrderOfDishCommand, bool>
        {
            public Fixture(Role? minimumRole) : base(minimumRole)
            {
                DishRepositoryMock = new DishRepositoryMock(MockBehavior.Strict);
            }

            public DishRepositoryMock DishRepositoryMock { get; }

            public List<Dish> Dishes { get; private set; }

            public Dish CurrentDish { get; private set; }

            public override IncOrderOfDishCommandHandler CreateTestObject()
            {
                return new IncOrderOfDishCommandHandler(
                    DishRepositoryMock.Object
                );
            }

            public override IncOrderOfDishCommand CreateSuccessfulCommand()
            {
                return new IncOrderOfDishCommand(CurrentDish.Id);
            }

            public void SetupDishes(int count)
            {
                var restaurantId = new RestaurantId(Guid.NewGuid());
                var dishCategoryId = new DishCategoryId(Guid.NewGuid());

                Dishes = new List<Dish>();

                for (var i = 0; i < count; i++)
                {
                    Dishes.Add(new DishBuilder()
                        .WithRestaurantId(restaurantId)
                        .WithCategoryId(dishCategoryId)
                        .WithOrderNo(i)
                        .WithValidConstrains()
                        .Create());
                }
            }

            public void SetupCurrentDish(int index)
            {
                CurrentDish = Dishes[index];
            }

            public void SetupDishRepositoryFindingDishByDishId()
            {
                DishRepositoryMock.SetupFindByDishIdAsync(CurrentDish.Id)
                    .ReturnsAsync(CurrentDish);
            }

            public void SetupDishRepositoryNotFindingDishByDishId()
            {
                DishRepositoryMock.SetupFindByDishIdAsync(CurrentDish.Id)
                    .ReturnsAsync((Dish) null);
            }

            public void SetupDishRepositoryFindingDishByDishCategoryId()
            {
                DishRepositoryMock.SetupFindByDishCategoryIdAsync(CurrentDish.CategoryId)
                    .ReturnsAsync(Dishes);
            }

            public void SetupDishRepositoryStoringDish(int index)
            {
                DishRepositoryMock.SetupStoreAsync(Dishes[index])
                    .Returns(Task.CompletedTask);
            }

            public override void SetupForSuccessfulCommandExecution(Role? role)
            {
                SetupDishes(3);
                SetupCurrentDish(1);
                SetupDishRepositoryFindingDishByDishId();
                SetupDishRepositoryFindingDishByDishCategoryId();
                SetupDishRepositoryStoringDish(1);
                SetupDishRepositoryStoringDish(2);
            }
        }
    }
}
