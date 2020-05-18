using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDataEngine.Strategy
{
    public class StraightForward<TModel> : Base<TModel>
    {
        private readonly Source<TModel> _source;

        public StraightForward(Source<TModel> source)
        {
            _source = source;
        }

        public override ValueTask<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken token)
        {
            return _source.ExecuteAsync<TResult>(expression, token);
        }
    }
}