using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Gastromio.Core.Application.Commands.RequestPasswordChange;
using Gastromio.Core.Application.Ports.Notification;
using Gastromio.Core.Application.Ports.Template;
using Gastromio.Core.Domain.Model.Users;
using Gastromio.Domain.TestKit.Application.Ports.Notification;
using Gastromio.Domain.TestKit.Application.Ports.Persistence;
using Gastromio.Domain.TestKit.Application.Ports.Template;
using Gastromio.Domain.TestKit.Domain.Model.Users;
using Moq;
using Xunit;

namespace Gastromio.Domain.Tests.Application.Commands.RequestPasswordChange
{
    public class RequestPasswordChangeCommandHandlerTests : CommandHandlerTestBase<
        RequestPasswordChangeCommandHandler, RequestPasswordChangeCommand, bool>
    {
        private readonly Fixture fixture;

        public RequestPasswordChangeCommandHandlerTests()
        {
            fixture = new Fixture(null);
        }

        [Fact]
        public async Task HandleAsync_UserNotKnown_DoesNotSendEmailAndReturnsSuccess()
        {
            // Arrange
            fixture.SetupRandomUser();
            fixture.SetupUserRepositoryNotFindingUser();

            var testObject = fixture.CreateTestObject();
            var command = fixture.CreateSuccessfulCommand();

            // Act
            var result = await testObject.HandleAsync(command, fixture.UserWithMinimumRole, CancellationToken.None);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                fixture.User.PasswordResetCode.Should().BeNull();
                fixture.User.PasswordResetExpiration.Should().BeNull();
                fixture.EmailNotificationServiceMock.VerifySendEmailNotificationAsync(Times.Never);
            }
        }

        [Fact]
        public async Task HandleAsync_AllValid_GeneratesResetCodeAndSendsEmailAndReturnsSuccess()
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
                fixture.User.PasswordResetCode.Should().NotBeNullOrEmpty();
                fixture.User.PasswordResetExpiration.Should().BeAfter(DateTimeOffset.Now);
                fixture.UserRepositoryMock.VerifyStoreAsync(fixture.User, Times.Once);
                fixture.EmailNotificationServiceMock.VerifySendEmailNotificationAsync(Times.Once);
            }
        }

        protected override CommandHandlerTestFixtureBase<RequestPasswordChangeCommandHandler,
            RequestPasswordChangeCommand, bool> FixtureBase
        {
            get { return fixture; }
        }

        private sealed class Fixture : CommandHandlerTestFixtureBase<RequestPasswordChangeCommandHandler,
            RequestPasswordChangeCommand, bool>
        {
            public Fixture(Role? minimumRole) : base(minimumRole)
            {
                UserRepositoryMock = new UserRepositoryMock(MockBehavior.Strict);
                TemplateServiceMock = new TemplateServiceMock(MockBehavior.Strict);
                EmailNotificationServiceMock = new EmailNotificationServiceMock(MockBehavior.Strict);
            }

            public UserRepositoryMock UserRepositoryMock { get; }

            public TemplateServiceMock TemplateServiceMock { get; }

            public EmailNotificationServiceMock EmailNotificationServiceMock { get; }

            public User User { get; private set; }

            public override RequestPasswordChangeCommandHandler CreateTestObject()
            {
                return new RequestPasswordChangeCommandHandler(
                    UserRepositoryMock.Object,
                    TemplateServiceMock.Object,
                    EmailNotificationServiceMock.Object
                );
            }

            public override RequestPasswordChangeCommand CreateSuccessfulCommand()
            {
                return new RequestPasswordChangeCommand(User.Email);
            }

            public void SetupRandomUser()
            {
                User = new UserBuilder()
                    .WithPasswordResetCode(null)
                    .WithPasswordResetExpiration(null)
                    .Create();
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

            public void SetupUserRepositoryStoringUser()
            {
                UserRepositoryMock.SetupStoreAsync(User)
                    .Returns(Task.CompletedTask);
            }

            public void SetupTemplateServiceGettingRequestPasswordChangeEmail()
            {
                TemplateServiceMock.SetupGetRequestPasswordChangeEmail(User.Email)
                    .Returns(new EmailData
                    {
                        Subject = "Password-Email",
                        HtmlPart = "abc",
                        TextPart = "def"
                    });
            }

            public void SetupEmailNotificationServiceSendingEmailNotification()
            {
                EmailNotificationServiceMock.SetupSendEmailNotificationAsync()
                    .ReturnsAsync(new EmailNotificationResponse(true, null));
            }

            public override void SetupForSuccessfulCommandExecution(Role? role)
            {
                SetupRandomUser();
                SetupUserRepositoryFindingUser();
                SetupUserRepositoryStoringUser();
                SetupTemplateServiceGettingRequestPasswordChangeEmail();
                SetupEmailNotificationServiceSendingEmailNotification();
            }
        }
    }
}
