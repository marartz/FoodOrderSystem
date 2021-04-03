using System;
using System.Collections.Generic;

namespace Gastromio.Domain.TestKit.Common
{
    public abstract class TestObjectBuilderBase<T>
    {
        private readonly TestObjectBuilder<T> testObjectBuilder;

        protected TestObjectBuilderBase()
        {
            testObjectBuilder = new TestObjectBuilder<T>();
        }

        public T Create()
        {
            return testObjectBuilder.Create();
        }

        public IEnumerable<T> CreateMany(int count)
        {
            return testObjectBuilder.CreateMany(count);
        }

        protected void WithConstantConstructorArgumentFor<TValue>(string paramName, TValue value)
        {
            testObjectBuilder.WithConstantConstructorArgumentFor(paramName, value);
        }

        protected void WithRangeConstrainedIntegerConstructorArgumentFor(string paramName, int minValue, int maxValue)
        {
            testObjectBuilder.WithRangeConstrainedIntegerConstructorArgumentFor(paramName, minValue, maxValue);
        }

        protected void WithRangeConstrainedDecimalConstructorArgumentFor(string paramName, decimal minValue,
            decimal maxValue)
        {
            testObjectBuilder.WithRangeConstrainedDecimalConstructorArgumentFor(paramName, minValue, maxValue);
        }

        protected void WithLengthConstrainedStringConstructorArgumentFor(string paramName, int minLength, int maxLength,
            char[] chars = null)
        {
            testObjectBuilder.WithLengthConstrainedStringConstructorArgumentFor(paramName, minLength, maxLength, chars);
        }

        protected void WithConstrainedConstructorArgumentFor<TValue>(string paramName,
            Func<TValue> createValueFunc)
        {
            testObjectBuilder.WithConstrainedConstructorArgumentFor(paramName, createValueFunc);
        }
    }
}
