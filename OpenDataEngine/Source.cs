using System;
using System.Collections.Generic;
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

        public async ValueTask<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken token) => (TResult)ExecuteAsync(expression, token);

        public IAsyncEnumerable<TModel> ExecuteAsync(Expression expression, CancellationToken token) => this.Execute(this.Prepare(expression), token);
    }

    public static class SourceExtensions
    {
        public static (String command, (String, Object)[] arguments) Prepare<TModel>(this Source<TModel> source, Expression expression) => source.Adapter.Translate(source.CreateQuery<TModel>(expression));

        public static IAsyncEnumerable<TModel> Execute<TModel>(this Source<TModel> source, (String command, (String, Object)[] arguments) statement, CancellationToken token) => 
            source.Adapter.From<TModel>(source.Connection.Execute(statement.command, statement.arguments, token), token);
    }
}
