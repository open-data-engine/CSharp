using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDataEngine.Strategy
{
    public class StraightForward : Base
    {
        private readonly Source.Source? _source;

        public StraightForward(Source.Source? source)
        {
            _source = source;
        }

        public override ValueTask<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken token)
        {
            return _source?.ExecuteAsync<TResult>(expression, token) ?? throw new Exception("No source defined");
        }
    }
}