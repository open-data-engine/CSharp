using System;
using OpenDataEngine.Source;

namespace OpenDataEngine.Schema
{
    public interface ISchema
    {
        public ISource? Source { get; set; }

        public String ResolvePath(String path);
        public String ResolveProperty(String property, Boolean alias = false);
        public String ReverseResolveProperty(String property);
    }
}
