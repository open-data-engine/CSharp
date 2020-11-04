using System;
using DotNetEnv;
using OpenDataEngine.Attribute;
using OpenDataEngine.Query;
using OpenDataEngine.Source;
using OpenDataEngine.Strategy;

namespace Example.Model
{
    [Strategies(typeof(Location))]
    [Sources(typeof(Location))]
    public class Location : Queryable<Location>
    {
        public UInt32? CompanyId { get; set; }
        public UInt32? LocationId { get; set; }
        public UInt32? RelationId { get; set; }
        public UInt32? ResellerId { get; set; }
        public String? ShortHash { get; set; }
        public String? Name { get; set; }
        public String? UniqueName { get; set; }

        public static readonly (String Key, ISource Source)[] Sources =
        {
            ("default", new Database(Env.GetString("DB_HOST"), Env.GetString("DB_USER"), Env.GetString("DB_PASS"), new { CompanyId = "Company_ID", LocationId = "Location_ID", RelationId = "Relation_ID", ResellerId = "Reseller_ID", Name = "Location_Name" }, "Retail", "Company_Location")),
            ("cache", new Cache()),
        };
        public static readonly (String, IStrategy)[] Strategies =
        {
            ("default", new CacheFirst("cache", "default")),
        };
    }
}
