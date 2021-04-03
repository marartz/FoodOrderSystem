using System;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using Gastromio.Core.Domain.Model.Orders;
using Gastromio.Domain.TestKit.Common;
using Gastromio.Domain.TestKit.Domain.Model.Orders;
using Xunit;

namespace Gastromio.Domain.Tests.Domain.Model.Orders
{
    public class OrderTests
    {
        private readonly Fixture fixture;

        public OrderTests()
        {
            fixture = new Fixture();
        }

        [Fact]
        public void GetValueOfOrder_CartInfoNull_ReturnsZero()
        {
            // Arrange
            fixture.SetupCartInfoNull();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = testObject.GetValueOfOrder();

            // Assert
            result.Should().Be(0);
        }

        [Fact]
        public void GetValueOfOrder_CartInfoWithoutDishes_ReturnsZero()
        {
            // Arrange
            fixture.SetupCartInfoWithoutDishes();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = testObject.GetValueOfOrder();

            // Assert
            result.Should().Be(0);
        }

        [Fact]
        public void GetValueOfOrder_CartInfoWithDishes_ReturnsSumOfPrices()
        {
            // Arrange
            fixture.SetupRandomCartInfo();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = testObject.GetValueOfOrder();

            // Assert
            var sum = fixture.CartInfo.OrderedDishes.Sum(orderedDish => orderedDish.Count * orderedDish.VariantPrice);
            result.Should().Be(sum);
        }

        [Fact]
        public void GetTotalPrice_CostsZero_ReturnsValueOfOrder()
        {
            // Arrange
            fixture.SetupRandomCartInfo();
            fixture.SetupCostsZero();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = testObject.GetTotalPrice();

            // Assert
            var sum = fixture.CartInfo.OrderedDishes.Sum(orderedDish => orderedDish.Count * orderedDish.VariantPrice);
            result.Should().Be(sum);
        }

        [Fact]
        public void GetTotalPrice_CostsNonZero_ReturnsValueOfOrderPlusCosts()
        {
            // Arrange
            fixture.SetupRandomCartInfo();
            fixture.SetupCostsNonZero();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = testObject.GetTotalPrice();

            // Assert
            var sum = fixture.CartInfo.OrderedDishes.Sum(orderedDish => orderedDish.Count * orderedDish.VariantPrice);
            result.Should().Be(sum + fixture.Costs);
        }

        [Fact]
        public void RegisterCustomerNotificationAttempt_InfoNull_CreatesInitialInfoAndReturnsSuccess()
        {
            // Arrange
            fixture.SetupRandomCartInfo();
            fixture.SetupCustomerNotificationInfoNull();
            var testObject = fixture.CreateTestObject();

            var message = RandomStringBuilder.Build();

            // Act
            var result = testObject.RegisterCustomerNotificationAttempt(true, message);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                testObject.CustomerNotificationInfo.Should().NotBeNull();
                testObject.CustomerNotificationInfo?.Status.Should().BeTrue();
                testObject.CustomerNotificationInfo?.Attempt.Should().Be(1);
                testObject.CustomerNotificationInfo?.Message.Should().Be(message);
                testObject.CustomerNotificationInfo?.Timestamp.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
                testObject.UpdatedOn.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
            }
        }

