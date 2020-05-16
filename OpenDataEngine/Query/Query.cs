using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDataEngine.Query
{
    public abstract class Queryable<TModel>
    {
        public static IAsyncQueryable<TModel> From(Source<TModel> source) => new Query<TModel>().From(source);
        public static IAsyncQueryable<TModel> Select(Expression<Func<TModel, dynamic>> expression) => new Query<TModel>().Select(expression);
        public static IAsyncQueryable<TModel> Where(Expression<Func<TModel, Boolean>> expression) => new Query<TModel>().Where(expression);
        public static IAsyncQueryable<TModel> Skip(Int32 numberOf) => new Query<TModel>().Skip(numberOf);
        public static IAsyncQueryable<TModel> Take(Int32 numberOf) => new Query<TModel>().Take(numberOf);
        public static IAsyncQueryable<TModel> Limit(Int32 take, Int32 skip = 0) => new Query<TModel>().Skip(skip).Take(take);
        public static IAsyncQueryable<TModel> OrderBy(Expression<Func<TModel, dynamic>> expression) => new Query<TModel>().OrderBy(expression);
    }

    public class Query<TModel> : IOrderedAsyncQueryable<TModel>
    {
        public IAsyncQueryProvider Provider { get; }
        public Type ElementType { get; } = typeof(TModel);
        public Expression Expression { get; }

        public Query()
        {
            Provider = new QueryProvider();
            Expression = Expression.Parameter(typeof(IAsyncQueryable<TModel>), "queryable");
        }

        public Query(IAsyncQueryProvider provider, Expression expression)
        {
            Provider = provider;
            Expression = expression;
        }

        public async IAsyncEnumerator<TModel> GetAsyncEnumerator(CancellationToken cancellationToken = new CancellationToken())
        {
            IAsyncEnumerable<TModel> enumerable = await Provider.ExecuteAsync<IAsyncEnumerable<TModel>>(Expression, cancellationToken);

            await foreach (TModel model in enumerable.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                yield return model;
            }
        }
    }
}