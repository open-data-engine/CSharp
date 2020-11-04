using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using OpenDataEngine.Attribute;
using OpenDataEngine.Source;

namespace OpenDataEngine.Strategy
{
    public class StraightForward : Base
    {
        private readonly String? _sourceKey;
        private readonly Source.Source? _source;

        public StraightForward(String sourceKey)
        {
            _sourceKey = sourceKey;
        }

        public StraightForward(Source.Source? source)
        {
            _source = source;
        }

        public override ValueTask<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken token)
        {
            ISource source = _source ?? GetModelType<TResult>().GetCustomAttribute<SourcesAttribute>()?[_sourceKey!] ?? throw new Exception("No source defined");

            return source.ExecuteAsync<TResult>(expression, token);
        }
    }
}