using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDataEngine.Strategy
{
    public class StraightForward : Base
    {
        private readonly SourceReference _source;

        public StraightForward(SourceReference source)
        {
            _source = source;
        }

        public override ValueTask<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken token)
        {
            return _source.Resolve<TResult>()?.ExecuteAsync<TResult>(expression, token) ?? throw new Exception("Unable to resolve the source");
        }
    }
}