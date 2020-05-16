using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OpenDataEngine.Connection
{
    public interface IConnection
    {
        IAsyncEnumerable<dynamic> Execute(String command, dynamic arguments);
    }
}
