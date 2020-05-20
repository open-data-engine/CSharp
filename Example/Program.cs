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
using OpenDataEngine.Schema;
using OpenDataEngine.Source;
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
            UInt32 companyId = 1005;
            UInt32 locationId = 1;

            String host = Env.GetString("DB_HOST");
            String user = Env.GetString("DB_USER");
            String pass = Env.GetString("DB_PASS");

            Table.DatabaseFormatter = s => s switch
            {
                "General" => $"FYN_{companyId}_General",
                "Location" => $"FYN_{companyId}_Location{locationId}",
                "Branche" => $"FYN_[BRANCHE]",
                _ => $"FYN_{s}",
            };

            Database<Relation> database = new Database<Relation>(host, user, pass, new { ID = "Customer_ID", FirstName = "First_Name", MiddleName = "Middle_Name", SurName = "Sur_Name" }, "General", "Customer");
            Cache<Relation> cache = new Cache<Relation>();
            CacheFirst<Relation> strategy = new CacheFirst<Relation>(cache, database);

            try
            {
                IAsyncQueryable<Relation> relations = Relation.Select(b => new { b.ID, b.Username }).From(strategy).Where(b => b.Username != "" && (b.ID > 100 || b.Username == username));
                
                await foreach (Relation relation in relations)
                {
                    Console.WriteLine($"Relation({relation.ID}) is named '{relation.Username}'");
                }

                Relation rel = await Relation.From(database).Where(r => (r.Username ?? r.FirstName ?? r.MiddleName ?? r.SurName) == username);
                Console.WriteLine($"Single Relation({rel.ID}) is named '{rel.Username}'");
            }
            catch(Exception exception)
            {
                Console.WriteLine("Exeption :: " + exception.Message);
            }
        }
    }
}
