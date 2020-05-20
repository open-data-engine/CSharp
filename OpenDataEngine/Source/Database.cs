using System;
using OpenDataEngine.Adapter;
using OpenDataEngine.Connection;
using OpenDataEngine.Schema;

namespace OpenDataEngine.Source
{
    public class Database<TModel> : Source<TModel>
    {
        public Database(String host, String user, String pass, dynamic mapping, String database, String? table = null) : 
            base(new Mysql(host, user, pass), new Sql<TModel>(), new Table<TModel>(mapping, database, table)) { }
    }
}