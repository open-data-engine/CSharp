using Example.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DotNetEnv;
using OpenDataEngine;
using OpenDataEngine.Connection;
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
            Stopwatch stopwatch = Stopwatch.StartNew();

            // Load .env file
            Env.Load();

            String username = "Chris";
            UInt32 companyId = 1005;
            UInt32 locationId = 1;

            String host = Env.GetString("DB_HOST");
            String user = Env.GetString("DB_USER");
            String pass = Env.GetString("DB_PASS");

            Console.WriteLine($"app took {stopwatch.ElapsedMilliseconds}ms");
            stopwatch.Reset();
            stopwatch.Start();

            Table.DatabaseFormatter = s => s switch
            {
                "General" => $"FYN_{companyId}_General",
                "Location" => $"FYN_{companyId}_Location{locationId}",
                "Branche" => $"FYN_[BRANCHE]",
                _ => $"FYN_{s}",
            };

            Database database = new Database(host, user, pass, new { ID = "Customer_ID", FirstName = "First_Name", MiddleName = "Middle_Name", SurName = "Sur_Name" }, "General", "Customer");
            Cache cache = new Cache();
            CacheFirst strategy = new CacheFirst(cache, database);

            Console.WriteLine($"app took {stopwatch.ElapsedMilliseconds}ms");
            stopwatch.Reset();
            stopwatch.Start();

            List<Relation> relations = new List<Relation>();
            try
            {
                await foreach (Relation relation in Relation.Select(b => new { b.ID, b.Username }).From(strategy).Where(b => b.Username != "" && (b.ID > 100 || b.Username == username)))
                {
                    relations.Add(relation);
                }
            }
            catch(Exception exception)
            {
                Console.WriteLine("Exeption :: " + exception.Message);
            }

            Console.WriteLine($"1st query took {stopwatch.ElapsedMilliseconds}ms");
            Console.WriteLine(String.Join("\n", relations.Select(relation => $"Relation({relation.ID}) is named '{relation.Username}'")));

            stopwatch.Reset();
            stopwatch.Start();
            
            Relation rel = null;
            try
            {
                rel = await Relation.Where(r => (r.Username ?? r.FirstName ?? r.MiddleName ?? r.SurName) == username);
            }
            catch(Exception exception)
            {
                Console.WriteLine("Exeption :: " + exception.Message);
            }

            Console.WriteLine($"2nd query took {stopwatch.ElapsedMilliseconds}ms");
            Console.WriteLine($"Single Relation({rel.ID}) is named '{rel.Username}'");

            stopwatch.Reset();
            stopwatch.Start();

            List<Relation> rels = new List<Relation>();
            try
            {
                IConnection connection = new Mysql(host, user, pass);
                String sql = @"
                    SELECT 
                        `FYN_1005_General`.`Customer`.`Customer_ID` AS 'ID', 
                        `FYN_1005_General`.`Customer`.`Username` 

                    FROM `FYN_1005_General`.`Customer` 

                    WHERE `FYN_1005_General`.`Customer`.`Username` != @CONST_1 
                    AND (
                        `FYN_1005_General`.`Customer`.`Customer_ID` > @CONST_0 
                        OR `FYN_1005_General`.`Customer`.`Username` = @username
                    )";
                
                await foreach (IDictionary<String, dynamic> record in connection.Execute(sql, new (String, Object)[] { ("username", username), ("CONST_0", 100), ("CONST_1", "") }, CancellationToken.None))
                {
                    rels.Add(new Relation
                    {
                        ID = record["ID"],
                        Username = record["Username"],
                    });
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("Exeption :: " + exception.Message);
            }

            Console.WriteLine($"3rd query took {stopwatch.ElapsedMilliseconds}ms");
            Console.WriteLine(String.Join("\n", rels.Select(relation => $"Relation({relation.ID}) is named '{relation.Username}'")));

            stopwatch.Reset();
            stopwatch.Start();
        }
    }
}
