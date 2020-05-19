using System;
using System.Collections.Generic;
using System.Text;

namespace OpenDataEngine.Schema
{
    public interface ISchema<TModel>
    {
        public Source<TModel> Source { get; set; }

        public String ResolveProperty(String property);
    }
}
