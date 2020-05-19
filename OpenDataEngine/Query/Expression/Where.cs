using System;
using System.Linq.Expressions;

namespace OpenDataEngine.Query.Clause
{
    public class Where : Base
    {
        public readonly Expression Body;

        public Where(Expression body)
        {
            Body = body;
        }
    }
}