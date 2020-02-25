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
        private String title;

        public static (String, Source)[] Sources = new (String, Source)[]
        {
            ("default", new Source(
                new Http("https://my-json-server.typicode.com/open-data-engine/CSharp", new
                {
                    Headers = new (String, String)[]
                    {
                        ("Accept", "application/json"),
                        ("Content-Type", "application/json"),
                        ("Authorization", "Bearer {token}"),
                    },
                }),
                new Json(),
                new Rest("book", new 
                {
                    title = "Title",
                })
            )),
        };
    }
}
