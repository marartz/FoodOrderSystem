using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture.Kernel;

namespace Gastromio.Domain.TestKit.AutoFixture
{
    public class ConstrainedTypeBuilder<TTarget> : ISpecimenBuilder
    {
        private readonly IList<Action<ConstructorParameterValueBag, ISpecimenContext>>
            constructorParameterActionList = new List<Action<ConstructorParameterValueBag, ISpecimenContext>>();

        private readonly IList<Action<TTarget>>
            postProcessActionList = new List<Action<TTarget>>();

        public void AddConstructorParameterAction(Action<ConstructorParameterValueBag, ISpecimenContext> constructorParameterAction)
        {
            constructorParameterActionList.Add(constructorParameterAction);
        }

        public void AddPostProcessAction(Action<TTarget> postProcessAction)
        {
            postProcessActionList.Add(postProcessAction);
        }

        public object Create(object request, ISpecimenContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof (context));

            if (!(request is Type type) || type != typeof(TTarget))
            {
                return new NoSpecimen();
            }

            var constructorInfo = type.GetConstructors().First();

            var parameterValueBag = new ConstructorParameterValueBag();

            foreach (var constructorParameterAction in constructorParameterActionList)
            {
                constructorParameterAction(parameterValueBag, context);
            }

            var parameterValues = constructorInfo.GetParameters()
                .Select(pi => parameterValueBag.Contains(pi.Name)
                    ? parameterValueBag.Get(pi.Name)
                    : context.Resolve(pi)
                )
                .ToArray();

            var target = (TTarget)constructorInfo.Invoke(parameterValues);

            foreach (var postProcessAction in postProcessActionList)
            {
                postProcessAction(target);
            }

            return target;
        }
    }
}
