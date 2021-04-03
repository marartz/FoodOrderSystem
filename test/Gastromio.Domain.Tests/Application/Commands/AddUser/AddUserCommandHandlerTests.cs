using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Gastromio.Core.Application.Commands.AddUser;
using Gastromio.Core.Application.DTOs;
using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.Users;
using Gastromio.Domain.TestKit.Application.Ports.Persistence;
using Gastromio.Domain.TestKit.Domain.Model.Users;
using Moq;
using Xunit;

namespace Gastromio.Domain.Tests.Application.Commands.AddUser
{
    public class AddUserCommandHandlerTests : CommandHandlerTestBase<AddUserCommandHandler,
        AddUserCommand, UserDTO>
    {
        private readonly Fixture fixture;

        public AddUserCommandHandlerTests()
        {
            fixture = new Fixture(Role.SystemAdmin);
        }

        // [Fact]
        // public async Task HandleAsync_UserCreationWithFailure_ReturnsFailure()
        // {
        //     // Arrange
        //     fixture.SetupUserRepositoryFindingUserByName();
        //
        //     var testObject = fixture.CreateTestObject();
        //     var command = new AddUserCommand(null);
        //
        //     // Act
        //     var result = await testObject.HandleAsync(command, fixture.UserWithMinimumRole, CancellationToken.None);
        //
        //     // Assert
        //     using (new AssertionScope())
        //     {
        //         result.Should().NotBeNull();
        //         result?.IsFailure.Should().BeTrue();
        //     }
        // }

        [Fact]
        public async Task HandleAsync_UserAlreadyKnown_ReturnsFailure()
        {
            // Arrange
            fixture.SetupRandomUserToCreate();
            fixture.SetupUserRepositoryFindingUserByEmail();

            var testObject = fixture.CreateTestObject();
            var command = fixture.CreateSuccessfulCommand();

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
        public async Task HandleAsync_AllValid_CreatesUserReturnsSuccess()
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
                fixture.UserRepositoryMock.VerifyStoreAsync(fixture.CreatedUser, Times.Once);
            }
        }

        protected override
            CommandHandlerTestFixtureBase<AddUserCommandHandler, AddUserCommand, UserDTO> FixtureBase
        {
            get { return fixture; }
        }

        private sealed class Fixture : CommandHandlerTestFixtureBase<AddUserCommandHandler,
            AddUserCommand, UserDTO>
        {
            public Fixture(Role? minimumRole) : base(minimumRole)
            {
                UserFactoryMock = new UserFactoryMock(MockBehavior.Strict);
                UserRepositoryMock = new UserRepositoryMock(MockBehavior.Strict);
            }

            public User CreatedUser { get; private set; }

            public UserFactoryMock UserFactoryMock { get; }

            public UserRepositoryMock UserRepositoryMock { get; }

            public override AddUserCommandHandler CreateTestObject()
            {
                return new AddUserCommandHandler(
                    UserFactoryMock.Object,
                    UserRepositoryMock.Object
                );
            }

            public override AddUserCommand CreateSuccessfulCommand()
            {
                return new AddUserCommand(CreatedUser.Role, CreatedUser.Email, "password");
            }

            public void SetupRandomUserToCreate()
            {
                CreatedUser = new UserBuilder()
                    .Create();
            }

            public void SetupUserRepositoryFindingUserByEmail()
            {
                UserRepositoryMock.SetupFindByEmailAsync(CreatedUser.Email)
                    .ReturnsAsync(CreatedUser);
            }

            public void SetupUserRepositoryNotFindingUserByEmail()
            {
                UserRepositoryMock.SetupFindByEmailAsync(CreatedUser.Email)
                    .ReturnsAsync((User) null);
            }

            public void SetupUserRepositoryStoringUser()
            {
                UserRepositoryMock.Setup(m => m.StoreAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);
            }

            public void SetupUserFactoryCreatingUser()
            {
                UserFactoryMock
                    .SetupCreate(CreatedUser.Role, CreatedUser.Email, "password", true, UserWithMinimumRole.Id)
                    .Returns(SuccessResult<User>.Create(CreatedUser));
            }

            public override void SetupForSuccessfulCommandExecution(Role? role)
            {
                SetupRandomUserToCreate();
                SetupUserRepositoryNotFindingUserByEmail();
                SetupUserFactoryCreatingUser();
                SetupUserRepositoryStoringUser();
            }
        }
    }
}
