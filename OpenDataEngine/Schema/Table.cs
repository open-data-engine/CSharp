using System;
using System.Collections.Generic;
using System.Reflection;

namespace OpenDataEngine.Schema
{
    public static class Table
    {
        public static Func<String, String> DatabaseFormatter;
        public static Func<String, String> TableFormatter;
    }

    public class Table<TModel>: Base<TModel>
    {
        public readonly String Database;
        public readonly String Name;
        public readonly Object Mapping;

        public Table(dynamic mapping, String database, String? table = null)
        {
            Database = database ?? typeof(TModel).Name;
            Name = table ?? typeof(TModel).Name;

            Mapping = mapping;
        }

        public override String ResolvePath(String path) => $"`{Table.DatabaseFormatter?.Invoke(Database) ?? Database}`.`{Table.TableFormatter?.Invoke(Name) ?? Name}`";
        public override String ResolveProperty(String property) => $"{ResolvePath("")}.`{Mapping.ValueOf<String>(property) ?? base.ResolveProperty(property)}`";
    }
}