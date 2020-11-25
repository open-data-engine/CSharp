using System;
using System.Collections.Generic;
using DotNetEnv;
using OpenDataEngine.Attribute;
using OpenDataEngine.Source;

namespace Example.Model.Relation
{
    [Sources(typeof(Extra))]
    public class Extra
    {
        public enum ExtraType
        {
            OpenProvider,
            AzureActiveDirectory,
        }

        public UInt32 RelationID { get; set; }
        public ExtraType Type { get; set; }
        public String Value { get; set; }

        public static readonly IDictionary<String, ISource> Sources = new Dictionary<String, ISource>
        {
            {
                "default",
                new Database(Env.GetString("DB_HOST"), Env.GetString("DB_USER"), Env.GetString("DB_PASS"), new { RelationID = "Relation_ID" }, "General", "Relation_Extra")
            },
        };
    }
}
