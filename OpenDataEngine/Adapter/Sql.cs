using System;
using System.Collections.Generic;
using System.Diagnostics;
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

    public class Sql: Base
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
            _clauses.Clear();
            _arguments.Clear();

            Visit(query.Expression);

            if (_clauses.ContainsKey(Clause.Select) == false || (_clauses[Clause.Select]?.Count() ?? 0) == 0)
            {
                _clauses.Add(Clause.Select, new List<String>{ $"SELECT {Source.Schema.ResolvePath("")}.*" });
            }

            if (_clauses.ContainsKey(Clause.From) == false || (_clauses[Clause.Select]?.Count() ?? 0) == 0)
            {
                _clauses.Add(Clause.From, new List<String>{ $"FROM {Source.Schema.ResolvePath("")}" });
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
                    sql = $"SELECT {String.Join(", ", selectExpression.Fields.Select(f => new SelectVisitor(this, _arguments, ref _constCount).Recurse(selectExpression.ModelType, f)))}";

                    break;

                case Where whereExpression:
                {
                    clause = Clause.Where;
                    using var visitor = new WhereVisitor(this, _arguments, ref _constCount);
                    sql = $"{(_clauses.ContainsKey(Clause.Where) ? "AND" : "WHERE")} {visitor.Recurse(whereExpression.ModelType, whereExpression.Body)}";

                    break;
                }

                // TODO(Chris Kruining) due to the nature of linq the limit can be split up into multiple clauses, find a way to combine these
                case Limit limitExpression:
                {
                    clause = Clause.Limit;
                    sql = limitExpression.Offset != null 
                        ? $"LIMIT {limitExpression.Offset}, {limitExpression.Amount}" 
                        : $"LIMIT {limitExpression.Amount}";

                    break;
                }

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
                    try
                    {
                        PropertyInfo? property = type.GetProperty(Source.Schema.ReverseResolveProperty(key));
                        Object? v = value;

                        if (property?.PropertyType.IsEnum ?? false)
                        {
                            v = Enum.Parse(property.PropertyType, (String)v, true);
                        }

                        if (v is DBNull)
                        {
                            v = null;
                        }

                        property?.SetValue(result, v, null);
                    }
                    catch (Exception exception)
                    {
                        throw new Exception($"failed to map field '{key}' to property, reason: '{exception.Message}'", exception);
                    }
                }

                yield return result;
            }
        }
    }

    public class SelectVisitor: IDisposable
    {
        private readonly Sql _owner;
        private readonly List<(String, Object)> _arguments;
        private UInt64 _constCount;

        public SelectVisitor(Sql owner, List<(String, Object)> arguments, ref UInt64 constCount)
        {
            _owner = owner;
            _arguments = arguments;
            _constCount = constCount;
        }

        public void Dispose()
        {

        }

        public String Recurse(Type modelType, Expression expression)
        {
            switch (expression)
            {
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

                    if (e.Member.DeclaringType == modelType)
                    {
                        return $"{_owner.Source.Schema.ResolveProperty(e.Member.Name, true)}";
                    }

                    _arguments.Add((e.Member.Name, e.GetValue()));
                    return $"@{e.Member.Name}";
                }

                default:
                    throw new Exception("Unhandeled expression in Where clause");
            }
        }
    }

    public class WhereVisitor: IDisposable
    {
        private readonly Sql _owner;
        private readonly List<(String, Object)> _arguments;
        private UInt64 _constCount;

        public WhereVisitor(Sql owner, List<(String, Object)> arguments, ref UInt64 constCount)
        {
            _owner = owner;
            _arguments = arguments;
            _constCount = constCount;
        }

        public void Dispose()
        {

        }

        public String Recurse(Type modelType, Expression expression)
        {
            switch (expression)
            {
                case UnaryExpression e:
                {
                    String right = Recurse(modelType, e.Operand);

                    // TODO(Chris Kruining)
                    // I doubt this is a proper implementation,
                    // but the documentation of C# says that 'Convert'
                    // is for type casting, since the right is
                    // a field selection this is not applicable
                    // (but the right does not have to be a field selection)
                    if (e.NodeType == ExpressionType.Convert)
                    {
                        return right;
                    }

                    return $"({e.NodeType.ToSql()} {right})";
                }

                case BinaryExpression e:
                {
                    // NOTE(Chris Kruining) Nasty hack to support enums as string in mysql
                    if (e.Left is UnaryExpression eLeft && eLeft.Operand.Type.IsEnum && e.Right is ConstantExpression eRight)
                    {
                        String constName = $"CONST_{_constCount++.Base52Encode()}";
                        _arguments.Add((constName, Enum.GetName(eLeft.Operand.Type, eRight.Value)!));
                        return $"{Recurse(modelType, e.Left)} {e.NodeType.ToSql()} @{constName}";
                    }

                    String right = Recurse(modelType, e.Right);

                    switch (e.NodeType)
                    {
                        case ExpressionType.Coalesce:
                        {
                            if (right.StartsWith("COALESCE"))
                            {
                                right = right.Substring(right.IndexOf('(') + 1).TrimEnd(')');
                            }

                            return $"COALESCE({Recurse(modelType, e.Left)}, {right})";
                        }

                        case ExpressionType.OrElse:
                        {
                            return $"({Recurse(modelType, e.Left)} {e.NodeType.ToSql(right == "NULL")} {right})";
                        }

                        default:
                        {
                            return $"{Recurse(modelType, e.Left)} {e.NodeType.ToSql(right == "NULL")} {right}";
                        }
                    }
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

                    if (e.Member.DeclaringType == modelType)
                    {
                        return $"{_owner.Source.Schema.ResolveProperty(e.Member.Name)}";
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
                            return $"{Recurse(modelType, e.Arguments[0])} IN([Parse_what_is_in_front_of_contains])";
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
            ExpressionType.Convert => "",
            _ => throw new Exception($"Unhandeled unary node type, '{expressionType}'"),
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