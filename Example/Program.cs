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
        static void Main(string[] args)
        {
            Run();
        }

        static async void Run()
        {
            String title = "My first book";

            /* =============================================
             * Style 1
             * =============================================
             *
             * The simple and short method, no other noice 
             * beside the code that counts
             * 
             */
            Console.WriteLine("Style 1");
            
            Book book1 = Book
                .Select(Book.Title, Book.Title)
                .Where(Book.Title.Is("@title", title));

            Console.WriteLine(book1.title);

            List<Book> books1 = Query<Book>
                .Select(Book.Title)
                .Where(Book.Title.Is("@title", title))
                .Limit(10);

            Console.WriteLine(books1.Select(b => b.Get(Book.Title)).ToString(", "));



            /* =============================================
             * Style 2
             * =============================================
             *
             * A little more verbose for those who prefer 
             * it that way
             * 
             */
            Console.WriteLine("Style 2");

            Book book2 = await new Query<Book>()
                .Select(new Select(Book.Title))
                .Where(new Where(Book.Title).Is("@title"))
                .Find(new { title });

            Console.WriteLine(book2.Get(Book.Title));

            List<Book> books2 = await new Query<Book> { new Select(Book.Title), new Where(Book.Title).Is("@title"), new Limit(10) }
                .FindAll(new { title })
                .ToListAsync();

            Console.WriteLine(books2.Select(b => b.Get(Book.Title)).ToString(", "));



            /* =============================================
             * Style 3
             * =============================================
             *
             * If you prefer a more manual style without 
             * all the hidden magic
             * 
             */
            Console.WriteLine("Style 3");

            Query<Book> book3Query = new Query<Book>();
            book3Query += new Select(Book.Title);
            book3Query += new Where(Book.Title).Is("@title");
            book3Query += new Limit(10);
            Book book3 = await book3Query.Find(new { title });

            Console.WriteLine(book3.Get(Book.Title));

            Query<Book> books3Query = new Query<Book>();
            books3Query.Add(new Select(Book.Title));
            books3Query.Add(new Where(Book.Title).Is("@title"));
            books3Query.Add(new Limit(10));
            List<Book> books3 = await books3Query.FindAll(new { title }).ToListAsync();

            Console.WriteLine(books3.Select(b => b.Get(Book.Title)).ToString(", "));
        }
    }

    public static class Extensions
    {
        public static String ToString(this IEnumerable<String> subject, String delimeter) => String.Join(delimeter, subject.ToArray());
    }
}
