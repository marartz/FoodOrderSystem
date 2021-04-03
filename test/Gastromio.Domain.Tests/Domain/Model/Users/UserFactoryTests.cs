using System;
using FluentAssertions;
using FluentAssertions.Execution;
using Gastromio.Core.Domain.Model.Users;
using Gastromio.Domain.TestKit.Common;
using Gastromio.Domain.TestKit.Domain.Model.Users;
using Xunit;

namespace Gastromio.Domain.Tests.Domain.Model.Users
{
    public class UserFactoryTests
    {
        private readonly Fixture fixture;

        public UserFactoryTests()
        {
            fixture = new Fixture();
        }

        [Fact]
        public void Create_ValidParameters_CreatesUser()
        {
            // Arrange
            fixture.SetupValidParameters();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = testObject.Create(
                fixture.Role,
                fixture.Email,
                fixture.Password,
                fixture.CheckPasswordPolicy,
                fixture.CreatedBy
            );

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                result?.Value.Should().BeOfType<User>();
                result?.Value?.Id.Value.Should().NotBeEmpty();
                result?.Value?.Role.Should().Be(fixture.Role);
                result?.Value?.Email.Should().Be(fixture.Email);
                result?.Value?.PasswordSalt.Should().NotBeNull();
                result?.Value?.PasswordHash.Should().NotBeNull();
                result?.Value?.PasswordResetCode.Should().BeNull();
                result?.Value?.PasswordResetExpiration.Should().BeNull();
                result?.Value?.CreatedOn.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
                result?.Value?.CreatedBy.Should().Be(fixture.CreatedBy);
                result?.Value?.UpdatedOn.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
                result?.Value?.UpdatedBy.Should().Be(fixture.CreatedBy);
            }
        }

        [Fact]
        public void Create_ValidParametersExceptPassword_ReturnsFailure()
        {
            // Arrange
            fixture.SetupValidParametersExceptPassword();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = testObject.Create(
                fixture.Role,
                fixture.Email,
                fixture.Password,
                fixture.CheckPasswordPolicy,
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
            public Role Role { get; private set; }

            public string Email { get; private set; }

            public string Password { get; private set; }

            public bool CheckPasswordPolicy { get; private set; }

            public UserId CreatedBy { get; private set; }

            public void SetupValidParameters()
            {
                Role = Role.SystemAdmin;
                Email = RandomStringBuilder.BuildWithLength(30);
                Password = UserTests.ShortestValidPassword;
                CheckPasswordPolicy = true;
                CreatedBy = new UserIdBuilder().Create();
            }

            public void SetupValidParametersExceptPassword()
            {
                SetupValidParameters();
                Password = "abc";
            }

            public UserFactory CreateTestObject()
            {
                return new UserFactory();
            }
        }
    }
}
