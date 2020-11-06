using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace OpenDataEngine.Query.Clause
{
    public class Select : Base
    {
        public readonly String Name;
        public readonly Type ModelType;
        public readonly IReadOnlyCollection<Expression> Fields;

        public Select(String name, Type type, IReadOnlyCollection<Expression> fields)
        {
            Name = name;
            ModelType = type;
            Fields = fields;
        }
    }
}