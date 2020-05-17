using System;
using System.Linq;
using OpenDataEngine.Query;

namespace OpenDataEngine.Adapter
{
    public abstract class Base: QueryVisitor, IAdapter
    {
        public abstract (String Command, Object[] Arguments) Translate(IAsyncQueryable query);
    }
}
