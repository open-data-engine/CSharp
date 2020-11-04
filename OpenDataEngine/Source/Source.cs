using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using OpenDataEngine.Adapter;
using OpenDataEngine.Connection;
using OpenDataEngine.Query;
using OpenDataEngine.Schema;

namespace OpenDataEngine.Source
{
    public class Source : QueryProvider, ISource
    {
        public readonly IConnection Connection;
        public readonly IAdapter Adapter;
        public readonly ISchema Schema;

        public Source(IConnection connection, IAdapter adapter, ISchema schema)
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
            if (!(typeof(TResult).IsGenericType && typeof(TResult).GetGenericTypeDefinition() == typeof(IAsyncEnumerable<>)))
            {
                return await this.ExecuteSingle<TResult>(this.Prepare<TResult>(expression), token);
            }
            
            Type modelType = typeof(TResult).GetGenericArguments()[0];
            Object prepared = typeof(SourceExtensions)
                .GetMethod("Prepare", BindingFlags.Static | BindingFlags.Public)
                ?.MakeGenericMethod(modelType)
                .Invoke(null, new Object[] { this, expression })!;

            return (TResult)typeof(SourceExtensions)
                .GetMethod("ExecuteMany", BindingFlags.Static | BindingFlags.Public)
                ?.MakeGenericMethod(modelType)
                .Invoke(null, new [] { this, prepared, token })!;
        }
    }

    public static class SourceExtensions
    {
        public static (String command, (String, Object)[] arguments) Prepare<TModel>(this Source source, Expression expression) => source.Adapter.Translate(source.CreateQuery<TModel>(expression));

        public static IAsyncEnumerable<TModel> ExecuteMany<TModel>(this Source source, (String command, (String, Object)[] arguments) statement, CancellationToken token) => 
            source.Adapter.From<TModel>(source.Connection.Execute(statement.command, statement.arguments, token), token);

        public static async ValueTask<TModel> ExecuteSingle<TModel>(this Source source, (String command, (String, Object)[] arguments) statement, CancellationToken token)
        {
            IAsyncEnumerator<TModel> enumerator = source.ExecuteMany<TModel>(statement, token).GetAsyncEnumerator(token);
            
            await enumerator.MoveNextAsync();
            TModel result = enumerator.Current;
            await enumerator.DisposeAsync();

            return result;
        }
    }
}
