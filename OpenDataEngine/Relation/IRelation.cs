using System;
using System.Linq.Expressions;
using OpenDataEngine.Adapter;

namespace OpenDataEngine.Relation
{
    public interface IRelation
    {
        public String Name { get; }
        public LambdaExpression Body { get; }

        public String Resolve(IAdapter? adapter);
    }
}
