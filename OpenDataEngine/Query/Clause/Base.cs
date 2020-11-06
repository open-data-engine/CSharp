using System;
using System.Collections.Generic;

namespace OpenDataEngine.Query.Clause
{
    public abstract class Base : System.Linq.Expressions.Expression
    {
        protected readonly List<(String, Object)> Arguments = new List<(String, Object)>();
    }
}