using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OpenDataEngine.Relation;

namespace OpenDataEngine.Attribute
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RelationsAttribute : System.Attribute
    {
        private readonly IEnumerable<IRelation>? _relations;

        public RelationsAttribute(Type type, String property = "Relations")
        {
            _relations = type.GetField(property, BindingFlags.Public | BindingFlags.Static)?.GetValue(null) as IEnumerable<IRelation>;
        }

        public IRelation? this[String name] => _relations?.FirstOrDefault(r => r.Name == name);
    }
}
