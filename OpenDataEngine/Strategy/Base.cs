using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using OpenDataEngine.Query;

namespace OpenDataEngine.Strategy
{
    public abstract class Base : QueryProvider, IStrategy
    {
        public override abstract ValueTask<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken token);
    }
}