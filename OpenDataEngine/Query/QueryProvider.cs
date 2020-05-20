using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDataEngine.Query
{
    public class QueryProvider : IAsyncQueryProvider
    {
        public IAsyncQueryable<TModel> CreateQuery<TModel>(Expression expression) => new Query<TModel>(this, expression);

        public virtual ValueTask<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}