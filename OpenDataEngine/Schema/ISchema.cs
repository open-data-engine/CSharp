using System;
using System.Collections.Generic;
using System.Text;

namespace OpenDataEngine.Schema
{
    public interface ISchema
    {
        public Source.Source Source { get; set; }

        public String ResolvePath(String path);
        public String ResolveProperty(String property, Boolean alias = false);
        public String ReverseResolveProperty(String property);
    }
}
