using OpenDataEngine;
using OpenDataEngine.Adapter;
using OpenDataEngine.Connection;
using OpenDataEngine.Schema;
using System;

namespace Example.Model
{
    public class Book: Model<Book>
    {
        public static readonly Field<String> Title = new Field<String>();
        public String title
        {
            get => Get(Title);
            set => Set(Title, value);
        }

        public static readonly dynamic Fields = new
        {
            Title = Field<String>.withDefault("Kaas is awesome"),
        };

        public static readonly dynamic Sources = new
        {
            Default = new TypicodeAPI(
                "https://my-json-server.typicode.com/open-data-engine/CSharp", 
                "book", 
                new 
                {
                    title = Fields.Title,
                }
            ),
            Cache = new Cache(),
        };

        public static readonly dynamic Strategies = new
        {
            Default = new CacheFirst(
                cache: Sources.Cache, 
                fallback: Sources.Default
            ),
            Unstable = new StaleWhileRevalidate(
                cache: Sources.Cache, 
                fallback: Sources.Default
            ),
            Live = new StraightForward(Sources.Default),
        };
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
