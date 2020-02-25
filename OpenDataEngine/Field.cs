using OpenDataEngine.Query.Clause;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenDataEngine
{
    public class Field<T>
    {
        public static implicit operator Select(Field<T> self) => new Select("");
        public static implicit operator Where(Field<T> self) => new Where("");
        public static implicit operator Limit(Field<T> self) => new Limit(0);
    }
}
