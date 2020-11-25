using System;
using System.Linq.Expressions;
using OpenDataEngine.Adapter;

namespace OpenDataEngine.Relation
{
    public class HasMany<TLocal, TForeign> : Base<TLocal, TForeign>
    {
        public HasMany(String name, Expression<Func<TLocal, TForeign, Boolean>> bindings): base(name, bindings)
        {

        }

        public override String Resolve(IAdapter? adapter)
        {
            return "";
        }
    }
}
