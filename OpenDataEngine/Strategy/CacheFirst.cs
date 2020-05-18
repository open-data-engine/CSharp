using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDataEngine.Strategy
{
    public class CacheFirst<TModel>: Base<TModel>
    {
        private readonly Source<TModel> _cache;
        private readonly Source<TModel> _fallback;

        public CacheFirst(Source<TModel> cache, Source<TModel> fallback)
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