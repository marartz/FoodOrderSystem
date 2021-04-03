using System;
using FluentAssertions;
using FluentAssertions.Execution;
using Gastromio.Core.Domain.Model.DishCategories;
using Gastromio.Core.Domain.Model.Restaurants;
using Gastromio.Core.Domain.Model.Users;
using Gastromio.Domain.TestKit.Common;
using Gastromio.Domain.TestKit.Domain.Model.Restaurants;
using Gastromio.Domain.TestKit.Domain.Model.Users;
using Xunit;

namespace Gastromio.Domain.Tests.Domain.Model.DishCategories
{
    public class DishCategoryFactoryTests
    {
        private readonly Fixture fixture;

        public DishCategoryFactoryTests()
        {
            fixture = new Fixture();
        }

        [Fact]
        public void Create_ValidParameters_CreatesDishCategory()
        {
            // Arrange
            fixture.SetupValidParameters();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = testObject.Create(
                fixture.RestaurantId,
                fixture.Name,
                fixture.OrderNo,
                fixture.CreatedBy
            );

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                result?.Value.Should().BeOfType<DishCategory>();
                result?.Value?.Id.Value.Should().NotBeEmpty();
                result?.Value?.Name.Should().Be(fixture.Name);
                result?.Value?.OrderNo.Should().Be(fixture.OrderNo);
                result?.Value?.CreatedOn.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
                result?.Value?.CreatedBy.Should().Be(fixture.CreatedBy);
                result?.Value?.UpdatedOn.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
                result?.Value?.UpdatedBy.Should().Be(fixture.CreatedBy);
            }
        }

        [Fact]
        public void Create_ValidParametersExceptName_ReturnsFailure()
        {
            // Arrange
            fixture.SetupValidParametersExceptName();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = testObject.Create(
                fixture.RestaurantId,
                fixture.Name,
                fixture.OrderNo,
                fixture.CreatedBy
            );

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void Create_ValidParametersExceptOrderNo_ReturnsFailure()
        {
            // Arrange
            fixture.SetupValidParametersExceptOrderNo();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = testObject.Create(
                fixture.RestaurantId,
                fixture.Name,
                fixture.OrderNo,
                fixture.CreatedBy
            );

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        private sealed class Fixture
        {
            public RestaurantId RestaurantId { get; private set; }

            public string Name { get; private set; }

            public int OrderNo { get; private set; }

            public UserId CreatedBy { get; private set; }

            public void SetupValidParameters()
            {
                RestaurantId = new RestaurantIdBuilder().Create();
                Name = RandomStringBuilder.BuildWithLength(100);
                OrderNo = 1;
                CreatedBy = new UserIdBuilder().Create();
            }

            public void SetupValidParametersExceptName()
            {
                SetupValidParameters();
                Name = RandomStringBuilder.BuildWithLength(101);
            }

            public void SetupValidParametersExceptOrderNo()
            {
                SetupValidParameters();
                OrderNo = -1;
            }

            public DishCategoryFactory CreateTestObject()
            {
                return new DishCategoryFactory();
            }
        }
    }
}
