using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OpenDataEngine.Source;

namespace OpenDataEngine.Connection
{
    public interface IConnection
    {
        public ISource? Source { get; set; }

        Task Connect(CancellationToken token);
        IAsyncEnumerable<IDictionary<String, dynamic>> Execute(String command, (String, Object)[] arguments, CancellationToken token);
    }
}
