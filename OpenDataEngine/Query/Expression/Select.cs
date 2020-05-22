using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace OpenDataEngine.Query.Clause
{
    public class Select : Base
    {
        public readonly Type ModelType;
        public readonly IReadOnlyCollection<Expression> Fields;

        public Select(Type type, IReadOnlyCollection<Expression> fields)
        {
            ModelType = type;
            Fields = fields;
        }
    }
}