using System;
using Gastromio.Core.Common;

namespace Gastromio.Core.Domain.Model.DishCategories
{
    public class DishCategoryId : ValueType<Guid>
    {
        public DishCategoryId(Guid value) : base(value)
        {
        }
    }
}
