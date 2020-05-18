using System;
using System.Collections.Generic;
using System.Threading;

namespace OpenDataEngine.Connection
{
    public class Http: Base
    {
        private String endpoint;
        private dynamic config;

        public Http(String endpoint, dynamic config)
        {
            this.endpoint = endpoint;
            this.config = config;
        }

        public override IAsyncEnumerable<IDictionary<String, dynamic>> Execute(String command, (String, Object)[] arguments, CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}
