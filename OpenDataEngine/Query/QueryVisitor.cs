using System;
using System.Linq;
using System.Linq.Expressions;
using OpenDataEngine.Query.Clause;

namespace OpenDataEngine.Query
{
    public abstract class QueryVisitor : ExpressionVisitor
    {
        protected abstract void Emit(String command, dynamic? arguments);

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

            switch (node.Method.Name)
            {
                case "Select":
                    // 'this' argument due to extension method
                    Visit(node.Arguments[0]);

                    if (!(StripQuotes(node.Arguments[1]) is LambdaExpression selectExpression))
                    {
                        throw new Exception("Unable to visit the argument of Select");
                    }

                    return Expression.Call(node.Object, node.Method, (selectExpression.Body as NewExpression)?.Arguments);

                case "Where":
                    // 'this' argument due to extension method
                    Visit(node.Arguments[0]);


                    if (!(StripQuotes(node.Arguments[1]) is LambdaExpression whereExpression))
                    {
                        throw new Exception("Unable to visit the argument of Where");
                    }

                    return new Where{ Body = whereExpression.Body };

                    break;

                default:
                    throw new NotSupportedException("Operator could not be converted to String");
            }

            return node;
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            return base.VisitUnary(node);
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            return base.VisitBinary(node);
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            return base.VisitConstant(node);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            // Builder.Append($"`{node.Member.Name}`");

            return node;
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