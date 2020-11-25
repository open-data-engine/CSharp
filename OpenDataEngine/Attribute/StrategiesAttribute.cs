using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OpenDataEngine.Strategy;

namespace OpenDataEngine.Attribute
{
    [AttributeUsage(AttributeTargets.Class)]
    public class StrategiesAttribute : System.Attribute
    {
        private readonly IDictionary<String, IStrategy>? _strategies;

        public StrategiesAttribute(Type type, String property = "Strategies")
        {
            _strategies = type.GetField(property, BindingFlags.Public | BindingFlags.Static)?.GetValue(null) as IDictionary<String, IStrategy>;
        }

        public IStrategy? this[String key] => _strategies?[key];
    }
}