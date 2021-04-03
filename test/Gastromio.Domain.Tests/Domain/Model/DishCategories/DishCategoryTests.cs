using System;
using FluentAssertions;
using FluentAssertions.Execution;
using Gastromio.Core.Domain.Model.DishCategories;
using Gastromio.Core.Domain.Model.Users;
using Gastromio.Domain.TestKit.Common;
using Gastromio.Domain.TestKit.Domain.Model.DishCategories;
using Gastromio.Domain.TestKit.Domain.Model.Users;
using Xunit;

namespace Gastromio.Domain.Tests.Domain.Model.DishCategories
{
    public class DishCategoryTests
    {
        private readonly Fixture fixture;

        public DishCategoryTests()
        {
            fixture = new Fixture();
        }

        [Fact]
        public void ChangeName_NameNull_ReturnsFailure()
        {
            // Arrange
            fixture.SetupChangedBy();
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
        public void ChangeName_NameLength101_ReturnsFailure()
        {
            // Arrange
            fixture.SetupChangedBy();
            var testObject = fixture.CreateTestObject();

            var name = RandomStringBuilder.BuildWithLength(101);

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
        public void ChangeName_NameLength100_ChangesNameAndReturnsSuccess()
        {
            // Arrange
            fixture.SetupChangedBy();
            var testObject = fixture.CreateTestObject();

            var name = RandomStringBuilder.BuildWithLength(100);

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
        public void ChangeOrderNo_OrderNoNegative_ReturnsFailure()
        {
            // Arrange
            fixture.SetupChangedBy();
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

        private sealed class Fixture
        {
            public UserId ChangedBy { get; private set; }

            public void SetupChangedBy()
            {
                ChangedBy = new UserIdBuilder().Create();
            }

            public DishCategory CreateTestObject()
            {
                return new DishCategoryBuilder()
                    .WithValidConstrains()
                    .Create();
            }
        }
    }
}
