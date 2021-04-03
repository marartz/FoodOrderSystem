using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using AutoFixture;
using Gastromio.Domain.TestKit.AutoFixture;

namespace Gastromio.Domain.TestKit.Common
{
    public sealed class TestObjectBuilder<T> : Fixture
    {
        private readonly ConstrainedTypeBuilder<T> constrainedTypeBuilder = new ConstrainedTypeBuilder<T>();

        public TestObjectBuilder()
        {
            Customizations.Add(new RandomDateSequenceGenerator());
            Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => Behaviors.Remove(b));
            Behaviors.Add(new OmitOnRecursionBehavior());
        }

        public T Create()
        {
            Customizations.Add(constrainedTypeBuilder);
            return this.Create<T>();
        }

        public IEnumerable<T> CreateMany(int count)
        {
            Customizations.Add(constrainedTypeBuilder);
            return this.CreateMany<T>(count);
        }

        public TestObjectBuilder<T> WithConstantConstructorArgumentFor<TValue>(string paramName, TValue value)
        {
            return WithConstrainedConstructorArgumentFor(paramName, () => value);
        }

        public TestObjectBuilder<T> WithLengthConstrainedStringConstructorArgumentFor(string paramName, int minLength,
            int maxLength, char[] chars = null)
        {
            return WithConstrainedConstructorArgumentFor(paramName,
                () => RandomStringBuilder.BuildWithRandomLength(minLength, maxLength, chars));
        }

        public TestObjectBuilder<T> WithRangeConstrainedIntegerConstructorArgumentFor(string paramName, int minValue,
            int maxValue)
        {
            return WithConstrainedConstructorArgumentFor(paramName,
                () => RandomProvider.Random.Next(minValue, maxValue));
        }

        public TestObjectBuilder<T> WithRangeConstrainedDecimalConstructorArgumentFor(string paramName,
            decimal minValue, decimal maxValue)
        {
            return WithConstrainedConstructorArgumentFor(paramName, () =>
            {
                var randomDouble = RandomProvider.Random.NextDouble();
                var range = maxValue - minValue;
                return minValue + (decimal) ((double) range * randomDouble);
            });
        }

        public TestObjectBuilder<T> WithConstrainedConstructorArgumentFor<TValue>(string paramName,
            Func<TValue> createValueFunc)
        {
            constrainedTypeBuilder.AddConstructorParameterAction((bag, context) =>
            {
                if (bag.Contains(paramName))
                    return;
                bag.Set(paramName, createValueFunc());
            });

            // Customizations.Add(
            //     new ConstrainedConstructorArgumentRelay<T, TValue>(paramName, createValueFunc));
            return this;
        }

        public TestObjectBuilder<T> WithConstantMemberValueFor<TMember>(Expression<Func<T, TMember>> memberPicker,
            TMember value)
        {
            return WithConstrainedMemberValueFor(memberPicker, () => value);
        }

        public TestObjectBuilder<T> WithConstrainedMemberValueFor<TMember>(Expression<Func<T, TMember>> memberPicker,
            Func<TMember> createValueFunc)
        {
            var memberInfo = GetMemberInfoFromPicker(memberPicker);
            Customizations.Add(
                new ConstrainedMemberValueRelay<TMember>(memberInfo, createValueFunc));
            return this;
        }

        private static MemberInfo GetMemberInfoFromPicker<TValue>(Expression<Func<T, TValue>> propertyPicker)
        {
            var expr = UnwrapIfConversionExpression(propertyPicker.Body);
            if (!(expr is MemberExpression memberExpression))
                throw new InvalidOperationException("selected member is no property");

            var memberInfo = memberExpression.Member;

            if (memberInfo is PropertyInfo propertyInfo && propertyInfo.GetSetMethod() == null)
                throw new InvalidOperationException($"property \"{memberInfo.Name}\" is read-only");

            return memberInfo;
        }

        private static Expression UnwrapIfConversionExpression(Expression exp)
        {
            return exp is UnaryExpression unaryExpression && unaryExpression.NodeType == ExpressionType.Convert
                ? unaryExpression.Operand
                : exp;
        }
    }
}
