using System;
using FluentAssertions;
using FluentAssertions.Execution;
using Gastromio.Core.Domain.Model.Cuisines;
using Gastromio.Core.Domain.Model.Users;
using Gastromio.Domain.TestKit.Common;
using Gastromio.Domain.TestKit.Domain.Model.Cuisines;
using Gastromio.Domain.TestKit.Domain.Model.Users;
using Xunit;

namespace Gastromio.Domain.Tests.Domain.Model.Cuisines
{
    public class CuisineTests
    {
        private readonly Fixture fixture;

        public CuisineTests()
        {
            fixture = new Fixture();
        }

        [Fact]
        public void ChangeName_NameNull_ReturnsFailure()
        {
            // Arrange
            fixture.SetupUpdatedBy();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = testObject.ChangeName(null, fixture.UpdatedBy);

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
            fixture.SetupUpdatedBy();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = testObject.ChangeName(string.Empty, fixture.UpdatedBy);

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
            fixture.SetupUpdatedBy();
            var testObject = fixture.CreateTestObject();

            var name = RandomStringBuilder.BuildWithLength(101);

            // Act
            var result = testObject.ChangeName(name, fixture.UpdatedBy);

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
            fixture.SetupUpdatedBy();
            var testObject = fixture.CreateTestObject();

            var name = RandomStringBuilder.BuildWithLength(100);

            // Act
            var result = testObject.ChangeName(name, fixture.UpdatedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                testObject.Name.Should().Be(name);
                testObject.UpdatedOn.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
                testObject.UpdatedBy.Should().Be(fixture.UpdatedBy);
            }
        }

        private sealed class Fixture
        {
            public UserId UpdatedBy { get; private set; }

            public void SetupUpdatedBy()
            {
                UpdatedBy = new UserIdBuilder().Create();
            }

            public Cuisine CreateTestObject()
            {
                return new CuisineBuilder()
                    .WithValidConstrains()
                    .Create();
            }
        }
    }
}
