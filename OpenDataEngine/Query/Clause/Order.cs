using System;
using System.Collections.Generic;
using System.Text;

namespace OpenDataEngine.Query.Clause
{
    public class Order: IClause
    {
        public enum Direction
        {
            Ascending,
            Descending,
        }

        private Direction v;

        public Order(Direction v)
        {
            this.v = v;
        }

        public static implicit operator Order(Direction v) => new Order(v);
    }
}
