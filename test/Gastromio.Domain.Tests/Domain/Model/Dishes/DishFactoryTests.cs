using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using Gastromio.Core.Domain.Model.DishCategories;
using Gastromio.Core.Domain.Model.Dishes;
using Gastromio.Core.Domain.Model.Restaurants;
using Gastromio.Core.Domain.Model.Users;
using Gastromio.Domain.TestKit.Common;
using Gastromio.Domain.TestKit.Domain.Model.DishCategories;
using Gastromio.Domain.TestKit.Domain.Model.Dishes;
using Gastromio.Domain.TestKit.Domain.Model.Restaurants;
using Gastromio.Domain.TestKit.Domain.Model.Users;
using Xunit;

namespace Gastromio.Domain.Tests.Domain.Model.Dishes
{
    public class DishFactoryTests
    {
        private readonly Fixture fixture;

        public DishFactoryTests()
        {
            fixture = new Fixture();
        }

        [Fact]
        public void Create_ValidParameters_CreatesDish()
        {
            // Arrange
            fixture.SetupValidParameters();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = testObject.Create(
                fixture.RestaurantId,
                fixture.DishCategoryId,
                fixture.Name,
                fixture.Description,
                fixture.ProductInfo,
                fixture.OrderNo,
                fixture.Variants,
                fixture.CreatedBy
            );

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                result?.Value.Should().BeOfType<Dish>();
                result?.Value?.Id.Value.Should().NotBeEmpty();
                result?.Value?.Name.Should().Be(fixture.Name);
                result?.Value?.Description.Should().Be(fixture.Description);
                result?.Value?.ProductInfo.Should().Be(fixture.ProductInfo);
                result?.Value?.OrderNo.Should().Be(fixture.OrderNo);
                result?.Value?.Variants.Should().BeEquivalentTo(fixture.Variants);
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
                fixture.DishCategoryId,
                fixture.Name,
                fixture.Description,
                fixture.ProductInfo,
                fixture.OrderNo,
                fixture.Variants,
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
        public void Create_ValidParametersExceptDescription_ReturnsFailure()
        {
            // Arrange
            fixture.SetupValidParametersExceptDescription();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = testObject.Create(
                fixture.RestaurantId,
                fixture.DishCategoryId,
                fixture.Name,
                fixture.Description,
                fixture.ProductInfo,
                fixture.OrderNo,
                fixture.Variants,
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
        public void Create_ValidParametersExceptProductInfo_ReturnsFailure()
        {
            // Arrange
            fixture.SetupValidParametersExceptProductInfo();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = testObject.Create(
                fixture.RestaurantId,
                fixture.DishCategoryId,
                fixture.Name,
                fixture.Description,
                fixture.ProductInfo,
                fixture.OrderNo,
                fixture.Variants,
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
                fixture.DishCategoryId,
                fixture.Name,
                fixture.Description,
                fixture.ProductInfo,
                fixture.OrderNo,
                fixture.Variants,
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
        public void Create_ValidParametersExceptVariants_ReturnsFailure()
        {
            // Arrange
            fixture.SetupValidParametersExceptVariants();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = testObject.Create(
                fixture.RestaurantId,
                fixture.DishCategoryId,
                fixture.Name,
                fixture.Description,
                fixture.ProductInfo,
                fixture.OrderNo,
                fixture.Variants,
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

            public DishCategoryId DishCategoryId { get; private set; }

            public string Name { get; private set; }

            public string Description { get; private set; }

            public string ProductInfo { get; private set; }

            public int OrderNo { get; private set; }

            public IList<DishVariant> Variants { get; private set; }

            public UserId CreatedBy { get; private set; }

            public void SetupValidParameters()
            {
                RestaurantId = new RestaurantIdBuilder().Create();
                DishCategoryId = new DishCategoryIdBuilder().Create();
                Name = RandomStringBuilder.BuildWithLength(40);
                Description = RandomStringBuilder.BuildWithLength(200);
                ProductInfo = RandomStringBuilder.BuildWithLength(200);
                OrderNo = 1;
                Variants = new DishVariantBuilder()
                    .WithValidConstrains()
                    .CreateMany(1)
                    .ToList();
                CreatedBy = new UserIdBuilder().Create();
            }

            public void SetupValidParametersExceptName()
            {
                SetupValidParameters();
                Name = RandomStringBuilder.BuildWithLength(41);
            }

            public void SetupValidParametersExceptDescription()
            {
                SetupValidParameters();
                Description = RandomStringBuilder.BuildWithLength(201);
            }

            public void SetupValidParametersExceptProductInfo()
            {
                SetupValidParameters();
                ProductInfo = RandomStringBuilder.BuildWithLength(201);
            }

            public void SetupValidParametersExceptOrderNo()
            {
                SetupValidParameters();
                OrderNo = -1;
            }

            public void SetupValidParametersExceptVariants()
            {
                SetupValidParameters();
                var variantName = RandomStringBuilder.BuildWithLength(41);
                var variant = new DishVariantBuilder()
                    .WithName(variantName)
                    .WithValidConstrains()
                    .Create();
                Variants = new List<DishVariant> {variant};
            }

            public DishFactory CreateTestObject()
            {
                return new DishFactory();
            }
        }
    }
}
