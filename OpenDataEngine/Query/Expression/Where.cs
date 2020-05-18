using System;

namespace OpenDataEngine.Query.Clause
{
    public class Where : Base
    {
        public System.Linq.Expressions.Expression Body { get; set; }
    }
}