using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using OpenDataEngine.Source;

namespace OpenDataEngine.Strategy
{
    public class CacheFirst: Base
    {
        private readonly SourceReference _cache;
        private readonly SourceReference _fallback;
        
        public CacheFirst(SourceReference cache, SourceReference fallback)
        {
            _cache = cache;
            _fallback = fallback;
        }
        
        public override ValueTask<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken token)
        {
            ISource source = _fallback.Resolve<TResult>() ?? throw new Exception("Couldn't resolve 'fallback' source");

            return source.ExecuteAsync<TResult>(expression, token);
        }
    }
}