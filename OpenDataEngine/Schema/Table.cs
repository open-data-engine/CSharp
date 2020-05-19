using System;
using System.Collections.Generic;
using System.Reflection;

namespace OpenDataEngine.Schema
{
    public class Table<TModel>: Base<TModel>
    {
        public readonly String Database;
        public readonly String Name;
        public readonly Object Mapping;

        public Table(dynamic mapping, String database = null, String table = null)
        {
            Database = database ?? typeof(TModel).Name;
            Name = table ?? typeof(TModel).Name;

            Mapping = mapping;
        }

        public override String ResolveProperty(String property) => Mapping.ValueOf<String>(property) ?? base.ResolveProperty(property);
    }
}