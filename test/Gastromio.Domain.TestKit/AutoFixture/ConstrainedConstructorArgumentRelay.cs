using System;
using System.Reflection;
using AutoFixture.Kernel;

namespace Gastromio.Domain.TestKit.AutoFixture
{
    public class ConstrainedConstructorArgumentRelay<TTarget, TValue> : ISpecimenBuilder
    {
        private readonly string paramName;
        private readonly Func<TValue> createValueFunc;

        public ConstrainedConstructorArgumentRelay(string paramName, Func<TValue> createValueFunc)
        {
            this.paramName = paramName;
            this.createValueFunc = createValueFunc;
        }

        public object Create(object request, ISpecimenContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (!(request is ParameterInfo parameter))
            {
                return new NoSpecimen();
            }

            if (parameter.Member.DeclaringType != typeof(TTarget) ||
                parameter.Member.MemberType != MemberTypes.Constructor ||
                parameter.ParameterType != typeof(TValue) ||
                parameter.Name != paramName)
            {
                return new NoSpecimen();
            }

            return createValueFunc();
        }
    }
}
