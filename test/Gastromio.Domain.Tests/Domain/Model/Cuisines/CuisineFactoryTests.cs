using System;
using FluentAssertions;
using FluentAssertions.Execution;
using Gastromio.Core.Domain.Model.Cuisines;
using Gastromio.Core.Domain.Model.Users;
using Gastromio.Domain.TestKit.Common;
using Gastromio.Domain.TestKit.Domain.Model.Users;
using Xunit;

namespace Gastromio.Domain.Tests.Domain.Model.Cuisines
{
    public class CuisineFactoryTests
    {
        private readonly Fixture fixture;

        public CuisineFactoryTests()
        {
            fixture = new Fixture();
        }

        [Fact]
        public void Create_ValidParameters_CreatesCuisine()
        {
            // Arrange
            fixture.SetupValidParameters();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = testObject.Create(fixture.Name, fixture.CreatedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                result?.Value.Should().BeOfType<Cuisine>();
                result?.Value?.Id.Value.Should().NotBeEmpty();
                result?.Value?.Name.Should().Be(fixture.Name);
                result?.Value?.CreatedOn.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
                result?.Value?.CreatedBy.Should().Be(fixture.CreatedBy);
                result?.Value?.UpdatedOn.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
                result?.Value?.UpdatedBy.Should().Be(fixture.CreatedBy);
            }
        }

        [Fact]
        public void Create_InvalidParameters_ReturnsFailure()
        {
            // Arrange
            fixture.SetupInvalidParameters();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = testObject.Create(fixture.Name, fixture.CreatedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        private sealed class Fixture
        {
            public string Name { get; private set; }

            public UserId CreatedBy { get; private set; }

            public void SetupValidParameters()
            {
                Name = RandomStringBuilder.BuildWithLength(100);
                CreatedBy = new UserIdBuilder().Create();
            }

            public void SetupInvalidParameters()
            {
                Name = RandomStringBuilder.BuildWithLength(101);
                CreatedBy = new UserIdBuilder().Create();
            }

            public CuisineFactory CreateTestObject()
            {
                return new CuisineFactory();
            }
        }
    }
}
