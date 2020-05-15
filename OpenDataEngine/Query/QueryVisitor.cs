using System;
using System.Linq.Expressions;

namespace OpenDataEngine.Query
{
    public class QueryVisitor : ExpressionVisitor
    {
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            return base.VisitMethodCall(node);
        }
    }
}