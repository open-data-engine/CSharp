using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using OpenDataEngine.Attribute;
using OpenDataEngine.Source;

namespace OpenDataEngine.Strategy
{
    public class CacheFirst: Base
    {
        private readonly String? _cacheKey;
        private readonly String? _fallbackKey;
        private readonly Source.Source? _cache;
        private readonly Source.Source? _fallback;
        
        public CacheFirst(String cache, String fallback)
        {
            _cacheKey = cache;
            _fallbackKey = fallback;
        }
        public CacheFirst(Source.Source cache, Source.Source fallback)
        {
            _cache = cache;
            _fallback = fallback;
        }

        public ISource? Resolve(Type model, String key) => key switch
        {
            "cache" => _cache ?? model.GetCustomAttribute<SourcesAttribute>()?[_cacheKey!],
            "fallback" => _fallback ?? model.GetCustomAttribute<SourcesAttribute>()?[_fallbackKey!],
            _ => throw new Exception("Unhandled source name"),
        };

        public override ValueTask<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken token)
        {
            if (_cache == null && _cacheKey == null)
            {
                throw new Exception("No deduction of 'cache' source possible, did you forget to pass either a `Source` or a (`String`)key?");
            }

            if (_fallback == null && _fallbackKey == null)
            {
                throw new Exception("No deduction of 'fallback' source possible, did you forget to pass either a `Source` or a (`String`)key?");
            }

            ISource source = Resolve(GetModelType<TResult>(), "fallback") ?? throw new Exception("Couldn't resolve 'fallback' source");

            return source.ExecuteAsync<TResult>(expression, token);
        }
    }
}