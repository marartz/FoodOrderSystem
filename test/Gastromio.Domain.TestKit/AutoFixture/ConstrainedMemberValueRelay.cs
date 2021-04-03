using System;
using System.Reflection;
using AutoFixture.Kernel;

namespace Gastromio.Domain.TestKit.AutoFixture
{
    public class ConstrainedMemberValueRelay<TValue> : ISpecimenBuilder
    {
        private readonly Random random;
        private readonly MemberInfo memberInfo;
        private readonly Func<TValue> createValueFunc;

        public ConstrainedMemberValueRelay(MemberInfo memberInfo, Func<TValue> createValueFunc)
        {
            this.random = random;
            this.memberInfo = memberInfo;
            this.createValueFunc = createValueFunc;
        }

        public object Create(object request, ISpecimenContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (!(request is MemberInfo curMemberInfo))
            {
                return new NoSpecimen();
            }

            if (curMemberInfo != memberInfo)
            {
                return new NoSpecimen();
            }

            return createValueFunc();
        }
    }
}
