using System;
using System.Linq;
using System.Reflection;
using OpenDataEngine.Source;

namespace OpenDataEngine.Attribute
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SourcesAttribute : System.Attribute
    {
        private readonly (String Key, ISource Source)[] _sources;

        public SourcesAttribute(Type type)
        {
            _sources = ((String Key, ISource Source)[])type.GetField("Sources", BindingFlags.Public | BindingFlags.Static)?.GetValue(null)!;
        }

        public ISource this[String key] => _sources.FirstOrDefault(kvp => kvp.Key == key).Source;
    }
}