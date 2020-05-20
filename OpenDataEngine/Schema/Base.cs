using System;

namespace OpenDataEngine.Schema
{
    public abstract class Base<TModel> : ISchema<TModel>
    {
        public Source<TModel> Source { get; set; }

        public abstract String ResolvePath(String path);

        public virtual String ResolveProperty(String property)
        {
            return property;
        }
    }
}
