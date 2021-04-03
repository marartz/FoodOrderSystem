using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Gastromio.Core.Application.Commands.ChangePasswordWithResetCode;
using Gastromio.Core.Domain.Model.Users;
using Gastromio.Domain.TestKit.Application.Ports.Persistence;
using Gastromio.Domain.TestKit.Domain.Model.Users;
using Moq;
using Xunit;

namespace Gastromio.Domain.Tests.Application.Commands.ChangePasswordWithResetCode
{
    public class ChangePasswordWithResetCodeCommandHandlerTests : CommandHandlerTestBase<
        ChangePasswordWithResetCodeCommandHandler, ChangePasswordWithResetCodeCommand, bool>
    {
        private readonly Fixture fixture;

        public ChangePasswordWithResetCodeCommandHandlerTests()
        {
            fixture = new Fixture(null);
        }

        [Fact]
        public async Task HandleAsync_AllValid_ChangesPasswordAndReturnsSuccess()
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
                fixture.User.UpdatedOn.Should().BeCloseTo(DateTimeOffset.Now, 1000);
                fixture.UserRepositoryMock.VerifyStoreAsync(fixture.User, Times.Once);
            }
        }

        protected override CommandHandlerTestFixtureBase<ChangePasswordWithResetCodeCommandHandler,
            ChangePasswordWithResetCodeCommand, bool> FixtureBase
        {
            get { return fixture; }
        }

        private sealed class Fixture : CommandHandlerTestFixtureBase<ChangePasswordWithResetCodeCommandHandler,
            ChangePasswordWithResetCodeCommand, bool>
        {
            public Fixture(Role? minimumRole) : base(minimumRole)
            {
                UserRepositoryMock = new UserRepositoryMock(MockBehavior.Strict);
            }

            public UserRepositoryMock UserRepositoryMock { get; }

            public User User { get; private set; }

            public override ChangePasswordWithResetCodeCommandHandler CreateTestObject()
            {
                return new ChangePasswordWithResetCodeCommandHandler(
                    UserRepositoryMock.Object
                );
            }

            public override ChangePasswordWithResetCodeCommand CreateSuccessfulCommand()
            {
                return new ChangePasswordWithResetCodeCommand(User.Id, User.PasswordResetCode, "Start2020!!");
            }

            public void SetupRandomUser()
            {
                User = new UserBuilder()
                    .WithPasswordResetExpiration(DateTimeOffset.Now.AddHours(1))
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

            public void SetupUserRepositoryStoringUser()
            {
                UserRepositoryMock.SetupStoreAsync(User)
                    .Returns(Task.CompletedTask);
            }

            public override void SetupForSuccessfulCommandExecution(Role? role)
            {
                SetupRandomUser();
                SetupUserRepositoryFindingUser();
                SetupUserRepositoryStoringUser();
            }
        }
    }
}
