using System;

namespace OpenDataEngine.Query.Clause
{
    public class Where: IClause
    {
        private String v;

        public Where(String v)
        {
            this.v = v;
        }

        public static implicit operator Where(String v) => new Where(v);
    }
}
