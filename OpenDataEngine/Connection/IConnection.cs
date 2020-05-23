using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDataEngine.Connection
{
    public interface IConnection
    {
        Task Connect(CancellationToken token);
        IAsyncEnumerable<IDictionary<String, dynamic>> Execute(String command, (String, Object)[] arguments, CancellationToken token);
    }
}
