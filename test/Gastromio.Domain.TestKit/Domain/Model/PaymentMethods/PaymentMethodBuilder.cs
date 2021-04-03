using Gastromio.Core.Domain.Model.PaymentMethods;
using Gastromio.Domain.TestKit.Common;

namespace Gastromio.Domain.TestKit.Domain.Model.PaymentMethods
{
    public class PaymentMethodBuilder : TestObjectBuilderBase<PaymentMethod>
    {
        public PaymentMethodBuilder WithId(PaymentMethodId id)
        {
            WithConstantConstructorArgumentFor("id", id);
            return this;
        }

        public PaymentMethodBuilder WithName(string name)
        {
            WithConstantConstructorArgumentFor("name", name);
            return this;
        }

        public PaymentMethodBuilder WithDescription(string description)
        {
            WithConstantConstructorArgumentFor("description", description);
            return this;
        }

        public PaymentMethodBuilder WithImageName(string imageName)
        {
            WithConstantConstructorArgumentFor("imageName", imageName);
            return this;
        }
    }
}
