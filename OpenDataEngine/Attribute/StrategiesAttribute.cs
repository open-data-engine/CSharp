using System;
using System.Linq;
using System.Reflection;
using OpenDataEngine.Strategy;

namespace OpenDataEngine.Attribute
{
    [AttributeUsage(AttributeTargets.Class)]
    public class StrategiesAttribute : System.Attribute
    {
        private readonly (String Key, IStrategy Strategy)[] _strategies;

        public StrategiesAttribute(Type type)
        {
            _strategies = ((String, IStrategy)[])type.GetField("Strategies", BindingFlags.Public | BindingFlags.Static)?.GetValue(null)!;
        }

        public IStrategy this[String key] => _strategies.FirstOrDefault(kvp => kvp.Key == key).Strategy;
    }
}