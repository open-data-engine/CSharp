using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using OpenDataEngine.Query;

namespace OpenDataEngine.Adapter
{
    public enum Clause
    {
        Select,
        Insert,
        Update,
        Delete,
        From,
        Values,
        Set,
        UpdateOnDuplicate,
        Join,
        Where,
        GroupBy,
        Having,
        Order,
        Limit,
    }

    public class Sql: Base
    {
        private readonly Dictionary<Clause, String> _clauses = new Dictionary<Clause, String>();
        private static readonly Dictionary<Clause, Clause[]> dependencies = new Dictionary<Clause, Clause[]>
        {
            { Clause.Select, new Clause[]{} },
            { Clause.Insert, new Clause[]{} },
            { Clause.Update, new Clause[]{} },
            { Clause.Delete, new Clause[]{} },
            { Clause.From, new []{ Clause.Select } },
            { Clause.Values, new []{ Clause.Insert } },
            { Clause.Set, new []{ Clause.Update } },
            { Clause.UpdateOnDuplicate, new []{ Clause.Insert, Clause.Values } },
            { Clause.Join, new []{ Clause.Select, Clause.From } },
            { Clause.Where, new []{ Clause.Select, Clause.From, Clause.Join, Clause.Delete } },
            { Clause.GroupBy, new []{ Clause.Select, Clause.From, Clause.Join, Clause.Where } },
            { Clause.Having, new []{ Clause.Select, Clause.From, Clause.Join, Clause.Where, Clause.GroupBy } },
            { Clause.Order, new []{ Clause.Select, Clause.From, Clause.Join, Clause.Where, Clause.GroupBy, Clause.Having } },
            { Clause.Limit, new []{ Clause.Select, Clause.From, Clause.Join, Clause.Where, Clause.Order, Clause.GroupBy, Clause.Having } },
        };

        public override(String Command, Object[] Arguments) Translate(IAsyncQueryable query)
        {
            Visit(query.Expression);

            return (String.Join(' ', _clauses.TopoLogicalSort(dependencies).Values), new Object[0]);
        }

        protected override void Emit(String command, dynamic? arguments)
        {
            Clause clause = Enum.Parse<Clause>(command);
            String args = clause switch
            {
                Clause.Select => String.Join(", ", (arguments as IEnumerable<String>).Select(a => $"`{a}`")),
                _ => "ARGS",
            };

            _clauses?.Add(clause, $"{command.ToUpperInvariant()} {args}");
        }
    }
}