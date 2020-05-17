using Example.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using OpenDataEngine;
using OpenDataEngine.Query;

namespace Example
{
    public static class TemporarySource
    {
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            String title = "My first book";
            
            try
            {
                IAsyncQueryable<Book> books = Book.Select(b => new { b.Title, b.PublishedAt }).From(new Database<Book>("192.168.198.8", "Chris", "rotschool")).Where(b => b.Title == title);

                await foreach (Book book in books)
                {
                    Console.WriteLine(book.Title);
                }
            }
            catch(Exception exception)
            {
                Console.WriteLine("Exeption :: " + exception.Message);
            }
        }
    }
}
