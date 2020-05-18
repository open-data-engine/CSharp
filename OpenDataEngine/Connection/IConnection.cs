using System;
using System.Collections.Generic;
using System.Threading;

namespace OpenDataEngine.Connection
{
    public interface IConnection
    {
        IAsyncEnumerable<IDictionary<String, dynamic>> Execute(String command, (String, Object)[] arguments, CancellationToken token);
    }
}
