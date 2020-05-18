using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using OpenDataEngine.Query;

namespace OpenDataEngine.Strategy
{
    public abstract class Base<TModel> : QueryProvider, IStrategy<TModel>
    {
        public override abstract ValueTask<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken token);
    }
}