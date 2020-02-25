using System;

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
    }
}
