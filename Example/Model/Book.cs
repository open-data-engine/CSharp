using OpenDataEngine;
using OpenDataEngine.Adapter;
using OpenDataEngine.Connection;
using OpenDataEngine.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace Example.Model
{
    public class Book: OpenDataEngine.Model
    {
        public String Title;

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
