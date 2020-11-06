using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OpenDataEngine.Query.Clause
{
    public enum OrderDirection
    {
        Ascending,
        Descending,
    }

    public class Order : Base
    {
        public readonly Type ModelType;
        public readonly Expression Filter;
        public readonly OrderDirection Direction;

        public Order(Type type, Expression filter, OrderDirection direction)
        {
            ModelType = type;
            Filter = filter;
            Direction = direction;
        }
    }
}
