using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Gastromio.Core.Application.Commands.DecOrderOfDishCategory;
using Gastromio.Core.Domain.Model.DishCategories;
using Gastromio.Core.Domain.Model.Restaurants;
using Gastromio.Core.Domain.Model.Users;
using Gastromio.Domain.TestKit.Application.Ports.Persistence;
using Gastromio.Domain.TestKit.Domain.Model.DishCategories;
using Moq;
using Xunit;

namespace Gastromio.Domain.Tests.Application.Commands.DecOrderOfDishCategory
{
    public class DecOrderOfDishCategoryCommandHandlerTests : CommandHandlerTestBase<
        DecOrderOfDishCategoryCommandHandler, DecOrderOfDishCategoryCommand, bool>
    {
        private readonly Fixture fixture;

        public DecOrderOfDishCategoryCommandHandlerTests()
        {
            fixture = new Fixture(Role.RestaurantAdmin);
        }

        [Fact]
        public async Task HandleAsync_DishCategoryNotKnown_ReturnsFailure()
        {
            // Arrange
            fixture.SetupDishCategories(3);
            fixture.SetupCurrentDishCategory(1);
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
        public async Task HandleAsync_ThreeDishCategories_CurrentHasIndex0_ChangesNothingAndReturnsSuccess()
        {
            // Arrange
            fixture.SetupDishCategories(3);
            fixture.SetupCurrentDishCategory(0);
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
                fixture.DishCategories[0].OrderNo.Should().Be(0);
                fixture.DishCategories[1].OrderNo.Should().Be(1);
                fixture.DishCategories[2].OrderNo.Should().Be(2);
                fixture.DishCategoryRepositoryMock.VerifyStoreAsync(fixture.DishCategories[0], Times.Never);
                fixture.DishCategoryRepositoryMock.VerifyStoreAsync(fixture.DishCategories[1], Times.Never);
                fixture.DishCategoryRepositoryMock.VerifyStoreAsync(fixture.DishCategories[2], Times.Never);
            }
        }

        [Fact]
        public async Task HandleAsync_ThreeDishCategories_CurrentHasIndex1_ChangesDishOrderAndReturnsSuccess()
        {
            // Arrange
            fixture.SetupDishCategories(3);
            fixture.SetupCurrentDishCategory(1);
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
                fixture.DishCategories[0].OrderNo.Should().Be(1);
                fixture.DishCategories[1].OrderNo.Should().Be(0);
                fixture.DishCategories[2].OrderNo.Should().Be(2);
                fixture.DishCategoryRepositoryMock.VerifyStoreAsync(fixture.DishCategories[0], Times.Once);
                fixture.DishCategoryRepositoryMock.VerifyStoreAsync(fixture.DishCategories[1], Times.Once);
                fixture.DishCategoryRepositoryMock.VerifyStoreAsync(fixture.DishCategories[2], Times.Never);
            }
        }

        [Fact]
        public async Task HandleAsync_ThreeDishCategories_CurrentHasIndex2_ChangesDishOrderAndReturnsSuccess()
        {
            // Arrange
            fixture.SetupDishCategories(3);
            fixture.SetupCurrentDishCategory(2);
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
                fixture.DishCategories[0].OrderNo.Should().Be(0);
                fixture.DishCategories[1].OrderNo.Should().Be(2);
                fixture.DishCategories[2].OrderNo.Should().Be(1);
                fixture.DishCategoryRepositoryMock.VerifyStoreAsync(fixture.DishCategories[0], Times.Never);
                fixture.DishCategoryRepositoryMock.VerifyStoreAsync(fixture.DishCategories[1], Times.Once);
                fixture.DishCategoryRepositoryMock.VerifyStoreAsync(fixture.DishCategories[2], Times.Once);
            }
        }

        [Fact]
        public async Task HandleAsync_OneDishCategory_ChangesNothingAndReturnsSuccess()
        {
            // Arrange
            fixture.SetupDishCategories(1);
            fixture.SetupCurrentDishCategory(0);
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
                fixture.DishCategories[0].OrderNo.Should().Be(0);
                fixture.DishCategoryRepositoryMock.VerifyStoreAsync(fixture.DishCategories[0], Times.Never);
            }
        }

        protected override
            CommandHandlerTestFixtureBase<DecOrderOfDishCategoryCommandHandler, DecOrderOfDishCategoryCommand, bool> FixtureBase
        {
            get { return fixture; }
        }

        private sealed class Fixture : CommandHandlerTestFixtureBase<DecOrderOfDishCategoryCommandHandler,
            DecOrderOfDishCategoryCommand, bool>
        {
            public Fixture(Role? minimumRole) : base(minimumRole)
            {
                DishCategoryRepositoryMock = new DishCategoryRepositoryMock(MockBehavior.Strict);
            }

            public DishCategoryRepositoryMock DishCategoryRepositoryMock { get; }

            public List<DishCategory> DishCategories { get; private set; }

            public DishCategory CurrentDishCategory { get; private set; }

            public override DecOrderOfDishCategoryCommandHandler CreateTestObject()
            {
                return new DecOrderOfDishCategoryCommandHandler(
                    DishCategoryRepositoryMock.Object
                );
            }

            public override DecOrderOfDishCategoryCommand CreateSuccessfulCommand()
            {
                return new DecOrderOfDishCategoryCommand(CurrentDishCategory.Id);
            }

            public void SetupDishCategories(int count)
            {
                var restaurantId = new RestaurantId(Guid.NewGuid());

                DishCategories = new List<DishCategory>();

                for (var i = 0; i < count; i++)
                {
                    DishCategories.Add(new DishCategoryBuilder()
                        .WithRestaurantId(restaurantId)
                        .WithOrderNo(i)
                        .WithValidConstrains()
                        .Create());
                }
            }

            public void SetupCurrentDishCategory(int index)
            {
                CurrentDishCategory = DishCategories[index];
            }

            public void SetupDishRepositoryFindingDishByDishId()
            {
                DishCategoryRepositoryMock.SetupFindByDishCategoryIdAsync(CurrentDishCategory.Id)
                    .ReturnsAsync(CurrentDishCategory);
            }

            public void SetupDishRepositoryNotFindingDishByDishId()
            {
                DishCategoryRepositoryMock.SetupFindByDishCategoryIdAsync(CurrentDishCategory.Id)
                    .ReturnsAsync((DishCategory) null);
            }

            public void SetupDishRepositoryFindingDishByDishCategoryId()
            {
                DishCategoryRepositoryMock.SetupFindByRestaurantIdAsync(CurrentDishCategory.RestaurantId)
                    .ReturnsAsync(DishCategories);
            }

            public void SetupDishRepositoryStoringDish(int index)
            {
                DishCategoryRepositoryMock.SetupStoreAsync(DishCategories[index])
                    .Returns(Task.CompletedTask);
            }

            public override void SetupForSuccessfulCommandExecution(Role? role)
            {
                SetupDishCategories(3);
                SetupCurrentDishCategory(1);
                SetupDishRepositoryFindingDishByDishId();
                SetupDishRepositoryFindingDishByDishCategoryId();
                SetupDishRepositoryStoringDish(0);
                SetupDishRepositoryStoringDish(1);
            }
        }
    }
}
