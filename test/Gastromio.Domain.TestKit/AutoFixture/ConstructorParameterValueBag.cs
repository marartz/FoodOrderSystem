using System.Collections.Generic;

namespace Gastromio.Domain.TestKit.AutoFixture
{
    public class ConstructorParameterValueBag
    {
        private readonly IDictionary<string, object> parameterValues = new Dictionary<string, object>();

        public bool Contains(string name)
        {
            return parameterValues.ContainsKey(name);
        }

        public bool TryGet(string name, out object value)
        {
            return parameterValues.TryGetValue(name, out value);
        }

        public object Get(string name)
        {
            return TryGet(name, out var value) ? value : null;
        }

        public bool TryGet<TValue>(string name, out TValue value)
        {
            if (!parameterValues.TryGetValue(name, out var tempValue))
            {
                value = default;
                return false;
            }

            value = (TValue) tempValue;
            return true;
        }

        public TValue Get<TValue>(string name)
        {
            return TryGet<TValue>(name, out var value) ? value : default;
        }

        public object Set(string name, object value)
        {
            parameterValues[name] = value;
            return value;
        }

        public TValue Set<TValue>(string name, TValue value)
        {
            parameterValues[name] = value;
            return value;
        }
    }
}
