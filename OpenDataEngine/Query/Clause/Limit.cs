using System;
using System.Collections.Generic;
using System.Text;

namespace OpenDataEngine.Query.Clause
{
    public class Limit: IClause
    {
        private UInt16 v;

        public Limit(UInt16 v)
        {
            this.v = v;
        }

        public static implicit operator Limit(UInt16 v) => new Limit(v);
    }
}
