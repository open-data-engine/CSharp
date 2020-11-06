using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace OpenDataEngine.Query.Clause
{
    public class Limit : Base
    {
        public readonly Expression? Amount;
        public readonly Expression? Offset;

        public Limit(Expression? amount, Expression? offset = null)
        {
            Amount = amount;
            Offset = offset;
        }
    }
}