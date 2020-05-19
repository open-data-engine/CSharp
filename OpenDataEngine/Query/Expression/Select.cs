using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace OpenDataEngine.Query.Clause
{
    public class Select : Base
    {
        public readonly IReadOnlyCollection<Expression> Fields;

        public Select(IReadOnlyCollection<Expression> fields)
        {
            Fields = fields;
        }
    }
}