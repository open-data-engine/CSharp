using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDataEngine.Connection
{
    public class Default : Base
    {
        public override Task Connect(CancellationToken token)
        {
            return Task.CompletedTask;
        }

        public override async IAsyncEnumerable<IDictionary<String, dynamic>> Execute(String command, (String, Object)[] arguments, CancellationToken token)
        {
            await Task.CompletedTask;

            yield break;
        }
    }
}
