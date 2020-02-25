using System;
using System.Collections.Generic;
using System.Text;

namespace OpenDataEngine.Schema
{
    public class Rest: Base
    {
        private String resource;
        private dynamic map;

        public Rest(String resource, dynamic map)
        {
            this.resource = resource;
            this.map = map;
        }
    }
}
