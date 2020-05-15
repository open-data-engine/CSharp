using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace OpenDataEngine.Query
{
    public class QueryBuilder<TModel>: QueryVisitor
    {
        public IAsyncQueryable<TModel> Query { get; }
        public StringBuilder Builder { get; } = new StringBuilder();

        public QueryBuilder(IAsyncQueryable<TModel> query)
        {
            Query = query;
        }

        private (String Command, Object[] Arguments)? result;
        public (String Command, Object[] Arguments) Result => result ??= Translate();

        public String Command => Result.Command;
        public Object[] Arguments => Result.Arguments;

        private (String Command, Object[] Arguments) Translate()
        {
            Visit(Query.Expression);

            return (Builder.ToString(), new Object[0]);
        }

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

            switch(node.Method.Name)
            {
                case "Select":
                    // if (query.Expression is Expression<Func<TModel, dynamic>> selectExpression)
                    // {
                    //     selection = (selectExpression.Body as NewExpression)?.Arguments.Select(a => (a as MemberExpression)?.Member.Name).Where(a => a != null);
                    // }

                    Builder.Append("SELECT ");

                    break;

                case "Where":
                    var args = node.Arguments.Select(a => Visit(a)).ToList();

                    Builder.Append("WHERE ");

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
            Builder.Append($"`{node.Member.Name}`");

            return node;
        }
    }
}