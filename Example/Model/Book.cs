using OpenDataEngine;
using OpenDataEngine.Adapter;
using OpenDataEngine.Connection;
using OpenDataEngine.Schema;
using System;
using System.Collections.Generic;
using OpenDataEngine.Query;

namespace Example.Model
{
    public class Book : Queryable<Book>
    {
        public String Title { get; set; }
        public String Author { get; set; }
        public String Publisher { get; set; }
        public DateTime PublishedAt { get; set; }
    }

    public class TypicodeAPI<TModel> : Source<TModel>
    {
        public TypicodeAPI(String url, String resource, dynamic mapping) : base(new Http(url, new { Headers = new[] { ("Accept", "application/json"), ("Content-Type", "application/json"), ("Authorization", "Bearer {token}"), }, }), new Json<TModel>(), new Rest<TModel>(resource, mapping)) {}
    }
}
