using System;
using FluentAssertions;
using FluentAssertions.Execution;
using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.Users;
using Gastromio.Domain.TestKit.Common;
using Gastromio.Domain.TestKit.Domain.Model.Users;
using Xunit;

namespace Gastromio.Domain.Tests.Domain.Model.Users
{
    public class UserTests
    {
        public const string ShortestValidPassword = "aB1!!!";

        private readonly Fixture fixture;

        public UserTests()
        {
            fixture = new Fixture();
        }

        [Fact]
        public void ChangeDetails_ChangesDetailsAndReturnsSuccess()
        {
            // Arrange
            fixture.SetupChangedBy();
            var testObject = fixture.CreateTestObject();

            const Role role = Role.SystemAdmin;
            var email = RandomStringBuilder.BuildWithLength(100);

            // Act
            var result = testObject.ChangeDetails(role, email, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                testObject.Role.Should().Be(role);
                testObject.Email.Should().Be(email);
                testObject.UpdatedOn.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
                testObject.UpdatedBy.Should().Be(fixture.ChangedBy);
            }
        }

        [Fact]
        public void ValidatePassword_PasswordMatches_ReturnsSuccessWithTrue()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupRandomPassword();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = testObject.ValidatePassword(fixture.Password);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                var successResult = (SuccessResult<bool>) result;
                successResult.Should().NotBeNull();
                successResult?.Value.Should().BeTrue();
            }
        }

        [Fact]
        public void ValidatePassword_PasswordDoesNotMatch_ReturnsSuccessWithFalse()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupRandomPassword();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = testObject.ValidatePassword("abc");

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                var successResult = (SuccessResult<bool>) result;
                successResult.Should().NotBeNull();
                successResult?.Value.Should().BeFalse();
            }
        }

        [Fact]
        public void ChangePassword_PasswordNull_ThrowsArgumentNullException()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupRandomPassword();
            var testObject = fixture.CreateTestObject();

            // Act
            Action act = () => testObject.ChangePassword(null, false, fixture.ChangedBy);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ChangePassword_EmptyPassword_WithoutPasswordPolicy_ReturnsSuccessResult()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupRandomPassword();
            var testObject = fixture.CreateTestObject();

            var saltBefore = testObject.PasswordSalt;
            var hashBefore = testObject.PasswordHash;

            // Act
            var result = testObject.ChangePassword("", false, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                testObject.PasswordSalt.Should().NotBeEquivalentTo(saltBefore);
                testObject.PasswordHash.Should().NotBeEquivalentTo(hashBefore);
                testObject.UpdatedOn.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
                testObject.UpdatedBy.Should().Be(fixture.ChangedBy);
            }
        }

        [Fact]
        public void ChangePassword_RandomPassword_WithoutPasswordPolicy_ReturnsSuccessResult()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupRandomPassword();
            var testObject = fixture.CreateTestObject();

            var saltBefore = testObject.PasswordSalt;
            var hashBefore = testObject.PasswordHash;

            var newPassword = RandomStringBuilder.BuildWithLength(20);

            // Act
            var result = testObject.ChangePassword(newPassword, false, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                testObject.PasswordSalt.Should().NotBeEquivalentTo(saltBefore);
                testObject.PasswordHash.Should().NotBeEquivalentTo(hashBefore);
                testObject.UpdatedOn.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
                testObject.UpdatedBy.Should().Be(fixture.ChangedBy);
            }
        }

        [Fact]
        public void ChangePassword_EmptyPassword_WithPasswordPolicy_ReturnsFailureResult()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupRandomPassword();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = testObject.ChangePassword("", true, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void ChangePassword_PasswordTooShort_WithPasswordPolicy_ReturnsFailureResult()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupRandomPassword();
            var testObject = fixture.CreateTestObject();

            const string newPassword = "aA0!";

            // Act
            var result = testObject.ChangePassword(newPassword, true, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void ChangePassword_PasswordWithoutLowerLetter_WithPasswordPolicy_ReturnsFailureResult()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupRandomPassword();
            var testObject = fixture.CreateTestObject();

            const string newPassword = "AAAABBBB1111!!!!";

            // Act
            var result = testObject.ChangePassword(newPassword, true, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void ChangePassword_PasswordWithoutUpperLetter_WithPasswordPolicy_ReturnsFailureResult()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupRandomPassword();
            var testObject = fixture.CreateTestObject();

            const string newPassword = "aaaabbbb1111!!!!";

            // Act
            var result = testObject.ChangePassword(newPassword, true, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void ChangePassword_PasswordWithoutDigit_WithPasswordPolicy_ReturnsFailureResult()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupRandomPassword();
            var testObject = fixture.CreateTestObject();

            const string newPassword = "aaaaBBBBCCCC!!!!";

            // Act
            var result = testObject.ChangePassword(newPassword, true, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void ChangePassword_PasswordWithoutSpecialCharacter_WithPasswordPolicy_ReturnsFailureResult()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupRandomPassword();
            var testObject = fixture.CreateTestObject();

            const string newPassword = "aaaaBBBB11112222";

            // Act
            var result = testObject.ChangePassword(newPassword, true, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void ChangePassword_PasswordValidWithLengthOfSix_WithPasswordPolicy_ReturnsSuccessResult()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupRandomPassword();
            var testObject = fixture.CreateTestObject();

            var saltBefore = testObject.PasswordSalt;
            var hashBefore = testObject.PasswordHash;

            // Act
            var result = testObject.ChangePassword(ShortestValidPassword, true, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                testObject.PasswordSalt.Should().NotBeEquivalentTo(saltBefore);
                testObject.PasswordHash.Should().NotBeEquivalentTo(hashBefore);
                testObject.UpdatedOn.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
                testObject.UpdatedBy.Should().Be(fixture.ChangedBy);
            }
        }

        [Fact]
        public void GeneratePasswordResetCode_GeneratesCode()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupRandomPassword();
            fixture.SetupRandomResetCodeAndFutureExpiration();
            var testObject = fixture.CreateTestObject();

            var resetCodeBefore = testObject.PasswordResetCode;

            // Act
            var result = testObject.GeneratePasswordResetCode();

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                testObject.PasswordResetCode.Should().NotBeEquivalentTo(resetCodeBefore);
                testObject.PasswordResetExpiration.Should().BeCloseTo(DateTimeOffset.UtcNow.AddMinutes(30), 1000);
            }
        }

        [Fact]
        public void ValidatePasswordResetCode_ResetCodeNull_ThrowsArgumentNullException()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupRandomPassword();
            fixture.SetupRandomResetCodeAndFutureExpiration();
            var testObject = fixture.CreateTestObject();

            // Act
            Action act = () => testObject.ValidatePasswordResetCode(null);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ValidatePasswordResetCode_CurrentResetCodeNotSet_ReturnsFailureResult()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupRandomPassword();
            fixture.SetupResetCodeAndExpirationNotSet();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = testObject.ValidatePasswordResetCode(Guid.NewGuid().ToByteArray());

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void ValidatePasswordResetCode_ExpirationInPast_ReturnsFailureResult()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupRandomPassword();
            fixture.SetupRandomResetCodeAndPastExpiration();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = testObject.ValidatePasswordResetCode(fixture.PasswordResetCode);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void ValidatePasswordResetCode_ResetCodeDoesNotMatch_ReturnsFailureResult()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupRandomPassword();
            fixture.SetupRandomResetCodeAndFutureExpiration();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = testObject.ValidatePasswordResetCode(Guid.NewGuid().ToByteArray());

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void ValidatePasswordResetCode_ResetCodeMatches_ReturnsSuccessResult()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupRandomPassword();
            fixture.SetupRandomResetCodeAndFutureExpiration();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = testObject.ValidatePasswordResetCode(fixture.PasswordResetCode);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
            }
        }

        [Fact]
        public void ChangePasswordWithResetCode_ResetCodeNull_ThrowsArgumentNullException()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupRandomPassword();
            fixture.SetupRandomResetCodeAndFutureExpiration();
            var testObject = fixture.CreateTestObject();

            // Act
            Action act = () => testObject.ChangePasswordWithResetCode(null, ShortestValidPassword);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ChangePasswordWithResetCode_CurrentResetCodeNotSet_ReturnsFailureResult()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupRandomPassword();
            fixture.SetupResetCodeAndExpirationNotSet();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = testObject.ChangePasswordWithResetCode(Guid.NewGuid().ToByteArray(), ShortestValidPassword);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void ChangePasswordWithResetCode_ExpirationInPast_ReturnsFailureResult()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupRandomPassword();
            fixture.SetupRandomResetCodeAndPastExpiration();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = testObject.ChangePasswordWithResetCode(fixture.PasswordResetCode, ShortestValidPassword);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void ChangePasswordWithResetCode_ResetCodeDoesNotMatch_ReturnsFailureResult()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupRandomPassword();
            fixture.SetupRandomResetCodeAndFutureExpiration();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = testObject.ChangePasswordWithResetCode(Guid.NewGuid().ToByteArray(), ShortestValidPassword);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void ChangePasswordWithResetCode_ResetCodeMatches_ChangesPasswordAndReturnsSuccessResult()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupRandomPassword();
            fixture.SetupRandomResetCodeAndFutureExpiration();
            var testObject = fixture.CreateTestObject();

            var saltBefore = testObject.PasswordSalt;
            var hashBefore = testObject.PasswordHash;

            // Act
            var result = testObject.ChangePasswordWithResetCode(fixture.PasswordResetCode, ShortestValidPassword);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                testObject.PasswordSalt.Should().NotBeEquivalentTo(saltBefore);
                testObject.PasswordHash.Should().NotBeEquivalentTo(hashBefore);
                testObject.PasswordResetCode.Should().BeNull();
                testObject.PasswordResetExpiration.Should().BeNull();
            }
        }

        private sealed class Fixture
        {
            public UserId ChangedBy { get; private set; }

            public string Password { get; private set; }

            public byte[] PasswordResetCode { get; private set; }

            public DateTimeOffset? PasswordResetExpiration { get; private set; }

            public void SetupChangedBy()
            {
                ChangedBy = new UserIdBuilder().Create();
            }

            public User CreateTestObject()
            {
                var user = new UserBuilder()
                    .WithPasswordResetCode(PasswordResetCode)
                    .WithPasswordResetExpiration(PasswordResetExpiration)
                    .Create();

                if (!string.IsNullOrEmpty(Password))
                {
                    user.ChangePassword(Password, false, new UserIdBuilder().Create());
                }

                return user;
            }

            public void SetupRandomPassword()
            {
                Password = RandomStringBuilder.BuildWithLength(20);
            }

            public void SetupResetCodeAndExpirationNotSet()
            {
                PasswordResetCode = null;
                PasswordResetExpiration = null;
            }

            public void SetupRandomResetCodeAndFutureExpiration()
            {
                PasswordResetCode = Guid.NewGuid().ToByteArray();
                PasswordResetExpiration = DateTimeOffset.UtcNow.AddMinutes(30);
            }

            public void SetupRandomResetCodeAndPastExpiration()
            {
                PasswordResetCode = Guid.NewGuid().ToByteArray();
                PasswordResetExpiration = DateTimeOffset.UtcNow.AddMinutes(-30);
            }
        }
    }
}
