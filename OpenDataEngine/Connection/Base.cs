using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenDataEngine.Connection
{
    public abstract class Base: IConnection
    {
        public abstract IAsyncEnumerable<dynamic> Execute(String command, dynamic arguments);
    }
}
