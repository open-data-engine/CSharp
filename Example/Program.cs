using Example.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DotNetEnv;
using OpenDataEngine;
using OpenDataEngine.Query;
using OpenDataEngine.Strategy;

namespace Example
{
    public static class TemporarySource
    {
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            // Load .env file
            Env.Load();

            String username = "Chris";

            String host = Env.GetString("DB_HOST");
            String user = Env.GetString("DB_USER");
            String pass = Env.GetString("DB_PASS");

            Database<Relation> database = new Database<Relation>(host, user, pass, new { ID = "Relation_ID" });
            Cache<Relation> cache = new Cache<Relation>();
            CacheFirst<Relation> strategy = new CacheFirst<Relation>(cache, database);

            try
            {
                IAsyncQueryable<Relation> relations = Relation.From(strategy).Where(b => b.Username != "");
                // IAsyncQueryable<Relation> relations = Relation.Select(b => new { b.ID, b.Username }).From(strategy).Where(b => b.Username != "");
                // IAsyncQueryable<Relation> relations = Relation.Select(b => new { b.ID, b.Username, Number = 10, Str = "Kaas", Bool = false }).From(strategy).Where(b => b.Username != "");

                await foreach (Relation relation in relations)
                {
                    Console.WriteLine($"Relation({relation.ID}) is named '{relation.Username}'");
                    Thread.Sleep(500);
                }
            }
            catch(Exception exception)
            {
                Console.WriteLine("Exeption :: " + exception.Message);
            }
        }
    }
}
