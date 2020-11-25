using System;
using System.Linq;
using System.Linq.Expressions;
using OpenDataEngine.Adapter;
using OpenDataEngine.Query;

namespace OpenDataEngine.Relation
{
    public class OwnsMany<TLocal, TForeign> : Base<TLocal, TForeign>
    {
        public OwnsMany(String name, Expression<Func<TLocal, TForeign, Boolean>> bindings) : base(name, bindings)
        {

        }

        public override String Resolve(IAdapter? adapter)
        {
            var local = typeof(TLocal);
            var foreign = typeof(TForeign);

            IAsyncQueryable<TForeign> query = new Query<TForeign>().Select();
            var kaas = adapter.Translate(query);

            return $"INNER JOIN {typeof(TForeign)} AS {Name} ON {Body}";
        }
    }
}
