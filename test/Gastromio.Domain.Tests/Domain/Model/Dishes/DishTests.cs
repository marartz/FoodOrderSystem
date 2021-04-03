using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using Gastromio.Core.Domain.Model.Dishes;
using Gastromio.Core.Domain.Model.Users;
using Gastromio.Domain.TestKit.Common;
using Gastromio.Domain.TestKit.Domain.Model.Dishes;
using Gastromio.Domain.TestKit.Domain.Model.Users;
using Xunit;

namespace Gastromio.Domain.Tests.Domain.Model.Dishes
{
    public class DishTests
    {
        private readonly Fixture fixture;

        public DishTests()
        {
            fixture = new Fixture();
        }

        [Fact]
        public void ChangeName_NameNull_ReturnsFailure()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupRandomVariants();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = testObject.ChangeName(null, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void ChangeName_NameEmpty_ReturnsFailure()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupRandomVariants();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = testObject.ChangeName(string.Empty, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void ChangeName_NameLength41_ReturnsFailure()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupRandomVariants();
            var testObject = fixture.CreateTestObject();

            var name = RandomStringBuilder.BuildWithLength(41);

            // Act
            var result = testObject.ChangeName(name, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void ChangeName_NameLength40_ChangesNameAndReturnsSuccess()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupRandomVariants();
            var testObject = fixture.CreateTestObject();

            var name = RandomStringBuilder.BuildWithLength(40);

            // Act
            var result = testObject.ChangeName(name, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                testObject.Name.Should().Be(name);
                testObject.UpdatedOn.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
                testObject.UpdatedBy.Should().Be(fixture.ChangedBy);
            }
        }

        [Fact]
        public void ChangeDescription_DescriptionNull_ReturnsSuccess()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupRandomVariants();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = testObject.ChangeDescription(null, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
            }
        }

        [Fact]
        public void ChangeDescription_DescriptionEmpty_ReturnsSuccess()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupRandomVariants();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = testObject.ChangeDescription(string.Empty, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
            }
        }

        [Fact]
        public void ChangeDescription_DescriptionLength201_ReturnsFailure()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupRandomVariants();
            var testObject = fixture.CreateTestObject();

            var description = RandomStringBuilder.BuildWithLength(201);

            // Act
            var result = testObject.ChangeDescription(description, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void ChangeDescription_DescriptionLength200_ChangesDescriptionAndReturnsSuccess()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupRandomVariants();
            var testObject = fixture.CreateTestObject();

            var description = RandomStringBuilder.BuildWithLength(200);

            // Act
            var result = testObject.ChangeDescription(description, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                testObject.Description.Should().Be(description);
                testObject.UpdatedOn.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
                testObject.UpdatedBy.Should().Be(fixture.ChangedBy);
            }
        }

        [Fact]
        public void ChangeProductInfo_ProductInfoNull_ReturnsSuccess()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupRandomVariants();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = testObject.ChangeProductInfo(null, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
            }
        }

