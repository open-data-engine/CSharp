using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDataEngine.Query
{
    public class QueryProvider<TModel> : IAsyncQueryProvider
    {
        public IEnumerable<String> Fields;
        private readonly Func<IAsyncQueryable, String> _translator;
        private readonly Func<Type, String, Object[], IAsyncEnumerable<TModel>> _executor;

        public QueryProvider(Func<IAsyncQueryable, String> translator, Func<Type, String, Object[], IAsyncEnumerable<TModel>> executor)
        {
            _translator = translator;
            _executor = executor;
        }

        public IAsyncQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new Query<TElement>(this, expression);
        }

        public async ValueTask<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken token)
        {
            if (typeof(TResult) != typeof(IAsyncEnumerable<TModel>))
            {
                throw new NotImplementedException($"`{typeof(TResult)}` is not supported by `{typeof(QueryProvider<TModel>)}`");
            }

            // Convert the `Expression` into a SQL string
            // TODO(Chris Kruining) Select current source and use the adapter to translate the query to a string
            QueryBuilder<TModel> builder = new QueryBuilder<TModel>(CreateQuery<TModel>(expression));

            (String command, Object[] arguments) = builder.Result;

            return (TResult)_executor(typeof(TModel), command, arguments);

        }
    }
}