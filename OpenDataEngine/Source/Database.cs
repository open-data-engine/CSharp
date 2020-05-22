using System;
using OpenDataEngine.Adapter;
using OpenDataEngine.Connection;
using OpenDataEngine.Schema;

namespace OpenDataEngine.Source
{
    public class Database : Source
    {
        public Database(String host, String user, String pass, dynamic mapping, String database, String? table = null) : 
            base(new Mysql(host, user, pass), new Sql(), new Table(mapping, database, table)) { }
    }
}