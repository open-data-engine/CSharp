using System;
using System.Linq.Expressions;

namespace OpenDataEngine.Query.Clause
{
    public class Where : Base
    {
        public readonly Type ModelType;
        public readonly Expression Body;

        public Where(Type type, Expression body)
        {
            ModelType = type;
            Body = body;
        }
    }
}