using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using OpenDataEngine.Query;

namespace OpenDataEngine.Adapter
{
    public abstract class Base : QueryVisitor, IAdapter
    {
        public Source.Source Source { get; set; }

        public abstract (String Command, (String, Object)[] Arguments) Translate(IAsyncQueryable query);
        public abstract IAsyncEnumerable<TResult> From<TResult>(IAsyncEnumerable<IDictionary<String, dynamic>> source, CancellationToken token);
    }
}
