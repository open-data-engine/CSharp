using System;
using OpenDataEngine.Adapter;
using OpenDataEngine.Connection;
using OpenDataEngine.Schema;
using OpenDataEngine.Model.Relation;
using OpenDataEngine.Query;
using OpenDataEngine.Source;

namespace Example.Model
{
    public class Book : Queryable<Book>
    {
        public String Title { get; set; }
        public String Publisher { get; set; }
        public DateTime PublishedAt { get; set; }

        [HasOne]
        public Relation Author { get; set; }
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
