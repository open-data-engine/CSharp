using Example.Model;
using OpenDataEngine;
using OpenDataEngine.Query.Clause;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Example
{
    class Program
    {
        static async void Main(string[] args)
        {
            Book book1 = Book
                .Select(Book.Title)
                .Where("some filter");

            List<Book> book2 = Query<Book>
                .Select("kaas")
                .Where("some filter")
                .Limit(10);

            Book book3 = await new Query<Book>()
                .Select("kaas")
                .Where("some filter")
                .Find();

            List<Book> book4 = await new Query<Book> { new Select("kaas").Is("@title"), new Where("some filter"), new Limit(10) }
                .FindAll()
                .ToListAsync();

            Console.WriteLine("Hello World!");
        }
    }
}
