using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OpenDataEngine.Schema
{
    public class Table: Base
    {
        public static Func<String, String> DatabaseFormatter;
        public static Func<String, String> TableFormatter;

        public readonly String Database;
        public readonly String Name;
        private readonly Object _mapping;

        public Table(dynamic mapping, String database, String? table = null)
        {
            Database = database ?? "";
            Name = table ?? "";

            _mapping = mapping;
        }

        public override String ResolvePath(String path) => $"`{DatabaseFormatter?.Invoke(Database) ?? Database}`.`{TableFormatter?.Invoke(Name) ?? Name}`";
        public override String ResolveProperty(String property, Boolean alias = false) => _mapping.Has(property) && alias
            ? $"{ResolvePath("")}.`{_mapping.ValueOf<String>(property)}` AS '{property}'"
            : $"{ResolvePath("")}.`{_mapping.ValueOf<String>(property) ?? base.ResolveProperty(property)}`";

        public override String ReverseResolveProperty(String property) => _mapping.GetType().GetProperties().SingleOrDefault(p => (p.GetValue(_mapping) as String) == property)?.Name ?? base.ReverseResolveProperty(property);

        private (String, String)[] _map;
        public (String Local, String Foreign)[] Mapping => _map ??= _mapping.GetType().GetProperties().Select(p => (p.Name, (String)p.GetValue(_mapping)!)).ToArray();
    }
}