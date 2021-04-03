using System;
using Gastromio.Core.Common;

namespace Gastromio.Core.Domain.Model.PaymentMethods
{
    public class PaymentMethodId : ValueType<Guid>
    {
        public static readonly PaymentMethodId Cash =
            new PaymentMethodId(Guid.Parse("8DBBC822-E4FF-47B6-8CA2-68F4F0C51AA3"));

        public static readonly PaymentMethodId DebitCard =
            new PaymentMethodId(Guid.Parse("146CEA98-B5FE-45E1-AF65-E42E22A0946F"));

        public static readonly PaymentMethodId CreditCard =
            new PaymentMethodId(Guid.Parse("8ACAAEAF-9AE3-41EC-BC5C-8D0333763B78"));

        public static readonly PaymentMethodId Invoice =
            new PaymentMethodId(Guid.Parse("64951B66-C4A9-4EE0-A4D9-EA44110B178E"));

        public PaymentMethodId(Guid value) : base(value)
        {
        }
    }
}
