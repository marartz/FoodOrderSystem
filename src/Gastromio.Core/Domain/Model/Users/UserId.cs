using System;
using Gastromio.Core.Common;

namespace Gastromio.Core.Domain.Model.Users
{
    public class UserId : ValueType<Guid>
    {
        public UserId(Guid value) : base(value)
        {
        }
    }
}
