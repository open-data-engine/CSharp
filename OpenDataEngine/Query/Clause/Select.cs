using System;

namespace OpenDataEngine.Query.Clause
{
    public class Select : IClause
    {
        private String v;

        public Select(String v)
        {
            this.v = v;
        }

        public static implicit operator Select(String v) => new Select(v);
    }
}
