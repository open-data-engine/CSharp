using System;
using System.Linq.Expressions;

namespace OpenDataEngine.Query.Clause
{
    public class Where : Base
    {
        public readonly String Name;
        public readonly Type ModelType;
        public readonly Expression Body;

        public Where(String name, Type type, Expression body)
        {
            Name = name;
            ModelType = type;
            Body = body;
        }
    }
}