using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDataEngine.Strategy
{
    public class StaleWhileRevalidate: Base
    {
        private readonly SourceReference _cache;
        private readonly SourceReference _fallback;

        public StaleWhileRevalidate(SourceReference cache, SourceReference fallback)
        {
            _cache = cache;
            _fallback = fallback;
        }

        public override ValueTask<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken token)
        {
            return _fallback.Resolve<TResult>()?.ExecuteAsync<TResult>(expression, token) ?? throw new Exception("Unable to resolve the 'fallback' source");
        }
    }
}