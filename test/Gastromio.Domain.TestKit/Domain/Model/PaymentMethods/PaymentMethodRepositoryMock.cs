using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Gastromio.Core.Domain.Model.PaymentMethods;
using Moq;
using Moq.Language.Flow;

namespace Gastromio.Domain.TestKit.Domain.Model.PaymentMethods
{
    public class PaymentMethodRepositoryMock : Mock<IPaymentMethodRepository>
    {
        public PaymentMethodRepositoryMock(MockBehavior behavior) : base(behavior)
        {
        }

        public ISetup<IPaymentMethodRepository, Task<IEnumerable<PaymentMethod>>> SetupFindAllAsync()
        {
            return Setup(m => m.FindAllAsync(It.IsAny<CancellationToken>()));
        }

        public void VerifyFindAllAsync(Func<Times> times)
        {
            Verify(m => m.FindAllAsync(It.IsAny<CancellationToken>()), times);
        }

        public ISetup<IPaymentMethodRepository, Task<PaymentMethod>> SetupFindByNameAsync(string name, bool ignoreCase)
        {
            return Setup(m => m.FindByNameAsync(name, ignoreCase, It.IsAny<CancellationToken>()));
        }

        public void VerifyFindByNameAsync(string name, bool ignoreCase, Func<Times> times)
        {
            Verify(m => m.FindByNameAsync(name, ignoreCase, It.IsAny<CancellationToken>()), times);
        }

        public ISetup<IPaymentMethodRepository, Task<PaymentMethod>> SetupFindByPaymentMethodIdAsync(
            PaymentMethodId paymentMethodId)
        {
            return Setup(m => m.FindByPaymentMethodIdAsync(paymentMethodId, It.IsAny<CancellationToken>()));
        }

        public void VerifyFindByPaymentMethodIdAsync(PaymentMethodId paymentMethodId, Func<Times> times)
        {
            Verify(m => m.FindByPaymentMethodIdAsync(paymentMethodId, It.IsAny<CancellationToken>()), times);
        }
    }
}
