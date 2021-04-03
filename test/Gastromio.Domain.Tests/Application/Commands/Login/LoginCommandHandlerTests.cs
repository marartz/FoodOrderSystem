using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Gastromio.Core.Application.Commands.Login;
using Gastromio.Core.Application.DTOs;
using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.Users;
using Gastromio.Domain.TestKit.Application.Ports.Persistence;
using Gastromio.Domain.TestKit.Domain.Model.Users;
using Moq;
using Xunit;

namespace Gastromio.Domain.Tests.Application.Commands.Login
{
    public class LoginCommandHandlerTests : CommandHandlerTestBase<LoginCommandHandler, LoginCommand, UserDTO>
    {
        private readonly Fixture fixture;

        public LoginCommandHandlerTests()
        {
            fixture = new Fixture(null);
        }

        [Fact]
        public async Task HandleAsync_EmailNull_ReturnsFailure()
        {
            // Arrange
            fixture.SetupForSuccessfulCommandExecution(fixture.MinimumRole);

            var testObject = fixture.CreateTestObject();
            var command = new LoginCommand(null, fixture.Password);

            // Act
            var result = await testObject.HandleAsync(command, fixture.UserWithMinimumRole, CancellationToken.None);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public async Task HandleAsync_EmailEmpty_ReturnsFailure()
        {
            // Arrange
            fixture.SetupForSuccessfulCommandExecution(fixture.MinimumRole);

            var testObject = fixture.CreateTestObject();
            var command = new LoginCommand("", fixture.Password);

            // Act
            var result = await testObject.HandleAsync(command, fixture.UserWithMinimumRole, CancellationToken.None);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public async Task HandleAsync_PasswordNull_ReturnsFailure()
        {
            // Arrange
            fixture.SetupForSuccessfulCommandExecution(fixture.MinimumRole);

            var testObject = fixture.CreateTestObject();
            var command = new LoginCommand(fixture.User.Email, null);

            // Act
            var result = await testObject.HandleAsync(command, fixture.UserWithMinimumRole, CancellationToken.None);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public async Task HandleAsync_PasswordEmpty_ReturnsFailure()
        {
            // Arrange
            fixture.SetupForSuccessfulCommandExecution(fixture.MinimumRole);

            var testObject = fixture.CreateTestObject();
            var command = new LoginCommand(fixture.User.Email, "");

            // Act
            var result = await testObject.HandleAsync(command, fixture.UserWithMinimumRole, CancellationToken.None);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public async Task HandleAsync_UserNotKnown_ReturnsFailure()
        {
            // Arrange
            fixture.SetupPassword();
            fixture.SetupRandomUserWithPassword();
            fixture.SetupUserRepositoryNotFindingUser();

            var testObject = fixture.CreateTestObject();
            var command = new LoginCommand(fixture.User.Email, "");

            // Act
            var result = await testObject.HandleAsync(command, fixture.UserWithMinimumRole, CancellationToken.None);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public async Task HandleAsync_EmailAndPasswordValid_ReturnsSuccessWithUser()
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
                var successResult = result as SuccessResult<UserDTO>;
                successResult.Should().NotBeNull();
                successResult?.Value.Should().NotBeNull();
                successResult?.Value.Id.Should().Be(fixture.User.Id.Value);
                successResult?.Value.Email.Should().Be(fixture.User.Email);
                successResult?.Value.Role.Should().Be("SystemAdmin");
            }
        }

        protected override CommandHandlerTestFixtureBase<LoginCommandHandler, LoginCommand, UserDTO> FixtureBase
        {
            get { return fixture; }
        }

        private sealed class Fixture : CommandHandlerTestFixtureBase<LoginCommandHandler, LoginCommand, UserDTO>
        {
            public Fixture(Role? minimumRole) : base(minimumRole)
            {
                UserRepositoryMock = new UserRepositoryMock(MockBehavior.Strict);
            }

            public UserRepositoryMock UserRepositoryMock { get; }

            public string Password { get; private set; }

            public User User { get; private set; }

            public override LoginCommandHandler CreateTestObject()
            {
                return new LoginCommandHandler(
                    UserRepositoryMock.Object
                );
            }

            public override LoginCommand CreateSuccessfulCommand()
            {
                return new LoginCommand(User.Email, Password);
            }

            public void SetupPassword()
            {
                Password = "Start2020!!!";
            }

            public void SetupRandomUserWithPassword()
            {
                User = new UserBuilder()
                    .WithRole(Role.SystemAdmin)
                    .Create();
                User.ChangePassword(Password, true, new UserId(Guid.NewGuid()));
            }

            public void SetupUserRepositoryFindingUser()
            {
                UserRepositoryMock.SetupFindByEmailAsync(User.Email)
                    .ReturnsAsync(User);
            }

            public void SetupUserRepositoryNotFindingUser()
            {
                UserRepositoryMock.SetupFindByEmailAsync(User.Email)
                    .ReturnsAsync((User)null);
            }

            public override void SetupForSuccessfulCommandExecution(Role? role)
            {
                SetupPassword();
                SetupRandomUserWithPassword();
                SetupUserRepositoryFindingUser();
            }
        }
    }
}
