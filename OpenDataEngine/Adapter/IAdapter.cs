using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace OpenDataEngine.Adapter
{
    public interface IAdapter<TModel>
    {
        public Source<TModel> Source { get; set; }

        public (String Command, (String, Object)[] Arguments) Translate(IAsyncQueryable query);
        public IAsyncEnumerable<TResult> From<TResult>(IAsyncEnumerable<IDictionary<String, dynamic>> source, CancellationToken token);
    }
}
