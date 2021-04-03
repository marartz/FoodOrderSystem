using System;
using Gastromio.Core.Common;

namespace Gastromio.Core.Domain.Model.Dishes
{
    public class DishId : ValueType<Guid>
    {
        public DishId(Guid value) : base(value)
        {
        }
    }
}
