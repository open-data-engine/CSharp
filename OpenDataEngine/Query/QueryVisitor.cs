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
                case "Select":
                {
                    if (!(StripQuotes(node.Arguments[1]) is LambdaExpression body))
                    {
                        throw new Exception("Unable to visit the argument of Select");
                    }

                    return new Select(body.Parameters[0].Name!, body.Parameters[0].Type, (body.Body as NewExpression)!.Arguments);
                }

                case "Where":
                {
                    if (!(StripQuotes(node.Arguments[1]) is LambdaExpression body))
                    {
                        throw new Exception("Unable to visit the argument of Where");
                    }

                    return new Where(body.Parameters[0].Name!, body.Parameters[0].Type, body.Body);
                }

                case "Take":
                {
                    return new Limit(node.Arguments[1], node.Arguments[2] ?? null);
                }

                case "Skip":
                {
                    return new Limit(null, node.Arguments[1]);
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