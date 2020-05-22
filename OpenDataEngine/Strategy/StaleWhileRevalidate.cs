using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDataEngine.Strategy
{
    public class StaleWhileRevalidate: Base
    {
        private readonly Source.Source _cache;
        private readonly Source.Source _fallback;

        public StaleWhileRevalidate(Source.Source cache, Source.Source fallback)
        {
            _cache = cache;
            _fallback = fallback;
        }

        public override ValueTask<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken token)
        {
            return _fallback.ExecuteAsync<TResult>(expression, token);
        }
    }
}