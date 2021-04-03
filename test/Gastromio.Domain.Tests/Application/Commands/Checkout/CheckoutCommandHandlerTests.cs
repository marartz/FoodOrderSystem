using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Gastromio.Core.Application.Commands.Checkout;
using Gastromio.Core.Application.DTOs;
using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.DishCategories;
using Gastromio.Core.Domain.Model.Dishes;
using Gastromio.Core.Domain.Model.Orders;
using Gastromio.Core.Domain.Model.PaymentMethods;
using Gastromio.Core.Domain.Model.Restaurants;
using Gastromio.Core.Domain.Model.Users;
using Gastromio.Domain.TestKit.Application.Ports.Persistence;
using Gastromio.Domain.TestKit.Common;
using Gastromio.Domain.TestKit.Domain.Model.DishCategories;
using Gastromio.Domain.TestKit.Domain.Model.Dishes;
using Gastromio.Domain.TestKit.Domain.Model.PaymentMethods;
using Gastromio.Domain.TestKit.Domain.Model.Restaurants;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace Gastromio.Domain.Tests.Application.Commands.Checkout
{
    public class CheckoutCommandHandlerTests : CommandHandlerTestBase<CheckoutCommandHandler, CheckoutCommand, OrderDTO>
    {
        private readonly Fixture fixture;

        public CheckoutCommandHandlerTests(ITestOutputHelper output)
        {
            fixture = new Fixture(output, null);
        }

        [Fact]
        public async Task HandleAsync_RestaurantNotFoundById_ReturnsFailure()
        {
            // Arrange
            fixture.SetupActiveRestaurantWithDeliveryActivated();
            fixture.SetupRandomEnabledDishCategories();
            fixture.SetupRandomDishes();
            fixture.SetupPaymentMethods();
            fixture.SetupValidServiceTime();
            fixture.SetupDefaultOrderedDishes();
            fixture.SetupSuccessfulCommandWithRestaurantId(OrderType.Delivery);
            fixture.SetupRestaurantRepositoryNotFindingRestaurantById();

            var testObject = fixture.CreateTestObject();
            var command = fixture.CreateSuccessfulCommand();

            // Act
            var result = await testObject.HandleAsync(command, fixture.UserWithMinimumRole, CancellationToken.None);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
                fixture.RestaurantRepositoryMock.VerifyFindByRestaurantIdAsync(fixture.Restaurant.Id, Times.Once);
            }
        }

        [Fact]
        public async Task HandleAsync_RestaurantFoundById_ReturnsSuccess()
        {
            // Arrange
            fixture.SetupActiveRestaurantWithDeliveryActivated();
            fixture.SetupRandomEnabledDishCategories();
            fixture.SetupRandomDishes();
            fixture.SetupPaymentMethods();
            fixture.SetupValidServiceTime();
            fixture.SetupDefaultOrderedDishes();
            fixture.SetupSuccessfulCommandWithRestaurantId(OrderType.Delivery);
            fixture.SetupRestaurantRepositoryFindingRestaurantById();
            fixture.SetupDishCategoryRepositoryFindingDishCategoriesById();
            fixture.SetupDishRepositoryFindingDishesById();
            fixture.SetupPaymentMethodRepositoryFindingPaymentMethods();
            fixture.SetupOrderRepositoryStoringOrder();

            var testObject = fixture.CreateTestObject();
            var command = fixture.CreateSuccessfulCommand();

            // Act
            var result = await testObject.HandleAsync(command, fixture.UserWithMinimumRole, CancellationToken.None);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                fixture.RestaurantRepositoryMock.VerifyFindByRestaurantIdAsync(fixture.Restaurant.Id, Times.Once);
            }
        }

        [Fact]
        public async Task HandleAsync_RestaurantNotFoundByName_ReturnsFailure()
        {
            // Arrange
            fixture.SetupActiveRestaurantWithDeliveryActivated();
            fixture.SetupRandomEnabledDishCategories();
            fixture.SetupRandomDishes();
            fixture.SetupPaymentMethods();
            fixture.SetupValidServiceTime();
            fixture.SetupDefaultOrderedDishes();
            fixture.SetupSuccessfulCommandWithRestaurantName(OrderType.Delivery);
            fixture.SetupRestaurantRepositoryNotFindingRestaurantByName();

            var testObject = fixture.CreateTestObject();
            var command = fixture.CreateSuccessfulCommand();

            // Act
            var result = await testObject.HandleAsync(command, fixture.UserWithMinimumRole, CancellationToken.None);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
                fixture.RestaurantRepositoryMock.VerifyFindByRestaurantNameAsync(fixture.Restaurant.Name, Times.Once);
            }
        }

        [Fact]
        public async Task HandleAsync_RestaurantFoundByName_ReturnsSuccess()
        {
            // Arrange
            fixture.SetupActiveRestaurantWithDeliveryActivated();
            fixture.SetupRandomEnabledDishCategories();
            fixture.SetupRandomDishes();
            fixture.SetupPaymentMethods();
            fixture.SetupValidServiceTime();
            fixture.SetupDefaultOrderedDishes();
            fixture.SetupSuccessfulCommandWithRestaurantName(OrderType.Delivery);
            fixture.SetupRestaurantRepositoryFindingRestaurantByName();
            fixture.SetupDishCategoryRepositoryFindingDishCategoriesById();
            fixture.SetupDishRepositoryFindingDishesById();
            fixture.SetupPaymentMethodRepositoryFindingPaymentMethods();
            fixture.SetupOrderRepositoryStoringOrder();

            var testObject = fixture.CreateTestObject();
            var command = fixture.CreateSuccessfulCommand();

            // Act
            var result = await testObject.HandleAsync(command, fixture.UserWithMinimumRole, CancellationToken.None);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                fixture.RestaurantRepositoryMock.VerifyFindByRestaurantNameAsync(fixture.Restaurant.Name, Times.Once);
            }
        }

        [Fact]
        public async Task HandleAsync_RestaurantInactive_ReturnsFailure()
        {
            // Arrange
            fixture.SetupInactiveRestaurant();
            fixture.SetupRandomEnabledDishCategories();
            fixture.SetupRandomDishes();
            fixture.SetupPaymentMethods();
            fixture.SetupValidServiceTime();
            fixture.SetupDefaultOrderedDishes();
            fixture.SetupSuccessfulCommandWithRestaurantId(OrderType.Delivery);
            fixture.SetupRestaurantRepositoryFindingRestaurantById();

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
        public async Task HandleAsync_DishThatCannotBeFound_ReturnsFailure()
        {
            // Arrange
            fixture.SetupActiveRestaurantWithDeliveryActivated();
            fixture.SetupRandomEnabledDishCategories();
            fixture.SetupRandomDishes();
            fixture.SetupPaymentMethods();
            fixture.SetupValidServiceTime();
            fixture.SetupOrderedDishThatCannotBeFound();
            fixture.SetupSuccessfulCommandWithRestaurantId(OrderType.Delivery);
            fixture.SetupRestaurantRepositoryFindingRestaurantById();
            fixture.SetupDishCategoryRepositoryFindingDishCategoriesById();
            fixture.SetupDishRepositoryFindingDishesById();
            fixture.SetupPaymentMethodRepositoryFindingPaymentMethods();
            fixture.SetupOrderRepositoryStoringOrder();

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
        public async Task HandleAsync_DishThatDoesNotBelongToRestaurant_ReturnsFailure()
        {
            // Arrange
            fixture.SetupActiveRestaurantWithDeliveryActivated();
            fixture.SetupRandomEnabledDishCategories();
            fixture.SetupRandomDishes();
            fixture.SetupPaymentMethods();
            fixture.SetupValidServiceTime();
            fixture.SetupOrderedDishThatDoesNotBelongToRestaurant();
            fixture.SetupSuccessfulCommandWithRestaurantId(OrderType.Delivery);
            fixture.SetupRestaurantRepositoryFindingRestaurantById();
            fixture.SetupDishCategoryRepositoryFindingDishCategoriesById();
            fixture.SetupDishRepositoryFindingDishesById();
            fixture.SetupPaymentMethodRepositoryFindingPaymentMethods();
            fixture.SetupOrderRepositoryStoringOrder();

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
        public async Task HandleAsync_DishThatBelongsToDisabledCategory_ReturnsFailure()
        {
            // Arrange
            fixture.SetupActiveRestaurantWithDeliveryActivated();
            fixture.SetupRandomDisabledDishCategories();
            fixture.SetupRandomDishes();
            fixture.SetupPaymentMethods();
            fixture.SetupValidServiceTime();
            fixture.SetupDefaultOrderedDishes();
            fixture.SetupSuccessfulCommandWithRestaurantId(OrderType.Delivery);
            fixture.SetupRestaurantRepositoryFindingRestaurantById();
            fixture.SetupDishCategoryRepositoryFindingDishCategoriesById();
            fixture.SetupDishRepositoryFindingDishesById();
            fixture.SetupPaymentMethodRepositoryFindingPaymentMethods();
            fixture.SetupOrderRepositoryStoringOrder();

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
        public async Task HandleAsync_VariantThatDoesNotBelongToDish_ReturnsFailure()
        {
            // Arrange
            fixture.SetupActiveRestaurantWithDeliveryActivated();
            fixture.SetupRandomEnabledDishCategories();
            fixture.SetupRandomDishes();
            fixture.SetupPaymentMethods();
            fixture.SetupValidServiceTime();
            fixture.SetupOrderedDishVariantThatDoesNotBelongToDish();
            fixture.SetupSuccessfulCommandWithRestaurantId(OrderType.Delivery);
            fixture.SetupRestaurantRepositoryFindingRestaurantById();
            fixture.SetupDishCategoryRepositoryFindingDishCategoriesById();
            fixture.SetupDishRepositoryFindingDishesById();
            fixture.SetupPaymentMethodRepositoryFindingPaymentMethods();
            fixture.SetupOrderRepositoryStoringOrder();

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
        public async Task HandleAsync_RemarkHasLength1001_ReturnsFailure()
        {
            // Arrange
            fixture.SetupActiveRestaurantWithDeliveryActivated();
            fixture.SetupRandomEnabledDishCategories();
            fixture.SetupRandomDishes();
            fixture.SetupPaymentMethods();
            fixture.SetupValidServiceTime();
            fixture.SetupOrderedDishesWithRemarkOfLength1001();
            fixture.SetupSuccessfulCommandWithRestaurantId(OrderType.Delivery);
            fixture.SetupRestaurantRepositoryFindingRestaurantById();
            fixture.SetupDishCategoryRepositoryFindingDishCategoriesById();
            fixture.SetupDishRepositoryFindingDishesById();
            fixture.SetupPaymentMethodRepositoryFindingPaymentMethods();
            fixture.SetupOrderRepositoryStoringOrder();

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
        public async Task HandleAsync_CountIsNegative_ReturnsFailure()
        {
            // Arrange
            fixture.SetupActiveRestaurantWithDeliveryActivated();
            fixture.SetupRandomEnabledDishCategories();
            fixture.SetupRandomDishes();
            fixture.SetupPaymentMethods();
            fixture.SetupValidServiceTime();
            fixture.SetupOrderedDishWithNegativeCount();
            fixture.SetupSuccessfulCommandWithRestaurantId(OrderType.Delivery);
            fixture.SetupRestaurantRepositoryFindingRestaurantById();
            fixture.SetupDishCategoryRepositoryFindingDishCategoriesById();
            fixture.SetupDishRepositoryFindingDishesById();
            fixture.SetupPaymentMethodRepositoryFindingPaymentMethods();
            fixture.SetupOrderRepositoryStoringOrder();

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
        public async Task HandleAsync_CountIsZero_ReturnsFailure()
        {
            // Arrange
            fixture.SetupActiveRestaurantWithDeliveryActivated();
            fixture.SetupRandomEnabledDishCategories();
            fixture.SetupRandomDishes();
            fixture.SetupPaymentMethods();
            fixture.SetupValidServiceTime();
            fixture.SetupOrderedDishWithZeroCount();
            fixture.SetupSuccessfulCommandWithRestaurantId(OrderType.Delivery);
            fixture.SetupRestaurantRepositoryFindingRestaurantById();
            fixture.SetupDishCategoryRepositoryFindingDishCategoriesById();
            fixture.SetupDishRepositoryFindingDishesById();
            fixture.SetupPaymentMethodRepositoryFindingPaymentMethods();
            fixture.SetupOrderRepositoryStoringOrder();

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
        public async Task HandleAsync_CountOf101_ReturnsFailure()
        {
            // Arrange
            fixture.SetupActiveRestaurantWithDeliveryActivated();
            fixture.SetupRandomEnabledDishCategories();
            fixture.SetupRandomDishes();
            fixture.SetupPaymentMethods();
            fixture.SetupValidServiceTime();
            fixture.SetupOrderedDishWithCountOf101();
            fixture.SetupSuccessfulCommandWithRestaurantId(OrderType.Delivery);
            fixture.SetupRestaurantRepositoryFindingRestaurantById();
            fixture.SetupDishCategoryRepositoryFindingDishCategoriesById();
            fixture.SetupDishRepositoryFindingDishesById();
            fixture.SetupPaymentMethodRepositoryFindingPaymentMethods();
            fixture.SetupOrderRepositoryStoringOrder();

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
        public async Task HandleAsync_PickupOrdered_PickupIsDisabled_ReturnsFailure()
        {
            // Arrange
            fixture.SetupActiveRestaurantWithDeliveryActivated();
            fixture.SetupRandomEnabledDishCategories();
            fixture.SetupRandomDishes();
            fixture.SetupPaymentMethods();
            fixture.SetupValidServiceTime();
            fixture.SetupDefaultOrderedDishes();
            fixture.SetupSuccessfulCommandWithRestaurantId(OrderType.Pickup);
            fixture.SetupRestaurantRepositoryFindingRestaurantById();
            fixture.SetupDishCategoryRepositoryFindingDishCategoriesById();
            fixture.SetupDishRepositoryFindingDishesById();
            fixture.SetupPaymentMethodRepositoryFindingPaymentMethods();
            fixture.SetupOrderRepositoryStoringOrder();

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
        public async Task HandleAsync_PickupOrdered_BelowMinimum_ReturnsFailure()
        {
            // Arrange
            fixture.SetupActiveRestaurantWithPickupActivated();
            fixture.SetupRandomEnabledDishCategories();
            fixture.SetupRandomDishes();
            fixture.SetupPaymentMethods();
            fixture.SetupValidServiceTime();
            fixture.SetupOrderedDishWithCountOfOne();
            fixture.SetupSuccessfulCommandWithRestaurantId(OrderType.Pickup);
            fixture.SetupRestaurantRepositoryFindingRestaurantById();
            fixture.SetupDishCategoryRepositoryFindingDishCategoriesById();
            fixture.SetupDishRepositoryFindingDishesById();
            fixture.SetupPaymentMethodRepositoryFindingPaymentMethods();
            fixture.SetupOrderRepositoryStoringOrder();

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
        public async Task HandleAsync_PickupOrdered_AboveMaximum_ReturnsFailure()
        {
            // Arrange
            fixture.SetupActiveRestaurantWithPickupActivated();
            fixture.SetupRandomEnabledDishCategories();
            fixture.SetupRandomDishes();
            fixture.SetupPaymentMethods();
            fixture.SetupValidServiceTime();
            fixture.SetupOrderedDishWithCountOf100();
            fixture.SetupSuccessfulCommandWithRestaurantId(OrderType.Pickup);
            fixture.SetupRestaurantRepositoryFindingRestaurantById();
            fixture.SetupDishCategoryRepositoryFindingDishCategoriesById();
            fixture.SetupDishRepositoryFindingDishesById();
            fixture.SetupPaymentMethodRepositoryFindingPaymentMethods();
            fixture.SetupOrderRepositoryStoringOrder();

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
        public async Task HandleAsync_PickupOrdered_GivenNameNull_ReturnsFailure()
        {
            // Arrange
            fixture.SetupActiveRestaurantWithPickupActivated();
            fixture.SetupRandomEnabledDishCategories();
            fixture.SetupRandomDishes();
            fixture.SetupPaymentMethods();
            fixture.SetupValidServiceTime();
            fixture.SetupDefaultOrderedDishes();
            fixture.SetupCommandWithGivenNameNull(OrderType.Pickup);
            fixture.SetupRestaurantRepositoryFindingRestaurantById();
            fixture.SetupDishCategoryRepositoryFindingDishCategoriesById();
            fixture.SetupDishRepositoryFindingDishesById();
            fixture.SetupPaymentMethodRepositoryFindingPaymentMethods();
            fixture.SetupOrderRepositoryStoringOrder();

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
        public async Task HandleAsync_PickupOrdered_GivenNameEmpty_ReturnsFailure()
        {
            // Arrange
            fixture.SetupActiveRestaurantWithPickupActivated();
            fixture.SetupRandomEnabledDishCategories();
            fixture.SetupRandomDishes();
            fixture.SetupPaymentMethods();
            fixture.SetupValidServiceTime();
            fixture.SetupDefaultOrderedDishes();
            fixture.SetupCommandWithGivenNameEmpty(OrderType.Pickup);
            fixture.SetupRestaurantRepositoryFindingRestaurantById();
            fixture.SetupDishCategoryRepositoryFindingDishCategoriesById();
            fixture.SetupDishRepositoryFindingDishesById();
            fixture.SetupPaymentMethodRepositoryFindingPaymentMethods();
            fixture.SetupOrderRepositoryStoringOrder();

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
        public async Task HandleAsync_PickupOrdered_LastNameNull_ReturnsFailure()
        {
            // Arrange
            fixture.SetupActiveRestaurantWithPickupActivated();
            fixture.SetupRandomEnabledDishCategories();
            fixture.SetupRandomDishes();
            fixture.SetupPaymentMethods();
            fixture.SetupValidServiceTime();
            fixture.SetupDefaultOrderedDishes();
            fixture.SetupCommandWithLastNameNull(OrderType.Pickup);
            fixture.SetupRestaurantRepositoryFindingRestaurantById();
            fixture.SetupDishCategoryRepositoryFindingDishCategoriesById();
            fixture.SetupDishRepositoryFindingDishesById();
            fixture.SetupPaymentMethodRepositoryFindingPaymentMethods();
            fixture.SetupOrderRepositoryStoringOrder();

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
        public async Task HandleAsync_PickupOrdered_LastNameEmpty_ReturnsFailure()
        {
            // Arrange
            fixture.SetupActiveRestaurantWithPickupActivated();
            fixture.SetupRandomEnabledDishCategories();
            fixture.SetupRandomDishes();
            fixture.SetupPaymentMethods();
            fixture.SetupValidServiceTime();
            fixture.SetupDefaultOrderedDishes();
            fixture.SetupCommandWithLastNameEmpty(OrderType.Pickup);
            fixture.SetupRestaurantRepositoryFindingRestaurantById();
            fixture.SetupDishCategoryRepositoryFindingDishCategoriesById();
            fixture.SetupDishRepositoryFindingDishesById();
            fixture.SetupPaymentMethodRepositoryFindingPaymentMethods();
            fixture.SetupOrderRepositoryStoringOrder();

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
        public async Task HandleAsync_PickupOrdered_EmailNull_ReturnsFailure()
        {
            // Arrange
            fixture.SetupActiveRestaurantWithPickupActivated();
            fixture.SetupRandomEnabledDishCategories();
            fixture.SetupRandomDishes();
            fixture.SetupPaymentMethods();
            fixture.SetupValidServiceTime();
            fixture.SetupDefaultOrderedDishes();
            fixture.SetupCommandWithEmailNull(OrderType.Pickup);
            fixture.SetupRestaurantRepositoryFindingRestaurantById();
            fixture.SetupDishCategoryRepositoryFindingDishCategoriesById();
            fixture.SetupDishRepositoryFindingDishesById();
            fixture.SetupPaymentMethodRepositoryFindingPaymentMethods();
            fixture.SetupOrderRepositoryStoringOrder();

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
        public async Task HandleAsync_PickupOrdered_PhoneNull_ReturnsFailure()
        {
            // Arrange
            fixture.SetupActiveRestaurantWithPickupActivated();
            fixture.SetupRandomEnabledDishCategories();
            fixture.SetupRandomDishes();
            fixture.SetupPaymentMethods();
            fixture.SetupValidServiceTime();
            fixture.SetupDefaultOrderedDishes();
            fixture.SetupCommandWithPhoneNull(OrderType.Pickup);
            fixture.SetupRestaurantRepositoryFindingRestaurantById();
            fixture.SetupDishCategoryRepositoryFindingDishCategoriesById();
            fixture.SetupDishRepositoryFindingDishesById();
            fixture.SetupPaymentMethodRepositoryFindingPaymentMethods();
            fixture.SetupOrderRepositoryStoringOrder();

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
        public async Task HandleAsync_PickupOrdered_PhoneEmpty_ReturnsFailure()
        {
            // Arrange
            fixture.SetupActiveRestaurantWithPickupActivated();
            fixture.SetupRandomEnabledDishCategories();
            fixture.SetupRandomDishes();
            fixture.SetupPaymentMethods();
            fixture.SetupValidServiceTime();
            fixture.SetupDefaultOrderedDishes();
            fixture.SetupCommandWithPhoneEmpty(OrderType.Pickup);
            fixture.SetupRestaurantRepositoryFindingRestaurantById();
            fixture.SetupDishCategoryRepositoryFindingDishCategoriesById();
            fixture.SetupDishRepositoryFindingDishesById();
            fixture.SetupPaymentMethodRepositoryFindingPaymentMethods();
            fixture.SetupOrderRepositoryStoringOrder();

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
        public async Task HandleAsync_PickupOrdered_EmailEmpty_ReturnsFailure()
        {
            // Arrange
            fixture.SetupActiveRestaurantWithPickupActivated();
            fixture.SetupRandomEnabledDishCategories();
            fixture.SetupRandomDishes();
            fixture.SetupPaymentMethods();
            fixture.SetupValidServiceTime();
            fixture.SetupDefaultOrderedDishes();
            fixture.SetupCommandWithEmailEmpty(OrderType.Pickup);
            fixture.SetupRestaurantRepositoryFindingRestaurantById();
            fixture.SetupDishCategoryRepositoryFindingDishCategoriesById();
            fixture.SetupDishRepositoryFindingDishesById();
            fixture.SetupPaymentMethodRepositoryFindingPaymentMethods();
            fixture.SetupOrderRepositoryStoringOrder();

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
        public async Task HandleAsync_DeliveryOrdered_DeliveryIsDisabled_ReturnsFailure()
        {
            // Arrange
            fixture.SetupActiveRestaurantWithPickupActivated();
            fixture.SetupRandomEnabledDishCategories();
            fixture.SetupRandomDishes();
            fixture.SetupPaymentMethods();
            fixture.SetupValidServiceTime();
            fixture.SetupDefaultOrderedDishes();
            fixture.SetupSuccessfulCommandWithRestaurantId(OrderType.Delivery);
            fixture.SetupRestaurantRepositoryFindingRestaurantById();
            fixture.SetupDishCategoryRepositoryFindingDishCategoriesById();
            fixture.SetupDishRepositoryFindingDishesById();
            fixture.SetupPaymentMethodRepositoryFindingPaymentMethods();
            fixture.SetupOrderRepositoryStoringOrder();

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
        public async Task HandleAsync_DeliveryOrdered_BelowMinimum_ReturnsFailure()
        {
            // Arrange
            fixture.SetupActiveRestaurantWithDeliveryActivated();
            fixture.SetupRandomEnabledDishCategories();
            fixture.SetupRandomDishes();
            fixture.SetupPaymentMethods();
            fixture.SetupValidServiceTime();
            fixture.SetupOrderedDishWithCountOfOne();
            fixture.SetupSuccessfulCommandWithRestaurantId(OrderType.Delivery);
            fixture.SetupRestaurantRepositoryFindingRestaurantById();
            fixture.SetupDishCategoryRepositoryFindingDishCategoriesById();
            fixture.SetupDishRepositoryFindingDishesById();
            fixture.SetupPaymentMethodRepositoryFindingPaymentMethods();
            fixture.SetupOrderRepositoryStoringOrder();

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
        public async Task HandleAsync_DeliveryOrdered_AboveMaximum_ReturnsFailure()
        {
            // Arrange
            fixture.SetupActiveRestaurantWithDeliveryActivated();
            fixture.SetupRandomEnabledDishCategories();
            fixture.SetupRandomDishes();
            fixture.SetupPaymentMethods();
            fixture.SetupValidServiceTime();
            fixture.SetupOrderedDishWithCountOf100();
            fixture.SetupSuccessfulCommandWithRestaurantId(OrderType.Delivery);
            fixture.SetupRestaurantRepositoryFindingRestaurantById();
            fixture.SetupDishCategoryRepositoryFindingDishCategoriesById();
            fixture.SetupDishRepositoryFindingDishesById();
            fixture.SetupPaymentMethodRepositoryFindingPaymentMethods();
            fixture.SetupOrderRepositoryStoringOrder();

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
        public async Task HandleAsync_DeliveryOrdered_GivenNameNull_ReturnsFailure()
        {
            // Arrange
            fixture.SetupActiveRestaurantWithDeliveryActivated();
            fixture.SetupRandomEnabledDishCategories();
            fixture.SetupRandomDishes();
            fixture.SetupPaymentMethods();
            fixture.SetupValidServiceTime();
            fixture.SetupDefaultOrderedDishes();
            fixture.SetupCommandWithGivenNameNull(OrderType.Delivery);
            fixture.SetupRestaurantRepositoryFindingRestaurantById();
            fixture.SetupDishCategoryRepositoryFindingDishCategoriesById();
            fixture.SetupDishRepositoryFindingDishesById();
            fixture.SetupPaymentMethodRepositoryFindingPaymentMethods();
            fixture.SetupOrderRepositoryStoringOrder();

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
        public async Task HandleAsync_DeliveryOrdered_GivenNameEmpty_ReturnsFailure()
        {
            // Arrange
            fixture.SetupActiveRestaurantWithDeliveryActivated();
            fixture.SetupRandomEnabledDishCategories();
            fixture.SetupRandomDishes();
            fixture.SetupPaymentMethods();
            fixture.SetupValidServiceTime();
            fixture.SetupDefaultOrderedDishes();
            fixture.SetupCommandWithGivenNameEmpty(OrderType.Delivery);
            fixture.SetupRestaurantRepositoryFindingRestaurantById();
            fixture.SetupDishCategoryRepositoryFindingDishCategoriesById();
            fixture.SetupDishRepositoryFindingDishesById();
            fixture.SetupPaymentMethodRepositoryFindingPaymentMethods();
            fixture.SetupOrderRepositoryStoringOrder();

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
        public async Task HandleAsync_DeliveryOrdered_LastNameNull_ReturnsFailure()
        {
            // Arrange
            fixture.SetupActiveRestaurantWithDeliveryActivated();
            fixture.SetupRandomEnabledDishCategories();
            fixture.SetupRandomDishes();
            fixture.SetupPaymentMethods();
            fixture.SetupValidServiceTime();
            fixture.SetupDefaultOrderedDishes();
            fixture.SetupCommandWithLastNameNull(OrderType.Delivery);
            fixture.SetupRestaurantRepositoryFindingRestaurantById();
            fixture.SetupDishCategoryRepositoryFindingDishCategoriesById();
            fixture.SetupDishRepositoryFindingDishesById();
            fixture.SetupPaymentMethodRepositoryFindingPaymentMethods();
            fixture.SetupOrderRepositoryStoringOrder();

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
        public async Task HandleAsync_DeliveryOrdered_LastNameEmpty_ReturnsFailure()
        {
            // Arrange
            fixture.SetupActiveRestaurantWithDeliveryActivated();
            fixture.SetupRandomEnabledDishCategories();
            fixture.SetupRandomDishes();
            fixture.SetupPaymentMethods();
            fixture.SetupValidServiceTime();
            fixture.SetupDefaultOrderedDishes();
            fixture.SetupCommandWithLastNameEmpty(OrderType.Delivery);
            fixture.SetupRestaurantRepositoryFindingRestaurantById();
            fixture.SetupDishCategoryRepositoryFindingDishCategoriesById();
            fixture.SetupDishRepositoryFindingDishesById();
            fixture.SetupPaymentMethodRepositoryFindingPaymentMethods();
            fixture.SetupOrderRepositoryStoringOrder();

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
        public async Task HandleAsync_DeliveryOrdered_StreetNull_ReturnsFailure()
        {
            // Arrange
            fixture.SetupActiveRestaurantWithDeliveryActivated();
            fixture.SetupRandomEnabledDishCategories();
            fixture.SetupRandomDishes();
            fixture.SetupPaymentMethods();
            fixture.SetupValidServiceTime();
            fixture.SetupDefaultOrderedDishes();
            fixture.SetupCommandWithStreetNull(OrderType.Delivery);
            fixture.SetupRestaurantRepositoryFindingRestaurantById();
            fixture.SetupDishCategoryRepositoryFindingDishCategoriesById();
            fixture.SetupDishRepositoryFindingDishesById();
            fixture.SetupPaymentMethodRepositoryFindingPaymentMethods();
            fixture.SetupOrderRepositoryStoringOrder();

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
        public async Task HandleAsync_DeliveryOrdered_StreetEmpty_ReturnsFailure()
        {
            // Arrange
            fixture.SetupActiveRestaurantWithDeliveryActivated();
            fixture.SetupRandomEnabledDishCategories();
            fixture.SetupRandomDishes();
            fixture.SetupPaymentMethods();
            fixture.SetupValidServiceTime();
            fixture.SetupDefaultOrderedDishes();
            fixture.SetupCommandWithStreetEmpty(OrderType.Delivery);
            fixture.SetupRestaurantRepositoryFindingRestaurantById();
            fixture.SetupDishCategoryRepositoryFindingDishCategoriesById();
            fixture.SetupDishRepositoryFindingDishesById();
            fixture.SetupPaymentMethodRepositoryFindingPaymentMethods();
            fixture.SetupOrderRepositoryStoringOrder();

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
        public async Task HandleAsync_DeliveryOrdered_ZipCodeNull_ReturnsFailure()
        {
            // Arrange
            fixture.SetupActiveRestaurantWithDeliveryActivated();
            fixture.SetupRandomEnabledDishCategories();
            fixture.SetupRandomDishes();
            fixture.SetupPaymentMethods();
            fixture.SetupValidServiceTime();
            fixture.SetupDefaultOrderedDishes();
            fixture.SetupCommandWithZipCodeNull(OrderType.Delivery);
            fixture.SetupRestaurantRepositoryFindingRestaurantById();
            fixture.SetupDishCategoryRepositoryFindingDishCategoriesById();
            fixture.SetupDishRepositoryFindingDishesById();
            fixture.SetupPaymentMethodRepositoryFindingPaymentMethods();
            fixture.SetupOrderRepositoryStoringOrder();

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
        public async Task HandleAsync_DeliveryOrdered_ZipCodeEmpty_ReturnsFailure()
        {
            // Arrange
            fixture.SetupActiveRestaurantWithDeliveryActivated();
            fixture.SetupRandomEnabledDishCategories();
            fixture.SetupRandomDishes();
            fixture.SetupPaymentMethods();
            fixture.SetupValidServiceTime();
            fixture.SetupDefaultOrderedDishes();
            fixture.SetupCommandWithZipCodeEmpty(OrderType.Delivery);
            fixture.SetupRestaurantRepositoryFindingRestaurantById();
            fixture.SetupDishCategoryRepositoryFindingDishCategoriesById();
            fixture.SetupDishRepositoryFindingDishesById();
            fixture.SetupPaymentMethodRepositoryFindingPaymentMethods();
            fixture.SetupOrderRepositoryStoringOrder();

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
        public async Task HandleAsync_DeliveryOrdered_CityNull_ReturnsFailure()
        {
            // Arrange
            fixture.SetupActiveRestaurantWithDeliveryActivated();
            fixture.SetupRandomEnabledDishCategories();
            fixture.SetupRandomDishes();
            fixture.SetupPaymentMethods();
            fixture.SetupValidServiceTime();
            fixture.SetupDefaultOrderedDishes();
            fixture.SetupCommandWithCityNull(OrderType.Delivery);
            fixture.SetupRestaurantRepositoryFindingRestaurantById();
            fixture.SetupDishCategoryRepositoryFindingDishCategoriesById();
            fixture.SetupDishRepositoryFindingDishesById();
            fixture.SetupPaymentMethodRepositoryFindingPaymentMethods();
            fixture.SetupOrderRepositoryStoringOrder();

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
        public async Task HandleAsync_DeliveryOrdered_CityEmpty_ReturnsFailure()
        {
            // Arrange
            fixture.SetupActiveRestaurantWithDeliveryActivated();
            fixture.SetupRandomEnabledDishCategories();
            fixture.SetupRandomDishes();
            fixture.SetupPaymentMethods();
            fixture.SetupValidServiceTime();
            fixture.SetupDefaultOrderedDishes();
            fixture.SetupCommandWithCityEmpty(OrderType.Delivery);
            fixture.SetupRestaurantRepositoryFindingRestaurantById();
            fixture.SetupDishCategoryRepositoryFindingDishCategoriesById();
            fixture.SetupDishRepositoryFindingDishesById();
            fixture.SetupPaymentMethodRepositoryFindingPaymentMethods();
            fixture.SetupOrderRepositoryStoringOrder();

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
        public async Task HandleAsync_DeliveryOrdered_EmailNull_ReturnsFailure()
        {
            // Arrange
            fixture.SetupActiveRestaurantWithDeliveryActivated();
            fixture.SetupRandomEnabledDishCategories();
            fixture.SetupRandomDishes();
            fixture.SetupPaymentMethods();
            fixture.SetupValidServiceTime();
            fixture.SetupDefaultOrderedDishes();
            fixture.SetupCommandWithEmailNull(OrderType.Delivery);
            fixture.SetupRestaurantRepositoryFindingRestaurantById();
            fixture.SetupDishCategoryRepositoryFindingDishCategoriesById();
            fixture.SetupDishRepositoryFindingDishesById();
            fixture.SetupPaymentMethodRepositoryFindingPaymentMethods();
            fixture.SetupOrderRepositoryStoringOrder();

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
        public async Task HandleAsync_DeliveryOrdered_PhoneNull_ReturnsFailure()
        {
            // Arrange
            fixture.SetupActiveRestaurantWithDeliveryActivated();
            fixture.SetupRandomEnabledDishCategories();
            fixture.SetupRandomDishes();
            fixture.SetupPaymentMethods();
            fixture.SetupValidServiceTime();
            fixture.SetupDefaultOrderedDishes();
            fixture.SetupCommandWithPhoneNull(OrderType.Delivery);
            fixture.SetupRestaurantRepositoryFindingRestaurantById();
            fixture.SetupDishCategoryRepositoryFindingDishCategoriesById();
            fixture.SetupDishRepositoryFindingDishesById();
            fixture.SetupPaymentMethodRepositoryFindingPaymentMethods();
            fixture.SetupOrderRepositoryStoringOrder();

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
        public async Task HandleAsync_DeliveryOrdered_PhoneEmpty_ReturnsFailure()
        {
            // Arrange
            fixture.SetupActiveRestaurantWithDeliveryActivated();
            fixture.SetupRandomEnabledDishCategories();
            fixture.SetupRandomDishes();
            fixture.SetupPaymentMethods();
            fixture.SetupValidServiceTime();
            fixture.SetupDefaultOrderedDishes();
            fixture.SetupCommandWithPhoneEmpty(OrderType.Delivery);
            fixture.SetupRestaurantRepositoryFindingRestaurantById();
            fixture.SetupDishCategoryRepositoryFindingDishCategoriesById();
            fixture.SetupDishRepositoryFindingDishesById();
            fixture.SetupPaymentMethodRepositoryFindingPaymentMethods();
            fixture.SetupOrderRepositoryStoringOrder();

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
        public async Task HandleAsync_DeliveryOrdered_EmailEmpty_ReturnsFailure()
        {
            // Arrange
            fixture.SetupActiveRestaurantWithDeliveryActivated();
            fixture.SetupRandomEnabledDishCategories();
            fixture.SetupRandomDishes();
            fixture.SetupPaymentMethods();
            fixture.SetupValidServiceTime();
            fixture.SetupDefaultOrderedDishes();
            fixture.SetupCommandWithEmailEmpty(OrderType.Delivery);
            fixture.SetupRestaurantRepositoryFindingRestaurantById();
            fixture.SetupDishCategoryRepositoryFindingDishCategoriesById();
            fixture.SetupDishRepositoryFindingDishesById();
            fixture.SetupPaymentMethodRepositoryFindingPaymentMethods();
            fixture.SetupOrderRepositoryStoringOrder();

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
        public async Task HandleAsync_OrderIsNotPossible_ReturnsFailure()
        {
            // Arrange
            fixture.SetupActiveRestaurantWithDeliveryActivated();
            fixture.SetupRandomEnabledDishCategories();
            fixture.SetupRandomDishes();
            fixture.SetupPaymentMethods();
            fixture.SetupInvalidServiceTime();
            fixture.SetupDefaultOrderedDishes();
            fixture.SetupSuccessfulCommandWithRestaurantId(OrderType.Delivery);
            fixture.SetupRestaurantRepositoryFindingRestaurantById();
            fixture.SetupDishCategoryRepositoryFindingDishCategoriesById();
            fixture.SetupDishRepositoryFindingDishesById();
            fixture.SetupPaymentMethodRepositoryFindingPaymentMethods();
            fixture.SetupOrderRepositoryStoringOrder();

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
        public async Task HandleAsync_PaymentMethodUnknown_ReturnsFailure()
        {
            // Arrange
            fixture.SetupActiveRestaurantWithDeliveryActivated();
            fixture.SetupRandomEnabledDishCategories();
            fixture.SetupRandomDishes();
            fixture.SetupPaymentMethods();
            fixture.SetupValidServiceTime();
            fixture.SetupDefaultOrderedDishes();
            fixture.SetupSuccessfulCommandWithRestaurantId(OrderType.Delivery);
            fixture.SetupRestaurantRepositoryFindingRestaurantById();
            fixture.SetupDishCategoryRepositoryFindingDishCategoriesById();
            fixture.SetupDishRepositoryFindingDishesById();
            fixture.SetupPaymentMethodRepositoryNotFindingPaymentMethods();
            fixture.SetupOrderRepositoryStoringOrder();

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
        public async Task HandleAsync_EnabledOrderNotificationByMobile_SetsMobileNotificationAttemptToNullAndReturnsSuccess()
        {
            // Arrange
            fixture.SetupActiveRestaurantWithDeliveryActivatedAndEnabledOrderNotificationByMobile();
            fixture.SetupRandomEnabledDishCategories();
            fixture.SetupRandomDishes();
            fixture.SetupPaymentMethods();
            fixture.SetupValidServiceTime();
            fixture.SetupDefaultOrderedDishes();
            fixture.SetupSuccessfulCommandWithRestaurantId(OrderType.Delivery);
            fixture.SetupRestaurantRepositoryFindingRestaurantById();
            fixture.SetupDishCategoryRepositoryFindingDishCategoriesById();
            fixture.SetupDishRepositoryFindingDishesById();
            fixture.SetupPaymentMethodRepositoryFindingPaymentMethods();
            fixture.SetupOrderRepositoryStoringOrder();

            var testObject = fixture.CreateTestObject();
            var command = fixture.CreateSuccessfulCommand();

            // Act
            var result = await testObject.HandleAsync(command, fixture.UserWithMinimumRole, CancellationToken.None);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                fixture.StoredOrder.Should().NotBeNull();
                fixture.StoredOrder?.RestaurantMobileNotificationInfo.Should().BeNull();
            }
        }

        [Fact]
        public async Task HandleAsync_DisabledOrderNotificationByMobile_SetsMobileNotificationAttemptWithStatusAndReturnsSuccess()
        {
            // Arrange
            fixture.SetupActiveRestaurantWithDeliveryActivatedAndDisabledOrderNotificationByMobile();
            fixture.SetupRandomEnabledDishCategories();
            fixture.SetupRandomDishes();
            fixture.SetupPaymentMethods();
            fixture.SetupValidServiceTime();
            fixture.SetupDefaultOrderedDishes();
            fixture.SetupSuccessfulCommandWithRestaurantId(OrderType.Delivery);
            fixture.SetupRestaurantRepositoryFindingRestaurantById();
            fixture.SetupDishCategoryRepositoryFindingDishCategoriesById();
            fixture.SetupDishRepositoryFindingDishesById();
            fixture.SetupPaymentMethodRepositoryFindingPaymentMethods();
            fixture.SetupOrderRepositoryStoringOrder();

            var testObject = fixture.CreateTestObject();
            var command = fixture.CreateSuccessfulCommand();

            // Act
            var result = await testObject.HandleAsync(command, fixture.UserWithMinimumRole, CancellationToken.None);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                fixture.StoredOrder.Should().NotBeNull();
                fixture.StoredOrder?.RestaurantMobileNotificationInfo.Should().NotBeNull();
                fixture.StoredOrder?.RestaurantMobileNotificationInfo?.Status.Should().BeTrue();
            }
        }

        [Fact]
        public async Task HandleAsync_DefaultCommand_CreatesOrderWithCorrectData()
        {
            // Arrange
            fixture.SetupActiveRestaurantWithDeliveryActivatedAndDisabledOrderNotificationByMobile();
            fixture.SetupRandomEnabledDishCategories();
            fixture.SetupRandomDishes();
            fixture.SetupPaymentMethods();
            fixture.SetupValidServiceTime();
            fixture.SetupDefaultOrderedDishes();
            fixture.SetupSuccessfulCommandWithRestaurantId(OrderType.Delivery);
            fixture.SetupRestaurantRepositoryFindingRestaurantById();
            fixture.SetupDishCategoryRepositoryFindingDishCategoriesById();
            fixture.SetupDishRepositoryFindingDishesById();
            fixture.SetupPaymentMethodRepositoryFindingPaymentMethods();
            fixture.SetupOrderRepositoryStoringOrder();

            var testObject = fixture.CreateTestObject();
            var command = fixture.CreateSuccessfulCommand();

            // Act
            var result = await testObject.HandleAsync(command, fixture.UserWithMinimumRole, CancellationToken.None);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                fixture.StoredOrder.Should().NotBeNull();
                fixture.StoredOrder?.Id.Should().NotBeNull();
                fixture.StoredOrder?.Id.Value.Should().NotBeEmpty();
                fixture.StoredOrder?.CustomerInfo.Should().NotBeNull();
                fixture.StoredOrder?.CustomerInfo?.GivenName.Should().Be(command.GivenName);
                fixture.StoredOrder?.CustomerInfo?.LastName.Should().Be(command.LastName);
                fixture.StoredOrder?.CustomerInfo?.Street.Should().Be(command.Street);
                fixture.StoredOrder?.CustomerInfo?.AddAddressInfo.Should().Be(command.AddAddressInfo);
                fixture.StoredOrder?.CustomerInfo?.ZipCode.Should().Be(command.ZipCode);
                fixture.StoredOrder?.CustomerInfo?.City.Should().Be(command.City);
                fixture.StoredOrder?.CustomerInfo?.Phone.Should().Be(command.Phone);
                fixture.StoredOrder?.CustomerInfo?.Email.Should().Be(command.Email);
                fixture.StoredOrder?.CartInfo.Should().NotBeNull();
                fixture.StoredOrder?.CartInfo?.OrderType.Should().Be(OrderType.Delivery);
                fixture.StoredOrder?.CartInfo?.RestaurantId.Should().Be(fixture.Restaurant.Id);
            }
        }

        protected override CommandHandlerTestFixtureBase<CheckoutCommandHandler, CheckoutCommand, OrderDTO> FixtureBase
        {
            get { return fixture; }
        }

        private sealed class Fixture : CommandHandlerTestFixtureBase<CheckoutCommandHandler, CheckoutCommand, OrderDTO>
        {
            public Fixture(ITestOutputHelper output, Role? minimumRole) : base(minimumRole)
            {
                Logger = output.BuildLoggerFor<CheckoutCommandHandler>();
                RestaurantRepositoryMock = new RestaurantRepositoryMock(MockBehavior.Strict);
                DishCategoryRepositoryMock = new DishCategoryRepositoryMock(MockBehavior.Strict);
                DishRepositoryMock = new DishRepositoryMock(MockBehavior.Strict);
                PaymentMethodRepositoryMock = new PaymentMethodRepositoryMock(MockBehavior.Strict);
                OrderRepositoryMock = new OrderRepositoryMock(MockBehavior.Strict);
            }

            public ILogger<CheckoutCommandHandler> Logger { get; }

            public RestaurantRepositoryMock RestaurantRepositoryMock { get; }

            public DishCategoryRepositoryMock DishCategoryRepositoryMock { get; }

            public DishRepositoryMock DishRepositoryMock { get; }

            public PaymentMethodRepositoryMock PaymentMethodRepositoryMock { get; }

            public OrderRepositoryMock OrderRepositoryMock { get; }

            public Restaurant Restaurant { get; private set; }

            public List<DishCategory> DishCategoriesOfRestaurant { get; private set; }

            public List<Dish> DishesOfRestaurant { get; private set; }

            public List<PaymentMethod> PaymentMethods { get; private set; }

            public DateTimeOffset ServiceTime { get; private set; }

            public List<CartDishInfoDTO> OrderedDishes { get; private set; }

            public CheckoutCommand SuccessfulCommand { get; private set; }

            public Order StoredOrder { get; private set; }

            public override CheckoutCommandHandler CreateTestObject()
            {
                return new CheckoutCommandHandler(
                    Logger,
                    RestaurantRepositoryMock.Object,
                    DishCategoryRepositoryMock.Object,
                    DishRepositoryMock.Object,
                    PaymentMethodRepositoryMock.Object,
                    OrderRepositoryMock.Object
                );
            }

            public override CheckoutCommand CreateSuccessfulCommand()
            {
                return SuccessfulCommand;
            }

            public void SetupActiveRestaurantWithPickupActivated()
            {
                var pickupInfo = new PickupInfoBuilder()
                    .WithEnabled(true)
                    .WithMinimumOrderValue(5)
                    .WithMaximumOrderValue(100)
                    .WithValidConstrains()
                    .Create();

                var deliveryInfo = new DeliveryInfoBuilder()
                    .WithEnabled(false)
                    .WithValidConstrains()
                    .Create();

                var regularOpeningDays = new List<RegularOpeningDay>();
                for (var day = 0; day < 7; day++)
                {
                    regularOpeningDays.Add(new RegularOpeningDay(
                        day,
                        new[] {new OpeningPeriod(TimeSpan.FromHours(16), TimeSpan.FromHours(23.75))}
                    ));
                }

                Restaurant = new RestaurantBuilder()
                    .WithIsActive(true)
                    .WithSupportedOrderMode(SupportedOrderMode.AtNextShift)
                    .WithRegularOpeningDays(regularOpeningDays)
                    .WithPickupInfo(pickupInfo)
                    .WithDeliveryInfo(deliveryInfo)
                    .WithValidConstrains()
                    .Create();
            }

            public void SetupActiveRestaurantWithDeliveryActivated()
            {
                var pickupInfo = new PickupInfoBuilder()
                    .WithEnabled(false)
                    .WithValidConstrains()
                    .Create();

                var deliveryInfo = new DeliveryInfoBuilder()
                    .WithEnabled(true)
                    .WithMinimumOrderValue(5)
                    .WithMaximumOrderValue(100)
                    .WithValidConstrains()
                    .Create();

                var regularOpeningDays = new List<RegularOpeningDay>();
                for (var day = 0; day < 7; day++)
                {
                    regularOpeningDays.Add(new RegularOpeningDay(
                        day,
                        new[] {new OpeningPeriod(TimeSpan.FromHours(16), TimeSpan.FromHours(23.75))}
                    ));
                }

                Restaurant = new RestaurantBuilder()
                    .WithIsActive(true)
                    .WithSupportedOrderMode(SupportedOrderMode.AtNextShift)
                    .WithRegularOpeningDays(regularOpeningDays)
                    .WithPickupInfo(pickupInfo)
                    .WithDeliveryInfo(deliveryInfo)
                    .WithValidConstrains()
                    .Create();
            }

            public void SetupActiveRestaurantWithDeliveryActivatedAndEnabledOrderNotificationByMobile()
            {
                var contactInfo = new ContactInfoBuilder()
                    .WithMobile("+491234567890")
                    .WithOrderNotificationByMobile(true)
                    .WithValidConstrains()
                    .Create();

                var pickupInfo = new PickupInfoBuilder()
                    .WithEnabled(false)
                    .WithValidConstrains()
                    .Create();

                var deliveryInfo = new DeliveryInfoBuilder()
                    .WithEnabled(true)
                    .WithMinimumOrderValue(5)
                    .WithMaximumOrderValue(100)
                    .WithValidConstrains()
                    .Create();

                var regularOpeningDays = new List<RegularOpeningDay>();
                for (var day = 0; day < 7; day++)
                {
                    regularOpeningDays.Add(new RegularOpeningDay(
                        day,
                        new[] {new OpeningPeriod(TimeSpan.FromHours(16), TimeSpan.FromHours(23.75))}
                    ));
                }

                Restaurant = new RestaurantBuilder()
                    .WithIsActive(true)
                    .WithContactInfo(contactInfo)
                    .WithSupportedOrderMode(SupportedOrderMode.AtNextShift)
                    .WithRegularOpeningDays(regularOpeningDays)
                    .WithPickupInfo(pickupInfo)
                    .WithDeliveryInfo(deliveryInfo)
                    .WithValidConstrains()
                    .Create();
            }

            public void SetupActiveRestaurantWithDeliveryActivatedAndDisabledOrderNotificationByMobile()
            {
                var contactInfo = new ContactInfoBuilder()
                    .WithMobile("+491234567890")
                    .WithOrderNotificationByMobile(false)
                    .WithValidConstrains()
                    .Create();

                var pickupInfo = new PickupInfoBuilder()
                    .WithEnabled(false)
                    .WithValidConstrains()
                    .Create();

                var deliveryInfo = new DeliveryInfoBuilder()
                    .WithEnabled(true)
                    .WithMinimumOrderValue(5)
                    .WithMaximumOrderValue(100)
                    .WithValidConstrains()
                    .Create();

                var regularOpeningDays = new List<RegularOpeningDay>();
                for (var day = 0; day < 7; day++)
                {
                    regularOpeningDays.Add(new RegularOpeningDay(
                        day,
                        new[] {new OpeningPeriod(TimeSpan.FromHours(16), TimeSpan.FromHours(23.75))}
                    ));
                }

                Restaurant = new RestaurantBuilder()
                    .WithIsActive(true)
                    .WithContactInfo(contactInfo)
                    .WithSupportedOrderMode(SupportedOrderMode.AtNextShift)
                    .WithRegularOpeningDays(regularOpeningDays)
                    .WithPickupInfo(pickupInfo)
                    .WithDeliveryInfo(deliveryInfo)
                    .WithValidConstrains()
                    .Create();
            }

            public void SetupInactiveRestaurant()
            {
                Restaurant = new RestaurantBuilder()
                    .WithIsActive(false)
                    .WithValidConstrains()
                    .Create();
            }

            public void SetupRandomEnabledDishCategories()
            {
                DishCategoriesOfRestaurant = new DishCategoryBuilder()
                    .WithRestaurantId(Restaurant.Id)
                    .WithEnabled(true)
                    .WithValidConstrains()
                    .CreateMany(3).ToList();
            }

            public void SetupRandomDisabledDishCategories()
            {
                DishCategoriesOfRestaurant = new DishCategoryBuilder()
                    .WithRestaurantId(Restaurant.Id)
                    .WithEnabled(false)
                    .WithValidConstrains()
                    .CreateMany(3).ToList();
            }

            public void SetupRandomDishes()
            {
                DishesOfRestaurant = new List<Dish>();
                foreach (var dishCategory in DishCategoriesOfRestaurant)
                {
                    var dishes = new DishBuilder()
                        .WithRestaurantId(Restaurant.Id)
                        .WithCategoryId(dishCategory.Id)
                        .WithVariants(new List<DishVariant>
                        {
                            new DishVariantBuilder()
                                .WithPrice(1.23m)
                                .WithValidConstrains()
                                .Create()
                        })
                        .WithValidConstrains()
                        .CreateMany(3).ToList();
                    DishesOfRestaurant.AddRange(dishes);
                }
            }

            public void SetupPaymentMethods()
            {
                PaymentMethods = new List<PaymentMethod>
                {
                    new PaymentMethod(PaymentMethodId.Cash, "Bar", "Sie bezahlen in bar.", null)
                };
            }

            public void SetupValidServiceTime()
            {
                ServiceTime = Date.Today.AddDays(1).ToUtcDateTimeOffset().AddHours(18);
            }

            public void SetupInvalidServiceTime()
            {
                ServiceTime = Date.Today.ToUtcDateTimeOffset().AddHours(23.8);
            }

            public void SetupDefaultOrderedDishes()
            {
                OrderedDishes = new List<CartDishInfoDTO>();
                foreach (var dish in DishesOfRestaurant)
                {
                    OrderedDishes.Add(new CartDishInfoDTO(
                        Guid.NewGuid(),
                        dish.Id,
                        dish.Variants.First().VariantId,
                        1,
                        "Standard"
                    ));
                }
            }

            public void SetupOrderedDishThatCannotBeFound()
            {
                var orderedDish = new CartDishInfoDTO(
                    Guid.NewGuid(),
                    new DishId(Guid.NewGuid()),
                    Guid.NewGuid(),
                    1,
                    "Standard"
                );

                OrderedDishes = new List<CartDishInfoDTO> {orderedDish};

                DishRepositoryMock.SetupFindByDishIdAsync(orderedDish.DishId)
                    .ReturnsAsync((Dish) null);
            }

            public void SetupOrderedDishThatDoesNotBelongToRestaurant()
            {
                var variant = new DishVariantBuilder()
                    .WithPrice(1.23m)
                    .WithValidConstrains()
                    .Create();

                var dish = new DishBuilder()
                    .WithRestaurantId(new RestaurantId(Guid.NewGuid()))
                    .WithVariants(new List<DishVariant> {variant})
                    .WithValidConstrains()
                    .Create();

                var orderedDish = new CartDishInfoDTO(
                    Guid.NewGuid(),
                    dish.Id,
                    variant.VariantId,
                    1,
                    "Standard"
                );

                OrderedDishes = new List<CartDishInfoDTO> {orderedDish};

                DishRepositoryMock.SetupFindByDishIdAsync(dish.Id)
                    .ReturnsAsync(dish);
            }

            public void SetupOrderedDishVariantThatDoesNotBelongToDish()
            {
                var variant = new DishVariantBuilder()
                    .WithPrice(1.23m)
                    .WithValidConstrains()
                    .Create();

                var dish = new DishBuilder()
                    .WithRestaurantId(Restaurant.Id)
                    .WithCategoryId(DishCategoriesOfRestaurant[0].Id)
                    .WithVariants(new List<DishVariant> {variant})
                    .WithValidConstrains()
                    .Create();

                var orderedDish = new CartDishInfoDTO(
                    Guid.NewGuid(),
                    dish.Id,
                    Guid.NewGuid(),
                    1,
                    "Standard"
                );

                OrderedDishes = new List<CartDishInfoDTO> {orderedDish};

                DishRepositoryMock.SetupFindByDishIdAsync(dish.Id)
                    .ReturnsAsync(dish);
            }

            public void SetupOrderedDishesWithRemarkOfLength1001()
            {
                OrderedDishes = new List<CartDishInfoDTO>();
                foreach (var dish in DishesOfRestaurant)
                {
                    OrderedDishes.Add(new CartDishInfoDTO(
                        Guid.NewGuid(),
                        dish.Id,
                        dish.Variants.First().VariantId,
                        1,
                        RandomStringBuilder.BuildWithLength(1001)
                    ));
                }
            }

            public void SetupOrderedDishWithNegativeCount()
            {
                var firstDish = DishesOfRestaurant.First();
                OrderedDishes = new List<CartDishInfoDTO>
                {
                    new CartDishInfoDTO(
                        Guid.NewGuid(),
                        firstDish.Id,
                        firstDish.Variants.First().VariantId,
                        -1,
                        "Standard"
                    )
                };
            }

            public void SetupOrderedDishWithZeroCount()
            {
                var firstDish = DishesOfRestaurant.First();
                OrderedDishes = new List<CartDishInfoDTO>
                {
                    new CartDishInfoDTO(
                        Guid.NewGuid(),
                        firstDish.Id,
                        firstDish.Variants.First().VariantId,
                        0,
                        "Standard"
                    )
                };
            }

            public void SetupOrderedDishWithCountOfOne()
            {
                var firstDish = DishesOfRestaurant.First();
                OrderedDishes = new List<CartDishInfoDTO>
                {
                    new CartDishInfoDTO(
                        Guid.NewGuid(),
                        firstDish.Id,
                        firstDish.Variants.First().VariantId,
                        1,
                        "Standard"
                    )
                };
            }

            public void SetupOrderedDishWithCountOf100()
            {
                var firstDish = DishesOfRestaurant.First();
                OrderedDishes = new List<CartDishInfoDTO>
                {
                    new CartDishInfoDTO(
                        Guid.NewGuid(),
                        firstDish.Id,
                        firstDish.Variants.First().VariantId,
                        100,
                        "Standard"
                    )
                };
            }

            public void SetupOrderedDishWithCountOf101()
            {
                var firstDish = DishesOfRestaurant.First();
                OrderedDishes = new List<CartDishInfoDTO>
                {
                    new CartDishInfoDTO(
                        Guid.NewGuid(),
                        firstDish.Id,
                        firstDish.Variants.First().VariantId,
                        101,
                        "Standard"
                    )
                };
            }

            public void SetupSuccessfulCommandWithRestaurantId(OrderType orderType)
            {
                SuccessfulCommand = new CheckoutCommand(
                    "Max",
                    "Mustermann",
                    "Musterstraße 1",
                    "4. Stock",
                    "12345",
                    "Musterstadt",
                    "+491234567890",
                    "max@mustermann.de",
                    orderType,
                    Restaurant.Id.Value.ToString(),
                    OrderedDishes,
                    "Bitte schnell!",
                    PaymentMethodId.Cash,
                    ServiceTime
                );
            }

            public void SetupSuccessfulCommandWithRestaurantName(OrderType orderType)
            {
                SuccessfulCommand = new CheckoutCommand(
                    "Max",
                    "Mustermann",
                    "Musterstraße 1",
                    "4. Stock",
                    "12345",
                    "Musterstadt",
                    "+491234567890",
                    "max@mustermann.de",
                    orderType,
                    Restaurant.Name,
                    OrderedDishes,
                    "Bitte schnell!",
                    PaymentMethodId.Cash,
                    ServiceTime
                );
            }

            public void SetupCommandWithGivenNameNull(OrderType orderType)
            {
                SuccessfulCommand = new CheckoutCommand(
                    null,
                    "Mustermann",
                    "Musterstraße 1",
                    "4. Stock",
                    "12345",
                    "Musterstadt",
                    "+491234567890",
                    "max@mustermann.de",
                    orderType,
                    Restaurant.Id.Value.ToString(),
                    OrderedDishes,
                    "Bitte schnell!",
                    PaymentMethodId.Cash,
                    ServiceTime
                );
            }

            public void SetupCommandWithGivenNameEmpty(OrderType orderType)
            {
                SuccessfulCommand = new CheckoutCommand(
                    "",
                    "Mustermann",
                    "Musterstraße 1",
                    "4. Stock",
                    "12345",
                    "Musterstadt",
                    "+491234567890",
                    "max@mustermann.de",
                    orderType,
                    Restaurant.Id.Value.ToString(),
                    OrderedDishes,
                    "Bitte schnell!",
                    PaymentMethodId.Cash,
                    ServiceTime
                );
            }

            public void SetupCommandWithLastNameNull(OrderType orderType)
            {
                SuccessfulCommand = new CheckoutCommand(
                    "Max",
                    null,
                    "Musterstraße 1",
                    "4. Stock",
                    "12345",
                    "Musterstadt",
                    "+491234567890",
                    "max@mustermann.de",
                    orderType,
                    Restaurant.Id.Value.ToString(),
                    OrderedDishes,
                    "Bitte schnell!",
                    PaymentMethodId.Cash,
                    ServiceTime
                );
            }

            public void SetupCommandWithLastNameEmpty(OrderType orderType)
            {
                SuccessfulCommand = new CheckoutCommand(
                    "Max",
                    "",
                    "Musterstraße 1",
                    "4. Stock",
                    "12345",
                    "Musterstadt",
                    "+491234567890",
                    "max@mustermann.de",
                    orderType,
                    Restaurant.Id.Value.ToString(),
                    OrderedDishes,
                    "Bitte schnell!",
                    PaymentMethodId.Cash,
                    ServiceTime
                );
            }

            public void SetupCommandWithStreetNull(OrderType orderType)
            {
                SuccessfulCommand = new CheckoutCommand(
                    "Max",
                    "Mustermann",
                    null,
                    "4. Stock",
                    "12345",
                    "Musterstadt",
                    "+491234567890",
                    "max@mustermann.de",
                    orderType,
                    Restaurant.Id.Value.ToString(),
                    OrderedDishes,
                    "Bitte schnell!",
                    PaymentMethodId.Cash,
                    ServiceTime
                );
            }

            public void SetupCommandWithStreetEmpty(OrderType orderType)
            {
                SuccessfulCommand = new CheckoutCommand(
                    "Max",
                    "Mustermann",
                    "",
                    "4. Stock",
                    "12345",
                    "Musterstadt",
                    "+491234567890",
                    "max@mustermann.de",
                    orderType,
                    Restaurant.Id.Value.ToString(),
                    OrderedDishes,
                    "Bitte schnell!",
                    PaymentMethodId.Cash,
                    ServiceTime
                );
            }

            public void SetupCommandWithZipCodeNull(OrderType orderType)
            {
                SuccessfulCommand = new CheckoutCommand(
                    "Max",
                    "Mustermann",
                    "Musterstraße 1",
                    "4. Stock",
                    null,
                    "Musterstadt",
                    "+491234567890",
                    "max@mustermann.de",
                    orderType,
                    Restaurant.Id.Value.ToString(),
                    OrderedDishes,
                    "Bitte schnell!",
                    PaymentMethodId.Cash,
                    ServiceTime
                );
            }

            public void SetupCommandWithZipCodeEmpty(OrderType orderType)
            {
                SuccessfulCommand = new CheckoutCommand(
                    "Max",
                    "Mustermann",
                    "Musterstraße 1",
                    "4. Stock",
                    "",
                    "Musterstadt",
                    "+491234567890",
                    "max@mustermann.de",
                    orderType,
                    Restaurant.Id.Value.ToString(),
                    OrderedDishes,
                    "Bitte schnell!",
                    PaymentMethodId.Cash,
                    ServiceTime
                );
            }

            public void SetupCommandWithCityNull(OrderType orderType)
            {
                SuccessfulCommand = new CheckoutCommand(
                    "Max",
                    "Mustermann",
                    "Musterstraße 1",
                    "4. Stock",
                    "12345",
                    null,
                    "+491234567890",
                    "max@mustermann.de",
                    orderType,
                    Restaurant.Id.Value.ToString(),
                    OrderedDishes,
                    "Bitte schnell!",
                    PaymentMethodId.Cash,
                    ServiceTime
                );
            }

            public void SetupCommandWithCityEmpty(OrderType orderType)
            {
                SuccessfulCommand = new CheckoutCommand(
                    "Max",
                    "Mustermann",
                    "Musterstraße 1",
                    "4. Stock",
                    "12345",
                    "",
                    "+491234567890",
                    "max@mustermann.de",
                    orderType,
                    Restaurant.Id.Value.ToString(),
                    OrderedDishes,
                    "Bitte schnell!",
                    PaymentMethodId.Cash,
                    ServiceTime
                );
            }
            public void SetupCommandWithPhoneNull(OrderType orderType)
            {
                SuccessfulCommand = new CheckoutCommand(
                    "Max",
                    "Mustermann",
                    "Musterstraße 1",
                    "4. Stock",
                    "12345",
                    "Musterstadt",
                    null,
                    "max@mustermann.de",
                    orderType,
                    Restaurant.Id.Value.ToString(),
                    OrderedDishes,
                    "Bitte schnell!",
                    PaymentMethodId.Cash,
                    ServiceTime
                );
            }

            public void SetupCommandWithPhoneEmpty(OrderType orderType)
            {
                SuccessfulCommand = new CheckoutCommand(
                    "Max",
                    "Mustermann",
                    "Musterstraße 1",
                    "4. Stock",
                    "12345",
                    "Musterstadt",
                    "",
                    "max@mustermann.de",
                    orderType,
                    Restaurant.Id.Value.ToString(),
                    OrderedDishes,
                    "Bitte schnell!",
                    PaymentMethodId.Cash,
                    ServiceTime
                );
            }

            public void SetupCommandWithEmailNull(OrderType orderType)
            {
                SuccessfulCommand = new CheckoutCommand(
                    "Max",
                    "Mustermann",
                    "Musterstraße 1",
                    "4. Stock",
                    "12345",
                    "Musterstadt",
                    "+491234567890",
                    null,
                    orderType,
                    Restaurant.Id.Value.ToString(),
                    OrderedDishes,
                    "Bitte schnell!",
                    PaymentMethodId.Cash,
                    ServiceTime
                );
            }

            public void SetupCommandWithEmailEmpty(OrderType orderType)
            {
                SuccessfulCommand = new CheckoutCommand(
                    "Max",
                    "Mustermann",
                    "Musterstraße 1",
                    "4. Stock",
                    "12345",
                    "Musterstadt",
                    "+491234567890",
                    "",
                    orderType,
                    Restaurant.Id.Value.ToString(),
                    OrderedDishes,
                    "Bitte schnell!",
                    PaymentMethodId.Cash,
                    ServiceTime
                );
            }

            public void SetupRestaurantRepositoryFindingRestaurantById()
            {
                RestaurantRepositoryMock.SetupFindByRestaurantIdAsync(Restaurant.Id)
                    .ReturnsAsync(Restaurant);
            }

            public void SetupRestaurantRepositoryNotFindingRestaurantById()
            {
                RestaurantRepositoryMock.SetupFindByRestaurantIdAsync(Restaurant.Id)
                    .ReturnsAsync((Restaurant) null);
            }

            public void SetupRestaurantRepositoryFindingRestaurantByName()
            {
                RestaurantRepositoryMock.SetupFindByRestaurantNameAsync(Restaurant.Name)
                    .ReturnsAsync(new[] {Restaurant});
            }

            public void SetupRestaurantRepositoryNotFindingRestaurantByName()
            {
                RestaurantRepositoryMock.SetupFindByRestaurantNameAsync(Restaurant.Name)
                    .ReturnsAsync(Enumerable.Empty<Restaurant>());
            }

            public void SetupDishCategoryRepositoryFindingDishCategoriesById()
            {
                foreach (var dishCategory in DishCategoriesOfRestaurant)
                {
                    DishCategoryRepositoryMock.SetupFindByDishCategoryIdAsync(dishCategory.Id)
                        .ReturnsAsync(dishCategory);
                }
            }

            public void SetupDishRepositoryFindingDishesById()
            {
                foreach (var dish in DishesOfRestaurant)
                {
                    DishRepositoryMock.SetupFindByDishIdAsync(dish.Id)
                        .ReturnsAsync(dish);
                }
            }

            public void SetupPaymentMethodRepositoryNotFindingPaymentMethods()
            {
                foreach (var paymentMethod in PaymentMethods)
                {
                    PaymentMethodRepositoryMock.SetupFindByPaymentMethodIdAsync(paymentMethod.Id)
                        .ReturnsAsync((PaymentMethod) null);
                }
            }

            public void SetupPaymentMethodRepositoryFindingPaymentMethods()
            {
                foreach (var paymentMethod in PaymentMethods)
                {
                    PaymentMethodRepositoryMock.SetupFindByPaymentMethodIdAsync(paymentMethod.Id)
                        .ReturnsAsync(paymentMethod);
                }
            }

            public void SetupOrderRepositoryStoringOrder()
            {
                OrderRepositoryMock.SetupStoreAsync()
                    .Callback<Order, CancellationToken>((order, token) => StoredOrder = order)
                    .Returns(Task.CompletedTask);
            }

            public override void SetupForSuccessfulCommandExecution(Role? role)
            {
                SetupActiveRestaurantWithDeliveryActivated();
                SetupRandomEnabledDishCategories();
                SetupRandomDishes();
                SetupPaymentMethods();
                SetupValidServiceTime();
                SetupDefaultOrderedDishes();
                SetupSuccessfulCommandWithRestaurantId(OrderType.Delivery);
                SetupRestaurantRepositoryFindingRestaurantById();
                SetupDishCategoryRepositoryFindingDishCategoriesById();
                SetupDishRepositoryFindingDishesById();
                SetupPaymentMethodRepositoryFindingPaymentMethods();
                SetupOrderRepositoryStoringOrder();
            }
        }
    }
}
