using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Gastromio.Core.Domain.Model.PaymentMethods
{
    public interface IPaymentMethodRepository
    {
        Task<IEnumerable<PaymentMethod>> FindAllAsync(CancellationToken cancellationToken = default);

        Task<PaymentMethod> FindByNameAsync(string name, bool ignoreCase, CancellationToken cancellationToken = default);

        Task<PaymentMethod> FindByPaymentMethodIdAsync(PaymentMethodId paymentMethodId, CancellationToken cancellationToken = default);
    }
}
