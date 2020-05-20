using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using OpenDataEngine.Adapter;
using OpenDataEngine.Connection;
using OpenDataEngine.Query;
using OpenDataEngine.Schema;

namespace OpenDataEngine
{
    public class Source<TModel>: QueryProvider
    {
        public readonly IConnection Connection;
        public readonly IAdapter<TModel> Adapter;
        public readonly ISchema<TModel> Schema;

        public Source(IConnection connection, IAdapter<TModel> adapter, ISchema<TModel> schema)
        {
            Connection = connection;
            Adapter = adapter;
            Schema = schema;

            if (Adapter != null)
            {
                Adapter.Source = this;
            }

            if (Schema != null)
            {
                Schema.Source = this;
            }
        }

        public override async ValueTask<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken token)
        {
            if (typeof(TResult) == typeof(IAsyncEnumerable<TModel>))
            {
                return (TResult)this.ExecuteMany(this.Prepare(expression), token);
            }

            if (typeof(TResult) == typeof(TModel))
            {
                return (TResult)(Object)(await this.ExecuteSingle(this.Prepare(expression), token))!;
            }

            throw new Exception($"Unhandeled result type `{typeof(TResult)}`");
        }
    }

    public static class SourceExtensions
    {
        public static (String command, (String, Object)[] arguments) Prepare<TModel>(this Source<TModel> source, Expression expression) => source.Adapter.Translate(source.CreateQuery<TModel>(expression));

        public static IAsyncEnumerable<TModel> ExecuteMany<TModel>(this Source<TModel> source, (String command, (String, Object)[] arguments) statement, CancellationToken token) => 
            source.Adapter.From<TModel>(source.Connection.Execute(statement.command, statement.arguments, token), token);

        public static ValueTask<TModel> ExecuteSingle<TModel>(this Source<TModel> source, (String command, (String, Object)[] arguments) statement, CancellationToken token) => source.ExecuteMany(statement, token).SingleAsync(token);
    }
}
