using System.Linq;
using Microsoft.Extensions.Logging;
using OpenDataEngine.Adapter;
using OpenDataEngine.Connection;
using OpenDataEngine.Schema;

namespace OpenDataEngine.Source
{
    public interface ISource : IAsyncQueryProvider
    {
        public ILogger<ISource>? Logger { get; set; }
        public IConnection Connection { get; }
        public IAdapter Adapter { get; }
        public ISchema Schema { get; }
    }
}