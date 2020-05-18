using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace OpenDataEngine.Adapter
{
    public class Json: Base
    {
        protected override void Emit(String command, dynamic? arguments)
        {
            throw new NotImplementedException();
        }

        public override (String Command, (String, Object)[] Arguments) Translate(IAsyncQueryable query)
        {
            throw new NotImplementedException();
        }

        public override IAsyncEnumerable<TResult> From<TResult>(IAsyncEnumerable<IDictionary<String, dynamic>> source, CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}
