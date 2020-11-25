using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OpenDataEngine.Source;

namespace OpenDataEngine.Attribute
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SourcesAttribute : System.Attribute
    {
        private readonly IDictionary<String, ISource>? _sources;

        public SourcesAttribute(Type type, String property = "Sources")
        {
            _sources = type.GetField(property, BindingFlags.Public | BindingFlags.Static)?.GetValue(null) as IDictionary<String, ISource>;
        }

        public ISource? this[String key] => _sources?[key];
    }
}