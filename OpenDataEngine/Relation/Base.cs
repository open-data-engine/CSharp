using System;
using System.Linq.Expressions;
using OpenDataEngine.Adapter;
using OpenDataEngine.Query;

namespace OpenDataEngine.Relation
{
    public abstract class Base<TLocal, TForeign> : QueryVisitor, IRelation
    {
        public String Name { get; }
        public LambdaExpression Body { get; }

        protected Base(String name, Expression<Func<TLocal, TForeign, Boolean>> bindings)
        {
            Name = name;
            Body = bindings;
        }

        public abstract String Resolve(IAdapter? adapter);
    }
}