        [Fact]
        public void ChangeProductInfo_ProductInfoEmpty_ReturnsSuccess()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupRandomVariants();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = testObject.ChangeProductInfo(string.Empty, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
            }
        }

        [Fact]
        public void ChangeProductInfo_ProductInfoLength201_ReturnsFailure()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupRandomVariants();
            var testObject = fixture.CreateTestObject();

            var productInfo = RandomStringBuilder.BuildWithLength(201);

            // Act
            var result = testObject.ChangeProductInfo(productInfo, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void ChangeProductInfo_ProductInfoLength200_ChangesProductInfoAndReturnsSuccess()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupRandomVariants();
            var testObject = fixture.CreateTestObject();

            var productInfo = RandomStringBuilder.BuildWithLength(200);

            // Act
            var result = testObject.ChangeProductInfo(productInfo, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                testObject.ProductInfo.Should().Be(productInfo);
                testObject.UpdatedOn.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
                testObject.UpdatedBy.Should().Be(fixture.ChangedBy);
            }
        }

        [Fact]
        public void ChangeOrderNo_OrderNoNegative_ReturnsFailure()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupRandomVariants();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = testObject.ChangeOrderNo(-1, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void ChangeOrderNo_OrderNoOne_ChangesOrderNoAndReturnsSuccess()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupRandomVariants();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = testObject.ChangeOrderNo(1, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                testObject.OrderNo.Should().Be(1);
                testObject.UpdatedOn.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
                testObject.UpdatedBy.Should().Be(fixture.ChangedBy);
            }
        }

        [Fact]
        public void AddVariant_VariantIdEmpty_ThrowsException()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupRandomVariants();
            var testObject = fixture.CreateTestObject();

            var variantId = Guid.Empty;
            var variantName = RandomStringBuilder.BuildWithLength(10);
            const decimal variantPrice = 12;

            // Act
            Action act = () => testObject.AddVariant(variantId, variantName, variantPrice, fixture.ChangedBy);

            // Assert
            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void AddVariant_VariantAlreadyExists_ThrowsException()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupRandomVariants();
            var testObject = fixture.CreateTestObject();

            var variantId = fixture.Variants.First().VariantId;
            var variantName = RandomStringBuilder.BuildWithLength(10);
            const decimal variantPrice = 12;

            // Act
            Action act = () => testObject.AddVariant(variantId, variantName, variantPrice, fixture.ChangedBy);

            // Assert
            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void AddVariant_VariantNameLength41_ReturnsFailure()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupRandomVariants();
            var testObject = fixture.CreateTestObject();

            var variantId = Guid.NewGuid();
            var variantName = RandomStringBuilder.BuildWithLength(41);
            const decimal variantPrice = 12;

            // Act
            var result = testObject.AddVariant(variantId, variantName, variantPrice, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void AddVariant_VariantPriceZero_ReturnsFailure()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupRandomVariants();
            var testObject = fixture.CreateTestObject();

            var variantId = Guid.NewGuid();
            var variantName = RandomStringBuilder.BuildWithLength(40);
            const decimal variantPrice = 0;

            // Act
            var result = testObject.AddVariant(variantId, variantName, variantPrice, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void AddVariant_VariantPriceNegative_ReturnsFailure()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupRandomVariants();
            var testObject = fixture.CreateTestObject();

            var variantId = Guid.NewGuid();
            var variantName = RandomStringBuilder.BuildWithLength(40);
            const decimal variantPrice = -1;

            // Act
            var result = testObject.AddVariant(variantId, variantName, variantPrice, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void AddVariant_VariantPriceGreaterThan200_ReturnsFailure()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupRandomVariants();
            var testObject = fixture.CreateTestObject();

            var variantId = Guid.NewGuid();
            var variantName = RandomStringBuilder.BuildWithLength(40);
            const decimal variantPrice = 200.01m;

            // Act
            var result = testObject.AddVariant(variantId, variantName, variantPrice, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void AddVariant_ValidParameters_AddsVariantAndReturnsSuccess()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupRandomVariants();
            var testObject = fixture.CreateTestObject();

            var variantId = Guid.NewGuid();
            var variantName = RandomStringBuilder.BuildWithLength(40);
            const decimal variantPrice = 200;

            // Act
            var result = testObject.AddVariant(variantId, variantName, variantPrice, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                testObject.Variants.Any(variant => variant.VariantId == variantId).Should().BeTrue();
                testObject.UpdatedOn.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
                testObject.UpdatedBy.Should().Be(fixture.ChangedBy);
            }
        }

        [Fact]
        public void RemoveVariant_VariantDoesNotExist_ThrowsException()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupRandomVariants();
            var testObject = fixture.CreateTestObject();

            var variantId = Guid.NewGuid();

            // Act
            Action act = () => testObject.RemoveVariant(variantId, fixture.ChangedBy);

            // Assert
            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void RemoveVariant_VariantExists_RemovesVariantsAndReturnsSuccess()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupRandomVariants();
            var testObject = fixture.CreateTestObject();

            var variantId = fixture.Variants.First().VariantId;

            // Act
            var result = testObject.RemoveVariant(variantId, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                testObject.Variants.Any(variant => variant.VariantId == variantId).Should().BeFalse();
                testObject.UpdatedOn.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
                testObject.UpdatedBy.Should().Be(fixture.ChangedBy);
            }
        }

        [Fact]
        public void ReplaceVariants_NewVariantsNull_RemovesAllVariantsAndReturnsSuccess()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupRandomVariants();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = testObject.ReplaceVariants(null, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                testObject.Variants.Should().BeEmpty();
                testObject.UpdatedOn.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
                testObject.UpdatedBy.Should().Be(fixture.ChangedBy);
            }
        }

        [Fact]
        public void ReplaceVariants_NewVariantsContainDuplicateIds_ThrowsException()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupRandomVariants();
            var testObject = fixture.CreateTestObject();

            var variantId = Guid.NewGuid();

            var newVariants = new DishVariantBuilder()
                .WithVariantId(variantId)
                .WithValidConstrains()
                .CreateMany(2);

            // Act
            Action act = () => testObject.ReplaceVariants(newVariants, fixture.ChangedBy);

            // Assert
            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void ReplaceVariant_VariantExists_ReplacesVariantsAndReturnsSuccess()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupRandomVariants();
            var testObject = fixture.CreateTestObject();

            var newVariants = new DishVariantBuilder()
                .WithValidConstrains()
                .CreateMany(2)
                .ToList();

            // Act
            var result = testObject.ReplaceVariants(newVariants, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                testObject.Variants.Should().BeEquivalentTo(newVariants);
                testObject.UpdatedOn.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
                testObject.UpdatedBy.Should().Be(fixture.ChangedBy);
            }
        }

        private sealed class Fixture
        {
            public UserId ChangedBy { get; private set; }

            public IList<DishVariant> Variants { get; private set; }

            public void SetupChangedBy()
            {
                ChangedBy = new UserIdBuilder().Create();
            }

            public void SetupRandomVariants()
            {
                Variants = new DishVariantBuilder()
                    .WithValidConstrains()
                    .CreateMany(3)
                    .ToList();
            }

            public Dish CreateTestObject()
            {
                return new DishBuilder()
                    .WithVariants(Variants)
                    .WithValidConstrains()
                    .Create();
            }
        }
    }
}
