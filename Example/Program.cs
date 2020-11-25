using Example.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNetEnv;
using Example.Model.Relation;
using Microsoft.Extensions.Logging;
using OpenDataEngine.Adapter;
using OpenDataEngine.Query;
using OpenDataEngine.Schema;
using OpenDataEngine.Source;

namespace Example
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Entered Program.Main");

            Setup();

            await TestRelations();
            // await TestPerformance();

            Console.ReadLine();
        }

        private static void Setup()
        {
            // Load .env file
            Env.Load();

            const UInt32 companyId = 1005;
            const UInt32 locationId = 1;

            Table.DatabaseFormatter = s => s switch
            {
                "General" => $"FYN_{companyId}_General",
                "Location" => $"FYN_{companyId}_Location{locationId}",
                _ => $"FYN_{s}",
            };

            ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Debug);
            });
            ILogger<ISource> logger = loggerFactory.CreateLogger<ISource>();

            Relation.Sources["default"].Logger = logger;
        }

        private static async ValueTask TestRelations()
        {
            Book book = new Book
            {
                Author = new Relation
                {
                    FirstName = "Chris Kruining",
                },
                Title = "API's for dummies",
                Publisher = "FYN Software",
                PublishedAt = DateTime.Now,
            };
            Relation[]? relationWithRelationalFilters = await Relation
                .Where(
                    r => r.Status == Status.Active 
                        && Sql.Raw<Boolean>("`FYN_1005_General`.`Customer`.`First_Name` != ''")
                        && r.Extras.Any(b => b.Type == Extra.ExtraType.AzureActiveDirectory)
                        // && Sql.Raw<Boolean>("`FYN_1005_General`.`Customer`.`ID` IN(SELECT R_Extras.`RelationID` FROM `FYN_1005_General`.`Relation_Extra` as R_Extras Where R_Extras.`Type` = 'AzureActiveDirectory')")
                )
                .OrderBy(r => r.FirstName)
                .ThenByDescending(r => r.SurName)
                .ToArrayAsync();
        }

        private static async ValueTask TestPerformance()
        {
            const String username = "Chris";

            String host = Env.GetString("DB_HOST");
            String user = Env.GetString("DB_USER");
            String pass = Env.GetString("DB_PASS");

            Database database = new Database(host, user, pass, new { ID = "Customer_ID", FirstName = "First_Name", MiddleName = "Middle_Name", SurName = "Sur_Name" }, "General", "Customer");

            await Performance.MeasureAndSummarize(
                1000,
                ("O.D.E. multi", async () =>
                {
                    List<Relation> relations = new List<Relation>();
                    IAsyncQueryable<Relation> query = Relation
                        .Select(r => new { r.ID, r.FirstName, r.MiddleName, r.SurName })
                        .Where(r => r.Username != "" && r.Status == Status.Active && (r.ID > 100 || r.Username == username));

                    await foreach (Relation relation in query)
                    {
                        relations.Add(relation);
                    }
                }),
                ("Manual multi", async () =>
                {
                    String sql = "SELECT `FYN_1005_General`.`Customer`.`Customer_ID` AS 'ID', `FYN_1005_General`.`Customer`.`First_Name` AS 'FirstName', `FYN_1005_General`.`Customer`.`Middle_Name` AS 'MiddleName', `FYN_1005_General`.`Customer`.`Sur_Name` AS 'SurName' FROM `FYN_1005_General`.`Customer` WHERE `FYN_1005_General`.`Customer`.`Username` != @CONST_1 AND (`FYN_1005_General`.`Customer`.`Customer_ID` > @CONST_0 OR `FYN_1005_General`.`Customer`.`Username` = @username)";
                    (String, Object)[] arguments = { ("username", username), ("CONST_0", 100), ("CONST_1", "") };

                    IEnumerable<Relation> relations = await database.Connection
                        .Execute(sql, arguments, CancellationToken.None)
                        .Select(record => new Relation
                        {
                            ID = record["ID"],
                            FirstName = record["FirstName"],
                            MiddleName = record["MiddleName"],
                            SurName = record["SurName"],
                        })
                        .ToListAsync();
                }),
                ("O.D.E. single", async () =>
                {
                    Relation relation = await Relation
                        .Select(r => new { r.ID, r.FirstName, r.MiddleName, r.SurName })
                        .Where(r => r.Username != "" && r.Status == Status.Active && (r.ID > 100 || r.Username == username));
                }),
                ("Manual single", async () =>
                {
                    String sql = "SELECT `FYN_1005_General`.`Customer`.`Customer_ID` AS 'ID', `FYN_1005_General`.`Customer`.`First_Name` AS 'FirstName', `FYN_1005_General`.`Customer`.`Middle_Name` AS 'MiddleName', `FYN_1005_General`.`Customer`.`Sur_Name` AS 'SurName' FROM `FYN_1005_General`.`Customer` WHERE `FYN_1005_General`.`Customer`.`Username` != @CONST_1 AND (`FYN_1005_General`.`Customer`.`Customer_ID` > @CONST_0 OR `FYN_1005_General`.`Customer`.`Username` = @username) LIMIT 1";
                    (String, Object)[] arguments = { ("username", username), ("CONST_0", 100), ("CONST_1", "") };

                    Relation relation = await database.Connection
                        .Execute(sql, arguments, CancellationToken.None)
                        .Select(record => new Relation
                        {
                            ID = record["ID"],
                            FirstName = record["FirstName"],
                            MiddleName = record["MiddleName"],
                            SurName = record["SurName"],
                        }).SingleAsync();
                })
            );
        }
    }

    public static class Performance
    {
        public static async ValueTask MeasureAndSummarize(UInt32 iterations, params (String Topic, Func<Task> Action)[] topics)
        {
            (String, Decimal[])[] measurements = await topics.ToAsyncEnumerable().SelectAwait(async t => (t.Topic, await Measure(iterations, t.Action))).ToArrayAsync();

            Summarize(iterations, measurements);
        }

        public static async ValueTask<Decimal[]> Measure(UInt32 iterations, Func<Task> action)
        {
            Decimal[] measurements = new Decimal[iterations];
            Stopwatch stopwatch = new Stopwatch();

            // Warmup
            await action.Invoke();

            for (UInt32 i = 0; i < iterations; i++)
            {
                Console.Write($"\r{i} / {iterations}");

                stopwatch.Restart();

                await action.Invoke();

                measurements[i] = stopwatch.ElapsedMilliseconds;
            }

            Console.WriteLine($"\rDone\t\t");

            return measurements;
        }

        public static void Summarize(UInt32 length, params (String Topic, Decimal[] Measurments)[] topics)
        {
            List<Decimal> sums = topics.Select(t => t.Measurments.Sum()).ToList();
            Decimal totalSum = sums.Sum();
            String summedRatios = String.Join(":", sums.Select(m => $"{(m / totalSum * 100):##}"));

            List<Decimal> averages = topics.Select(t => t.Measurments.Average()).ToList();
            Decimal totalAverages = averages.Sum();
            String averagedRatios = String.Join(":", averages.Select(m => $"{(m / totalAverages * 100):##}"));

            Console.WriteLine($"\nRan {length} iterations\n{String.Join(" \t", topics.Select(t => t.Topic))}\tRatios");
            Console.WriteLine($"{String.Join(" \t", sums.Select(s => $"{s:00.00}ms"))} \t{summedRatios:00.00} \tSummed");
            Console.WriteLine($"{String.Join(" \t", averages.Select(s => $"{s:00.00}ms"))} \t{averagedRatios:00.00} \tAverage");
        }
    }
}
