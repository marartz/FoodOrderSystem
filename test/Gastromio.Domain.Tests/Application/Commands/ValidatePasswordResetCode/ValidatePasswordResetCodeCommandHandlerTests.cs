using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Gastromio.Core.Application.Commands.ValidatePasswordResetCode;
using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.Users;
using Gastromio.Domain.TestKit.Application.Ports.Persistence;
using Gastromio.Domain.TestKit.Domain.Model.Users;
using Moq;
using Xunit;

namespace Gastromio.Domain.Tests.Application.Commands.ValidatePasswordResetCode
{
    public class ValidatePasswordResetCodeCommandHandlerTests : CommandHandlerTestBase<
        ValidatePasswordResetCodeCommandHandler, ValidatePasswordResetCodeCommand, bool>
    {
        private readonly Fixture fixture;

        public ValidatePasswordResetCodeCommandHandlerTests()
        {
            fixture = new Fixture(null);
        }

        [Fact]
        public async Task HandleAsync_UserNotKnown_ReturnsPasswordResetCodeIsInvalidFailure()
        {
            // Arrange
            fixture.SetupPasswordResetCode();
            fixture.SetupRandomUser();
            fixture.SetupUserRepositoryNotFindingUser();

            var testObject = fixture.CreateTestObject();
            var command = fixture.CreateSuccessfulCommand();

            // Act
            var result = await testObject.HandleAsync(command, fixture.UserWithMinimumRole, CancellationToken.None);

            // Assert
            using (new AssertionScope())
            {
                var failureResult = result as FailureResult<bool>;
                failureResult.Should().NotBeNull();
                failureResult?.Errors
                    ?.Any(en => en.Value.Any(error => error.Code == FailureResultCode.PasswordResetCodeIsInvalid))
                    .Should().BeTrue();
            }
        }

        [Fact]
        public async Task HandleAsync_ResetCodeDoesNotMatch_ReturnsPasswordResetCodeIsInvalidFailure()
        {
            // Arrange
            fixture.SetupPasswordResetCode();
            fixture.SetupRandomUser();
            fixture.SetupUserRepositoryFindingUser();

            var testObject = fixture.CreateTestObject();
            var command = new ValidatePasswordResetCodeCommand(fixture.User.Id, new byte[] {2, 3, 4, 6});

            // Act
            var result = await testObject.HandleAsync(command, fixture.UserWithMinimumRole, CancellationToken.None);

            // Assert
            using (new AssertionScope())
            {
                var failureResult = result as FailureResult<bool>;
                failureResult.Should().NotBeNull();
                failureResult?.Errors
                    ?.Any(en => en.Value.Any(error => error.Code == FailureResultCode.PasswordResetCodeIsInvalid))
                    .Should().BeTrue();
            }
        }

        [Fact]
        public async Task HandleAsync_AllValid_ValidatesResetCodeAndReturnsSuccess()
        {
            // Arrange
            fixture.SetupForSuccessfulCommandExecution(fixture.MinimumRole);

            var testObject = fixture.CreateTestObject();
            var command = fixture.CreateSuccessfulCommand();

            // Act
            var result = await testObject.HandleAsync(command, fixture.UserWithMinimumRole, CancellationToken.None);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
            }
        }

        protected override CommandHandlerTestFixtureBase<ValidatePasswordResetCodeCommandHandler,
            ValidatePasswordResetCodeCommand, bool> FixtureBase
        {
            get { return fixture; }
        }

        private sealed class Fixture : CommandHandlerTestFixtureBase<ValidatePasswordResetCodeCommandHandler,
            ValidatePasswordResetCodeCommand, bool>
        {
            public Fixture(Role? minimumRole) : base(minimumRole)
            {
                UserRepositoryMock = new UserRepositoryMock(MockBehavior.Strict);
            }

            public UserRepositoryMock UserRepositoryMock { get; }

            public byte[] PasswordResetCode { get; private set; }

            public User User { get; private set; }

            public override ValidatePasswordResetCodeCommandHandler CreateTestObject()
            {
                return new ValidatePasswordResetCodeCommandHandler(
                    UserRepositoryMock.Object
                );
            }

            public override ValidatePasswordResetCodeCommand CreateSuccessfulCommand()
            {
                return new ValidatePasswordResetCodeCommand(User.Id, PasswordResetCode);
            }

            public void SetupPasswordResetCode()
            {
                PasswordResetCode = new byte[] {1, 2, 3, 4, 5};
            }

            public void SetupRandomUser()
            {
                User = new UserBuilder()
                    .WithPasswordResetCode(PasswordResetCode)
                    .WithPasswordResetExpiration(DateTimeOffset.Now.AddMinutes(10))
                    .Create();
            }

            public void SetupUserRepositoryFindingUser()
            {
                UserRepositoryMock.SetupFindByUserIdAsync(User.Id)
                    .ReturnsAsync(User);
            }

            public void SetupUserRepositoryNotFindingUser()
            {
                UserRepositoryMock.SetupFindByUserIdAsync(User.Id)
                    .ReturnsAsync((User)null);
            }

            public override void SetupForSuccessfulCommandExecution(Role? role)
            {
                SetupPasswordResetCode();
                SetupRandomUser();
                SetupUserRepositoryFindingUser();
            }
        }
    }
}
