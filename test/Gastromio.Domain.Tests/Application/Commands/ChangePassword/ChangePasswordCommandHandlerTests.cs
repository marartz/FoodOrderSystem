using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Gastromio.Core.Application.Commands.ChangePassword;
using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.Users;
using Gastromio.Domain.TestKit.Application.Ports.Persistence;
using Gastromio.Domain.TestKit.Domain.Model.Users;
using Moq;
using Xunit;

namespace Gastromio.Domain.Tests.Application.Commands.ChangePassword
{
    public class ChangePasswordCommandHandlerTests : CommandHandlerTestBase<ChangePasswordCommandHandler,
        ChangePasswordCommand, bool>
    {
        private readonly Fixture fixture;

        public ChangePasswordCommandHandlerTests()
        {
            fixture = new Fixture(Role.Customer);
        }

        [Fact]
        public async Task HandleAsync_NoGivenCommand_ShouldThrowArgumentNullException()
        {
            // Arrange
            var testObject = fixture.CreateTestObject();

            // Act
            Func<Task> act = async () => await testObject.HandleAsync(null, fixture.UserWithMinimumRole, CancellationToken.None);

            // Assert
            using (new AssertionScope())
            {
                await act.Should().ThrowAsync<ArgumentNullException>();
            }
        }

        [Fact]
        public async Task HandleAsync_CommandWithoutPassword_ShouldThrowArgumentNullException()
        {
            // Arrange
            var testObject = fixture.CreateTestObject();

            // Act
            Func<Task> act = async () => await testObject.HandleAsync(new ChangePasswordCommand(null), fixture.UserWithMinimumRole, CancellationToken.None);

            // Assert
            using (new AssertionScope())
            {
                await act.Should()
                    .ThrowAsync<ArgumentNullException>()
                    .WithMessage("*password*");
            }
        }

        [Fact]
        public async Task HandleAsync_CommandWithInvalidPassword_ShouldFailBecauseOfInvalidPassword()
        {
            // Arrange
            var testObject = fixture.CreateTestObject();

            // Act
            var result = await testObject.HandleAsync(new ChangePasswordCommand("test"), fixture.UserWithMinimumRole, CancellationToken.None);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
                result?.Should().BeOfType(typeof(FailureResult<bool>));
                FailureResult<bool> fresult = (FailureResult<bool>)result;
                fresult.Errors.Should().HaveCount(1);
                fresult.Errors.Values.First().First().Code.Should().Be(FailureResultCode.PasswordIsNotValid);
            }
        }

        [Fact]
        public async Task HandleAsync_NoGivenUser_ShouldReturnUnauthorized()
        {
            // Arrange
            var testObject = fixture.CreateTestObject();
            var command = fixture.CreateSuccessfulCommand();

            // Act
            var result = await testObject.HandleAsync(command, null, CancellationToken.None);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
                result?.Should().BeOfType(typeof(FailureResult<bool>));
                FailureResult<bool> fresult = (FailureResult<bool>)result;
                fresult.Errors.Should().HaveCount(1);
                fresult.Errors.Values.First().First().Code.Should().Be(FailureResultCode.SessionExpired);
            }
        }

        [Fact]
        public async Task HandleAsync_AllValid_ShouldChangePasswordOfCurrentUser()
        {
            // Arrange
            fixture.SetupForSuccessfulCommandExecution(fixture.MinimumRole);

            var testObject = fixture.CreateTestObject();
            var command = fixture.CreateSuccessfulCommand();

            // Act
            var result = await testObject.HandleAsync(command, fixture.User, CancellationToken.None);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                var validationResult = fixture.User.ValidatePassword("Start2020!!!") as SuccessResult<bool>;
                validationResult.Should().NotBeNull();
                validationResult?.Value.Should().BeTrue();
                fixture.UserRepositoryMock.VerifyStoreAsync(fixture.User, Times.Once);
            }
        }

        protected override
            CommandHandlerTestFixtureBase<ChangePasswordCommandHandler, ChangePasswordCommand, bool> FixtureBase
        {
            get { return fixture; }
        }

        private sealed class Fixture : CommandHandlerTestFixtureBase<ChangePasswordCommandHandler,
            ChangePasswordCommand, bool>
        {
            public Fixture(Role? minimumRole) : base(minimumRole)
            {
                UserRepositoryMock = new UserRepositoryMock(MockBehavior.Strict);
            }

            public User User { get; private set; }

            public UserRepositoryMock UserRepositoryMock { get; }

            public string NewPassword { get; private set; }

            public override ChangePasswordCommandHandler CreateTestObject()
            {
                return new ChangePasswordCommandHandler(
                    UserRepositoryMock.Object
                );
            }

            public override ChangePasswordCommand CreateSuccessfulCommand()
            {
                return new ChangePasswordCommand(NewPassword);
            }

            public void SetupUser(Role? role)
            {
                User = new UserBuilder()
                    .WithRole(role ?? Role.Customer)
                    .WithEmail("test@test.de")
                    .Create();
            }

            public void SetupNewPassword()
            {
                NewPassword = "Start2020!!!";
            }

            public void SetupUserRepositoryStoringUser()
            {
                UserRepositoryMock.Setup(m => m.StoreAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);
            }

            public override void SetupForSuccessfulCommandExecution(Role? role)
            {
                SetupUser(role);
                SetupNewPassword();
                SetupUserRepositoryStoringUser();
            }
        }
    }
}
