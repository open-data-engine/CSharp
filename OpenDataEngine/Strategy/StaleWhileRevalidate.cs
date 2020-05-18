using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDataEngine.Strategy
{
    public class StaleWhileRevalidate<TModel>: Base<TModel>
    {
        private readonly Source<TModel> _cache;
        private readonly Source<TModel> _fallback;

        public StaleWhileRevalidate(Source<TModel> cache, Source<TModel> fallback)
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