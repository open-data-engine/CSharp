using System;
using DotNetEnv;
using OpenDataEngine;
using OpenDataEngine.Attribute;
using OpenDataEngine.Query;
using OpenDataEngine.Source;
using OpenDataEngine.Strategy;

namespace Example.Model
{
    [Strategies(typeof(Relation))]
    [Sources(typeof(Relation))]
    public class Relation : Queryable<Relation>
    {
        public UInt32 ID { get; set; }
        public String Username { get; set; }

        public String FirstName { get; set; }
        public String MiddleName { get; set; }
        public String SurName { get; set; }

        public static new readonly (String Key, ISource Source)[] Sources =
        {
            ("default", new Database(Env.GetString("DB_HOST"), Env.GetString("DB_USER"), Env.GetString("DB_PASS"), new { ID = "Customer_ID", FirstName = "First_Name", MiddleName = "Middle_Name", SurName = "Sur_Name" }, "General", "Customer")),
            ("cache", new Cache()),
        };

        public static new readonly (String, IStrategy)[] Strategies =
        {
            ("default", new CacheFirst("cache", "default")),
        };
    }
}