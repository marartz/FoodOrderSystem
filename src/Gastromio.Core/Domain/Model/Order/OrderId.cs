using System;
using Gastromio.Core.Common;

namespace Gastromio.Core.Domain.Model.Order
{
    public class OrderId : ValueType<Guid>
    {
        public OrderId(Guid value) : base(value)
        {
        }
    }
}