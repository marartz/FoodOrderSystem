using System;
using Gastromio.Core.Application.Ports.Template;
using Gastromio.Core.Domain.Model.Orders;
using Moq;
using Moq.Language.Flow;

namespace Gastromio.Domain.TestKit.Application.Ports.Template
{
    public class TemplateServiceMock : Mock<ITemplateService>
    {
        public TemplateServiceMock()
        {
        }

        public TemplateServiceMock(MockBehavior behavior) : base(behavior)
        {
        }

        public ISetup<ITemplateService, EmailData> SetupGetCustomerEmail(Order order)
        {
            return Setup(m => m.GetCustomerEmail(order));
        }

        public void VerifyGetCustomerEmail(Order order, Func<Times> times)
        {
            Verify(m => m.GetCustomerEmail(order), times);
        }

        public ISetup<ITemplateService, EmailData> SetupGetRestaurantEmail(Order order)
        {
            return Setup(m => m.GetRestaurantEmail(order));
        }

        public void VerifyGetRestaurantEmail(Order order, Func<Times> times)
        {
            Verify(m => m.GetRestaurantEmail(order), times);
        }

        public ISetup<ITemplateService, EmailData> SetupGetRequestPasswordChangeEmail(string email, string url)
        {
            return Setup(m => m.GetRequestPasswordChangeEmail(email, url));
        }

        public void VerifyGetRequestPasswordChangeEmail(string email, string url, Func<Times> times)
        {
            Verify(m => m.GetRequestPasswordChangeEmail(email, url), times);
        }

        public ISetup<ITemplateService, EmailData> SetupGetRequestPasswordChangeEmail(string email)
        {
            return Setup(m => m.GetRequestPasswordChangeEmail(email, It.IsAny<string>()));
        }

        public void VerifyGetRequestPasswordChangeEmail(string email, Func<Times> times)
        {
            Verify(m => m.GetRequestPasswordChangeEmail(email, It.IsAny<string>()), times);
        }

        public ISetup<ITemplateService, string> SetupGetRestaurantMobileMessage(Order order)
        {
            return Setup(m => m.GetRestaurantMobileMessage(order));
        }

        public void VerifyGetRestaurantMobileMessage(Order order, Func<Times> times)
        {
            Verify(m => m.GetRestaurantMobileMessage(order), times);
        }
    }
}
