using System;
using System.Linq;

namespace OpenDataEngine.Adapter
{
    public interface IAdapter
    {
        public (String Command, Object[] Arguments) Translate(IAsyncQueryable query);
    }
}
