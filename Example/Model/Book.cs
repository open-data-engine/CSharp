using OpenDataEngine;
using OpenDataEngine.Adapter;
using OpenDataEngine.Connection;
using OpenDataEngine.Schema;
using System;
using OpenDataEngine.Query;

namespace Example.Model
{
    public struct Book2
    {
        public String Title { get; set; }
        public String Author { get; set; }
        public String Publisher { get; set; }
        public DateTime PublishedAt { get; set; }
    }

    public class Book: Queryable<Book>
    {
        public String Title { get; set; }
        public String Author { get; set; }
        public String Publisher { get; set; }
        public DateTime PublishedAt { get; set; }
    }

    public class Cache : Source
    {
        public Cache() : base(null, null, null)
        {
        }
    }

    public class TypicodeAPI : Source
    {
        public TypicodeAPI(String url, String resource, dynamic mapping) : base(
            new Http(url, new
            {
                Headers = new[]
                {
                    ("Accept", "application/json"),
                    ("Content-Type", "application/json"),
                    ("Authorization", "Bearer {token}"),
                },
            }),
            new Json(),
            new Rest(resource, mapping)
        )
        {
        }
    }

    public class Strategy
    {

    }

    public class StraightForward : Strategy
    {
        public StraightForward(Source source)
        {

        }
    }

    public class StaleWhileRevalidate : Strategy
    {
        public StaleWhileRevalidate(Source cache, Source fallback)
        {

        }
    }

    public class CacheFirst : Strategy
    {
        public CacheFirst(Source cache, Source fallback)
        {

        }
    }
}
