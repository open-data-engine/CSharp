﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using OpenDataEngine.Query;
using OpenDataEngine.Query.Clause;

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

    public class Sql<TModel>: Base<TModel>
    {
        private static readonly Dictionary<Clause, Clause[]> Dependencies = new Dictionary<Clause, Clause[]>
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

        private UInt64 _constCount;
        private readonly List<(String, Object)> _arguments = new List<(String, Object)>();
        private readonly Dictionary<Clause, List<String>> _clauses = new Dictionary<Clause, List<String>>();
        
        public override(String Command, (String, Object)[] Arguments) Translate(IAsyncQueryable query)
        {
            Visit(query.Expression);

            String database = "FYN_1005_General";
            String table = "Customer";

            if (_clauses.ContainsKey(Clause.Select) == false || (_clauses[Clause.Select]?.Count() ?? 0) == 0)
            {
                _clauses.Add(Clause.Select, new List<String>{ $"SELECT `{database}`.`{table}`.*" });
            }

            if (_clauses.ContainsKey(Clause.From) == false || (_clauses[Clause.Select]?.Count() ?? 0) == 0)
            {
                _clauses.Add(Clause.From, new List<String>{ $"FROM `{database}`.`{table}`" });
            }

            return (String.Join(' ', _clauses.TopoLogicalSort(Dependencies).Values.SelectMany(v => v)), _arguments.ToArray());
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            Query.Clause.Base expression = (Query.Clause.Base)base.VisitMethodCall(node);

            Clause clause;
            String sql;

            switch (expression)
            {
                case Select selectExpression:
                    clause = Clause.Select;
                    sql = $"SELECT {String.Join(", ", selectExpression.Fields.Select(Recurse))}";

                    break;

                case Where whereExpression:
                    clause = Clause.Where;
                    sql = $"{(_clauses.ContainsKey(Clause.Where) ? "AND" : "WHERE")} {Recurse(whereExpression.Body)}";

                    break;

                default:
                    throw new Exception($"Unhandeled clause '{expression.GetType()}'");
            }


            if (_clauses.ContainsKey(clause) == false)
            {
                _clauses.Add(clause, new List<String>());
            }

            _clauses[clause].Add(sql);

            return node;
        }

        public async override IAsyncEnumerable<TResult> From<TResult>(IAsyncEnumerable<IDictionary<String, dynamic>> source, [EnumeratorCancellation] CancellationToken token)
        {
            Type type = typeof(TResult);

            await foreach (IDictionary<String, dynamic> record in source.WithCancellation(token).ConfigureAwait(false))
            {
                TResult result = Activator.CreateInstance<TResult>();

                foreach ((String key, dynamic value) in record)
                {
                    if (type.GetProperty(key) != null)
                    {
                        type.GetProperty(key)?.SetValue(result, value, null);
                    }
                }

                yield return result;
            }
        }

        private String Recurse(Expression expression)
        {
            switch (expression)
            {
                case UnaryExpression e:
                    {
                        String right = Recurse(e.Operand);

                        return $"({e.NodeType.ToSql()} {right})";
                    }

                case BinaryExpression e:
                    {
                        String right = Recurse(e.Right);

                        return $"({Recurse(e.Left)} {e.NodeType.ToSql(right == "NULL")} {right})";
                    }

                case ConstantExpression e:
                    {
                        String constName = $"CONST_{_constCount++.Base52Encode()}";
                        _arguments.Add((constName, e.Value));

                        return $"@{constName}";
                    }

                case MemberExpression e:
                    {
                        if (!(e.Member is PropertyInfo) && !(e.Member is FieldInfo))
                        {
                            throw new Exception("Unhandeled member access, member is neither a property nor a field");
                        }

                        if (e.Member.DeclaringType == typeof(TModel))
                        {
                            return $"`{Source.Schema.ResolveProperty(e.Member.Name)}`";
                        }

                        _arguments.Add((e.Member.Name, e.GetValue()));
                        return $"@{e.Member.Name}";
                    }

                case MethodCallExpression e:
                    {
                        switch (e.Method.Name)
                        {
                            case "Contains":
                                {
                                    return $"(__prop__ IN(__props__))";
                                }

                            default:
                                throw new Exception($"'{e.Method.Name}' method calls in Where clause not supported at this point in time");
                        }

                    }

                default:
                    throw new Exception("Unhandeled expression in Where clause");
            }
        }
    }

    public static class SqlExtensions
    {
        private const SByte True = 1;
        private const SByte False = 0;

        // TODO(Chris Kruining) I have a sneeking suspision that this is VERY error prone, make sure it isn't or remove it
        private static T CastTo<T>(this Object subject) => (T)subject;

        public static Object GetValue(this Expression expression)
        {
            var member = Expression.Convert(expression, typeof(Object));
            var lambda = Expression.Lambda<Func<Object>>(member);
            var getter = lambda.Compile();

            return getter();
        }

        public static String ToSql(this MemberExpression expression) => $"`$database`.`$table`.`${expression.Member.Name}`";
        public static String ToSql(this ConstantExpression expression) => expression.Value.ToSql(expression.Type);

        public static String ToSql(this ExpressionType expressionType, Boolean rightIsNull = false) => expressionType switch
        {
            ExpressionType.Add => "+",
            ExpressionType.Subtract => "-",
            ExpressionType.Multiply => "*",
            ExpressionType.Divide => "/",
            ExpressionType.Modulo => "%",
            ExpressionType.AndAlso => "AND",
            ExpressionType.OrElse => "OR",
            ExpressionType.And => "&",
            ExpressionType.Or => "|",
            ExpressionType.ExclusiveOr => "^",
            ExpressionType.Equal => rightIsNull ? "IS" : "=",
            ExpressionType.NotEqual => "!=",
            ExpressionType.Not => "NOT",
            ExpressionType.LessThan => "<",
            ExpressionType.LessThanOrEqual => "<=",
            ExpressionType.GreaterThan => ">",
            ExpressionType.GreaterThanOrEqual => ">=",
            _ => throw new Exception("Unhandeled unary node type"),
        };

        public static String ToSql(this Object subject, Type type)
        {
            subject = typeof(SqlExtensions).GetMethod(nameof(CastTo))?.MakeGenericMethod(type)?.Invoke(null, new[] { subject }) ?? "";

            return subject switch
            {
                Int16 value => value.ToSql(),
                Int32 value => value.ToSql(),
                Int64 value => value.ToSql(),
                UInt16 value => value.ToSql(),
                UInt32 value => value.ToSql(),
                UInt64 value => value.ToSql(),
                Decimal value => value.ToSql(),
                String value => value.ToSql(),
                Boolean value => value.ToSql().ToString(),
                DateTime value => value.ToSql(),
                TimeSpan value => value.ToSql(),
                _ => subject.ToString() ?? "",
            };
        }

        // Simple types
        public static String ToSql(this String subject) => $"'{subject ?? ""}'";
        public static SByte ToSql(this Boolean subject) => subject ? True : False;

        // Numbers
        public static String ToSql(this Int16 subject) => subject.ToString(CultureInfo.InvariantCulture);
        public static String ToSql(this Int32 subject) => subject.ToString(CultureInfo.InvariantCulture);
        public static String ToSql(this Int64 subject) => subject.ToString(CultureInfo.InvariantCulture);
        public static String ToSql(this UInt16 subject) => subject.ToString(CultureInfo.InvariantCulture);
        public static String ToSql(this UInt32 subject) => subject.ToString(CultureInfo.InvariantCulture);
        public static String ToSql(this UInt64 subject) => subject.ToString(CultureInfo.InvariantCulture);

        // Decimals
        public static String ToSql(this Decimal subject) => subject.ToString("#0." + new String('0', subject.DecimalPlaces()), CultureInfo.InvariantCulture);
        
        // Dates and times
        public static String ToSql(this DateTime input, DateTime? minDate = null)
        {
            minDate ??= DateTime.MinValue;

            //2012-08-31 13:35:38
            return input.Equals(minDate) ? "0000-00-00 00:00:00" : input.ToString("yyyy-MM-dd HH:mm:ss");
        }
        public static String ToSqlDate(this DateTime input, DateTime? minDate = null)
        {
            minDate ??= DateTime.MinValue;

            //2012-08-31
            return input.Equals(minDate) ? "0000-00-00" : input.ToString("yyyy-MM-dd");
        }
        public static String ToSql(this TimeSpan input, TimeSpan? minTime = null)
        {
            minTime ??= TimeSpan.Zero;

            //13:35:38
            return input == minTime ? "00:00:00" : input.ToString(@"hh\:mm\:ss");
        }
        public static String ToSqlTime(this DateTime input, DateTime? minTime = null)
        {
            minTime ??= DateTime.MinValue;

            //13:35:38
            return input == minTime ? "00:00:00" : input.ToString(@"HH\:mm\:ss");
        }
        public static DateTime FromSqlDateTime(this String subject) => String.IsNullOrWhiteSpace(subject) ? DateTime.MinValue : DateTime.ParseExact(subject, "yyyy-MM-dd HH:mm:ss", null, DateTimeStyles.None);
        public static DateTime FromSqlDate(this String input) => String.IsNullOrWhiteSpace(input) ? DateTime.MinValue : DateTime.ParseExact(input, "yyyy-MM-dd", null, DateTimeStyles.None);
        public static TimeSpan FromSqlTimeToTimeSpan(this String input) => String.IsNullOrWhiteSpace(input) ? TimeSpan.Zero : TimeSpan.ParseExact(input, @"hh\:mm\:ss", CultureInfo.InvariantCulture);
        public static DateTime FromSqlTime(this String input) => String.IsNullOrWhiteSpace(input) ? DateTime.MinValue : DateTime.ParseExact(input, "HH:mm:ss", null, DateTimeStyles.None);

        // nullable type methods
        public static String ToSql(this Int16? subject) => subject?.ToSql() ?? "0";
        public static String ToSql(this Int32? subject) => subject?.ToSql() ?? "0";
        public static String ToSql(this Int64? subject) => subject?.ToSql() ?? "0";
        public static String ToSql(this UInt16? subject) => subject?.ToSql() ?? "0";
        public static String ToSql(this UInt32? subject) => subject?.ToSql() ?? "0";
        public static String ToSql(this UInt64? subject) => subject?.ToSql() ?? "0";
        public static String ToSql(this Decimal? subject) => subject?.ToSql() ?? "0";
        public static SByte ToSql(this Boolean? subject) => subject?.ToSql() ?? False;


    }
}