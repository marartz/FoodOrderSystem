using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Gastromio.Core.Domain.Model.PaymentMethods
{
    public class PaymentMethodRepository : IPaymentMethodRepository
    {
        private static readonly List<PaymentMethod> PaymentMethods = new List<PaymentMethod>
        {
            new PaymentMethod(PaymentMethodId.Cash, "Bar", "Sie bezahlen mit Bargeld", "bar"),
            new PaymentMethod(PaymentMethodId.DebitCard, "EC-Karte", "Sie bezahlen mit Ihrer EC-Karte", "maestro"),
            new PaymentMethod(PaymentMethodId.CreditCard, "Kreditkarte", "Sie bezahlen mit Ihrer Kreditkarte", "kreditkarte"),
            new PaymentMethod(PaymentMethodId.Invoice, "Rechnung", "Sie bezahlen später auf Rechnung", "rechnung")
        };

        public Task<IEnumerable<PaymentMethod>> FindAllAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(PaymentMethods.AsEnumerable());
        }

        public Task<PaymentMethod> FindByNameAsync(string name, bool ignoreCase, CancellationToken cancellationToken = default)
        {
            var stringComparison = ignoreCase
                ? StringComparison.InvariantCultureIgnoreCase
                : StringComparison.InvariantCulture;

            var paymentMethod = PaymentMethods.FirstOrDefault(pm =>
                string.Equals(pm.Name, name, stringComparison));

            return Task.FromResult(paymentMethod);
        }

        public Task<PaymentMethod> FindByPaymentMethodIdAsync(PaymentMethodId paymentMethodId, CancellationToken cancellationToken = default)
        {
            var paymentMethod = PaymentMethods.FirstOrDefault(pm => pm.Id == paymentMethodId);
            return Task.FromResult(paymentMethod);
        }
    }
}
