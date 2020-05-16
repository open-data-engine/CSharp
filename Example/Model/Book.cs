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

    public class Cache<TModel> : Source<TModel>
    {
        public Cache() : base(null, null, null)
        {
        }
    }

    public class Database<TModel> : Source<TModel>
    {
        public Database(String host, String user, String pass) : base(
            new Mysql(host, user, pass),
            new Sql(),
            new Table()
        ) {}
    }

    public class TypicodeAPI<TModel> : Source<TModel>
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
        ) {}
    }

    public class Strategy<TModel>
    {

    }

    public class StraightForward<TModel> : Strategy<TModel>
    {
        public StraightForward(Source<TModel> source)
        {

        }
    }

    public class StaleWhileRevalidate<TModel> : Strategy<TModel>
    {
        public StaleWhileRevalidate(Source<TModel> cache, Source<TModel> fallback)
        {

        }
    }

    public class CacheFirst<TModel> : Strategy<TModel>
    {
        public CacheFirst(Source<TModel> cache, Source<TModel> fallback)
        {

        }
    }
}
