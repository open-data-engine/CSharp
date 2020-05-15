using Example.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
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

            Init<Book>();

            try
            {
                IAsyncQueryable<Book> books = Book.Select(b => new { b.Title, b.PublishedAt }).Where(b => String.Equals(b.Title, title));

                await foreach (Book book in books)
                {
                    Console.WriteLine(book.Title);
                }
            }
            catch(Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }


        public static void Init<TModel>() where TModel : new()
        {
            Query<TModel>.Translator = query =>
            {
                IEnumerable<String> selection = new String[0];

                if (query.Expression is Expression<Func<TModel, dynamic>> selectExpression)
                {
                    selection = (selectExpression.Body as NewExpression)?.Arguments.Select(a => (a as MemberExpression)?.Member.Name).Where(a => a != null);
                }



                return "";
            };
            Query<TModel>.Executor = (type, queryString, arguments) =>
            {
                return new[] { new TModel(), new TModel() }.ToAsyncEnumerable();
            };
        }
    }
}
