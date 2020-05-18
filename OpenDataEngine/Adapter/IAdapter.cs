using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace OpenDataEngine.Adapter
{
    public interface IAdapter
    {
        public (String Command, (String, Object)[] Arguments) Translate(IAsyncQueryable query);
        public IAsyncEnumerable<TResult> From<TResult>(IAsyncEnumerable<IDictionary<String, dynamic>> source, CancellationToken token);
    }
}
