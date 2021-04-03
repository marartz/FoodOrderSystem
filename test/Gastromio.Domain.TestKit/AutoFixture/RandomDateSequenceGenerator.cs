using System;
using System.Reflection;
using AutoFixture;
using AutoFixture.Kernel;
using Gastromio.Core.Common;

namespace Gastromio.Domain.TestKit.AutoFixture
{
    public class RandomDateSequenceGenerator : ISpecimenBuilder
    {
        private readonly RandomNumericSequenceGenerator randomizer;

        public RandomDateSequenceGenerator()
        {
            var minDate = Date.Today.AddYears(-2);
            var maxDate = Date.Today.AddYears(2);
            randomizer = new RandomNumericSequenceGenerator(minDate.ToDateTime(DateTimeKind.Unspecified).Ticks,
                maxDate.ToDateTime(DateTimeKind.Unspecified).Ticks);
        }

        public RandomDateSequenceGenerator(Date minDate, Date maxDate)
        {
            if (minDate >= maxDate)
                throw new ArgumentException("The 'minDate' argument must be less than the 'maxDate'.");
            randomizer = new RandomNumericSequenceGenerator(minDate.ToDateTime(DateTimeKind.Unspecified).Ticks,
                maxDate.ToDateTime(DateTimeKind.Unspecified).Ticks);
        }

        public object Create(object request, ISpecimenContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            return !IsNotDateRequest(request)
                ? CreateRandomDate(context)
                : new NoSpecimen();
        }

        private static bool IsNotDateRequest(object request) =>
            !typeof(Date).GetTypeInfo().IsAssignableFrom(request as Type);

        private object CreateRandomDate(ISpecimenContext context)
        {
            var dateTime = new DateTime(GetRandomNumberOfTicks(context));
            return new Date(dateTime.Year, dateTime.Month, dateTime.Day);
        }

        private long GetRandomNumberOfTicks(ISpecimenContext context) =>
            (long) randomizer.Create(typeof(long), context);
    }
}
