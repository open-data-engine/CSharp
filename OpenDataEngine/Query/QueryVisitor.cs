using System;
using System.Linq;
using System.Linq.Expressions;
using OpenDataEngine.Query.Clause;

namespace OpenDataEngine.Query
{
    public abstract class QueryVisitor : ExpressionVisitor
    {
        private static readonly Type[] validTypes =
        {
            typeof(AsyncQueryable),
            typeof(AsyncQueryableExtensions)
        };
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (validTypes.Contains(node.Method.DeclaringType) == false)
            {
                return base.VisitMethodCall(node);
            }

            // 'this' argument due to extension method
            Visit(node.Arguments[0]);

            switch (node.Method.Name)
            {
                case nameof(AsyncQueryable.Select):
                {
                    if (!(StripQuotes(node.Arguments[1]) is LambdaExpression body))
                    {
                        throw new Exception("Unable to visit the argument of Select");
                    }

                    return new Select(body.Parameters[0].Name!, body.Parameters[0].Type, (body.Body as NewExpression)!.Arguments);
                }

                case nameof(AsyncQueryable.Where):
                {
                    if (!(StripQuotes(node.Arguments[1]) is LambdaExpression body))
                    {
                        throw new Exception("Unable to visit the argument of Where");
                    }

                    return new Where(body.Parameters[0].Name!, body.Parameters[0].Type, body.Body);
                }

                case nameof(AsyncQueryable.Take):
                {
                    return new Limit(node.Arguments[1], node.Arguments[2] ?? null);
                }

                case nameof(AsyncQueryable.Skip):
                {
                    return new Limit(null, node.Arguments[1]);
                }

                case nameof(AsyncQueryable.OrderBy):
                case nameof(AsyncQueryable.ThenBy):
                {
                    if (!(StripQuotes(node.Arguments[1]) is LambdaExpression body))
                    {
                        throw new Exception("Unable to visit the argument of Select");
                    }

                    return new Order(body.Parameters[0].Type, body.Body, OrderDirection.Ascending);
                }

                case nameof(AsyncQueryable.OrderByDescending):
                case nameof(AsyncQueryable.ThenByDescending):
                {
                    if (!(StripQuotes(node.Arguments[1]) is LambdaExpression body))
                    {
                        throw new Exception("Unable to visit the argument of Select");
                    }

                    return new Order(body.Parameters[0].Type, body.Body, OrderDirection.Descending);
                }

                case nameof(AsyncQueryable.Reverse):
                {
                    // TODO(Chris Kruining) Because `Reverse` has no parameters I need to figure out, mysql for one ignores ordering by null
                    return new Order(typeof(void), Expression.Lambda(Expression.Constant(null)), OrderDirection.Descending);
                }

                default:
                    throw new NotSupportedException("Operator could not be converted to String");
            }
        }

        private static Expression StripQuotes(Expression e)
        {
            while (e.NodeType == ExpressionType.Quote)
            {
                e = ((UnaryExpression)e).Operand;
            }

            return e;
        }
    }
}