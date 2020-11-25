using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OpenDataEngine.Source;

namespace OpenDataEngine.Connection
{
    public abstract class Base: IConnection
    {
        public ISource? Source { get; set; }

        public abstract Task Connect(CancellationToken token);
        public abstract IAsyncEnumerable<IDictionary<String, dynamic>> Execute(String command, (String, Object)[] arguments, CancellationToken token);
    }
}
