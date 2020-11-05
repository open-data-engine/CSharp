using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDataEngine.Adapter
{
    public class Default : Base
    {
        public override (String Command, (String, Object)[] Arguments) Translate(IAsyncQueryable query)
        {
            return default;
        }

        public override async IAsyncEnumerable<TResult> From<TResult>(IAsyncEnumerable<IDictionary<String, dynamic>> source, CancellationToken token)
        {
            await Task.CompletedTask;

            yield break;
        }
    }
}
