using System;
using System.Threading;
using System.Threading.Tasks;
using Gastromio.Core.Application.Ports.Notification;
using Moq;
using Moq.Language.Flow;

namespace Gastromio.Domain.TestKit.Application.Ports.Notification
{
    public class EmailNotificationServiceMock : Mock<IEmailNotificationService>
    {
        public EmailNotificationServiceMock()
        {
        }

        public EmailNotificationServiceMock(MockBehavior behavior) : base(behavior)
        {
        }

        public ISetup<IEmailNotificationService, Task<EmailNotificationResponse>> SetupSendEmailNotificationAsync(
            EmailNotificationRequest emailNotificationRequest)
        {
            return Setup(m => m.SendEmailNotificationAsync(emailNotificationRequest, It.IsAny<CancellationToken>()));
        }

        public void VerifySendEmailNotificationAsync(EmailNotificationRequest emailNotificationRequest,
            Func<Times> times)
        {
            Verify(m => m.SendEmailNotificationAsync(emailNotificationRequest, It.IsAny<CancellationToken>()), times);
        }

        public ISetup<IEmailNotificationService, Task<EmailNotificationResponse>> SetupSendEmailNotificationAsync()
        {
            return Setup(m =>
                m.SendEmailNotificationAsync(It.IsAny<EmailNotificationRequest>(), It.IsAny<CancellationToken>()));
        }

        public void VerifySendEmailNotificationAsync(Func<Times> times)
        {
            Verify(
                m => m.SendEmailNotificationAsync(It.IsAny<EmailNotificationRequest>(), It.IsAny<CancellationToken>()),
                times);
        }
    }
}
