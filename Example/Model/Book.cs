using System;
using OpenDataEngine.Adapter;
using OpenDataEngine.Connection;
using OpenDataEngine.Schema;
using OpenDataEngine.Query;
using OpenDataEngine.Source;

namespace Example.Model
{
    public class Book : Queryable<Book>
    {
        public UInt32 ID { get; set; }
        public UInt32 RelationID { get; set; }
        public String Title { get; set; }
        public String Publisher { get; set; }
        public DateTime PublishedAt { get; set; }

        public Relation.Relation Author { get; set; }
    }

    public class TypicodeAPI : Source
    {
        public TypicodeAPI(String url, String resource, dynamic mapping) : 
            base(
                new Http(url, new { Headers = new[] { ("Accept", "application/json"), ("Content-Type", "application/json"), ("Authorization", "Bearer {token}") } }), 
                new Json(), 
                new Rest(resource, mapping)
            ) {}
    }
}
