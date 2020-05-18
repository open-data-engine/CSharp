using System;
using System.Reflection;

namespace OpenDataEngine.Schema
{
    public class Table<TModel>: Base
    {
        public readonly String Database;
        public readonly String Name;
        public readonly dynamic Mapping;

        public Table(dynamic mapping, String database = null, String table = null)
        {
            Database = database ?? typeof(TModel).Name;
            Name = table ?? typeof(TModel).Name;

            Mapping = mapping;
        }
    }
}