using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace OpenDataEngine.Query
{
    public static class AsyncQueryableExtensions
    {
        private static MethodInfo? selectMethodInfo;
        private static MethodInfo? SelectMethod(Type type) => (selectMethodInfo ??= new Func<IAsyncQueryable<Object>, Expression<Func<Object, Object>>, IAsyncQueryable<Object>>(Select).GetMethodInfo()!.GetGenericMethodDefinition()).MakeGenericMethod(type);
        
        public static IAsyncQueryable<TModel> Select<TModel>(this IAsyncQueryable<TModel> query, Expression<Func<TModel, dynamic>> expression)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            return query.Provider.CreateQuery<TModel>(Expression.Call(SelectMethod(typeof(TModel)), query.Expression, expression));
        }
    }
}