        [Fact]
        public void RegisterCustomerNotificationAttempt_InfoNotNull_IncreasesAttemptAndReturnsSuccess()
        {
            // Arrange
            fixture.SetupRandomCartInfo();
            fixture.SetupCustomerNotificationInfoNotNull();
            var testObject = fixture.CreateTestObject();

            var message = RandomStringBuilder.Build();

            // Act
            var result = testObject.RegisterCustomerNotificationAttempt(true, message);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                testObject.CustomerNotificationInfo.Should().NotBeNull();
                testObject.CustomerNotificationInfo?.Status.Should().BeTrue();
                testObject.CustomerNotificationInfo?.Attempt.Should().Be(fixture.CustomerNotificationInfo.Attempt + 1);
                testObject.CustomerNotificationInfo?.Message.Should().Be(message);
                testObject.CustomerNotificationInfo?.Timestamp.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
                testObject.UpdatedOn.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
            }
        }

        [Fact]
        public void RegisterRestaurantEmailNotificationAttempt_InfoNull_CreatesInitialInfoAndReturnsSuccess()
        {
            // Arrange
            fixture.SetupRandomCartInfo();
            fixture.SetupRestaurantEmailNotificationInfoNull();
            var testObject = fixture.CreateTestObject();

            var message = RandomStringBuilder.Build();

            // Act
            var result = testObject.RegisterRestaurantEmailNotificationAttempt(true, message);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                testObject.RestaurantEmailNotificationInfo.Should().NotBeNull();
                testObject.RestaurantEmailNotificationInfo?.Status.Should().BeTrue();
                testObject.RestaurantEmailNotificationInfo?.Attempt.Should().Be(1);
                testObject.RestaurantEmailNotificationInfo?.Message.Should().Be(message);
                testObject.RestaurantEmailNotificationInfo?.Timestamp.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
                testObject.UpdatedOn.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
            }
        }

        [Fact]
        public void RegisterRestaurantEmailNotificationAttempt_InfoNotNull_IncreasesAttemptAndReturnsSuccess()
        {
            // Arrange
            fixture.SetupRandomCartInfo();
            fixture.SetupRestaurantEmailNotificationInfoNotNull();
            var testObject = fixture.CreateTestObject();

            var message = RandomStringBuilder.Build();

            // Act
            var result = testObject.RegisterRestaurantEmailNotificationAttempt(true, message);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                testObject.RestaurantEmailNotificationInfo.Should().NotBeNull();
                testObject.RestaurantEmailNotificationInfo?.Status.Should().BeTrue();
                testObject.RestaurantEmailNotificationInfo?.Attempt.Should().Be(fixture.RestaurantEmailNotificationInfo.Attempt + 1);
                testObject.RestaurantEmailNotificationInfo?.Message.Should().Be(message);
                testObject.RestaurantEmailNotificationInfo?.Timestamp.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
                testObject.UpdatedOn.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
            }
        }

        [Fact]
        public void RegisterRestaurantMobileNotificationAttempt_InfoNull_CreatesInitialInfoAndReturnsSuccess()
        {
            // Arrange
            fixture.SetupRandomCartInfo();
            fixture.SetupRestaurantMobileNotificationInfoNull();
            var testObject = fixture.CreateTestObject();

            var message = RandomStringBuilder.Build();

            // Act
            var result = testObject.RegisterRestaurantMobileNotificationAttempt(true, message);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                testObject.RestaurantMobileNotificationInfo.Should().NotBeNull();
                testObject.RestaurantMobileNotificationInfo?.Status.Should().BeTrue();
                testObject.RestaurantMobileNotificationInfo?.Attempt.Should().Be(1);
                testObject.RestaurantMobileNotificationInfo?.Message.Should().Be(message);
                testObject.RestaurantMobileNotificationInfo?.Timestamp.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
                testObject.UpdatedOn.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
            }
        }

        [Fact]
        public void RegisterRestaurantMobileNotificationAttempt_InfoNotNull_IncreasesAttemptAndReturnsSuccess()
        {
            // Arrange
            fixture.SetupRandomCartInfo();
            fixture.SetupRestaurantMobileNotificationInfoNotNull();
            var testObject = fixture.CreateTestObject();

            var message = RandomStringBuilder.Build();

            // Act
            var result = testObject.RegisterRestaurantMobileNotificationAttempt(true, message);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                testObject.RestaurantMobileNotificationInfo.Should().NotBeNull();
                testObject.RestaurantMobileNotificationInfo?.Status.Should().BeTrue();
                testObject.RestaurantMobileNotificationInfo?.Attempt.Should().Be(fixture.RestaurantMobileNotificationInfo.Attempt + 1);
                testObject.RestaurantMobileNotificationInfo?.Message.Should().Be(message);
                testObject.RestaurantMobileNotificationInfo?.Timestamp.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
                testObject.UpdatedOn.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
            }
        }

        private sealed class Fixture
        {
            public CartInfo CartInfo { get; private set; }

            public decimal Costs { get; private set; }

            public NotificationInfo CustomerNotificationInfo { get; private set; }

            public NotificationInfo RestaurantEmailNotificationInfo { get; private set; }

            public NotificationInfo RestaurantMobileNotificationInfo { get; private set; }

            public Order CreateTestObject()
            {
                return new OrderBuilder()
                    .WithCartInfo(CartInfo)
                    .WithCosts(Costs)
                    .WithCustomerNotificationInfo(CustomerNotificationInfo)
                    .WithRestaurantEmailNotificationInfo(RestaurantEmailNotificationInfo)
                    .WithRestaurantMobileNotificationInfo(RestaurantMobileNotificationInfo)
                    .Create();
            }

            public void SetupRandomCartInfo()
            {
                CartInfo = new CartInfoBuilder()
                    .Create();
            }

            public void SetupCartInfoNull()
            {
                CartInfo = null;
            }

            public void SetupCartInfoWithoutDishes()
            {
                CartInfo = new CartInfoBuilder()
                    .WithoutOrderedDishes()
                    .Create();
            }

            public void SetupCostsZero()
            {
                Costs = 0;
            }

            public void SetupCostsNonZero()
            {
                Costs = 1.23m;
            }

            public void SetupCustomerNotificationInfoNull()
            {
                CustomerNotificationInfo = null;
            }

            public void SetupCustomerNotificationInfoNotNull()
            {
                CustomerNotificationInfo = new NotificationInfoBuilder().Create();
            }

            public void SetupRestaurantEmailNotificationInfoNull()
            {
                RestaurantEmailNotificationInfo = null;
            }

            public void SetupRestaurantEmailNotificationInfoNotNull()
            {
                RestaurantEmailNotificationInfo = new NotificationInfoBuilder().Create();
            }

            public void SetupRestaurantMobileNotificationInfoNull()
            {
                RestaurantMobileNotificationInfo = null;
            }

            public void SetupRestaurantMobileNotificationInfoNotNull()
            {
                RestaurantMobileNotificationInfo = new NotificationInfoBuilder().Create();
            }
        }
    }
}
