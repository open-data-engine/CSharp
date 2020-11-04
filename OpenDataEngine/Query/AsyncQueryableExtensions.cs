using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using OpenDataEngine.Strategy;

namespace OpenDataEngine.Query
{
    public static class AsyncQueryableExtensions
    {
        private static MethodInfo? selectMethodInfo;
        private static MethodInfo SelectMethod(Type type) => (selectMethodInfo ??= new Func<IAsyncQueryable<Object>, Expression<Func<Object, Object>>, IAsyncQueryable<Object>>(Select).GetMethodInfo()!.GetGenericMethodDefinition()).MakeGenericMethod(type);
        
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

        public static IAsyncQueryable<TModel> From<TModel>(this IAsyncQueryable<TModel> query, String source)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            
            return new Query<TModel>(default, query.Expression);
        }
        public static IAsyncQueryable<TModel> From<TModel>(this IAsyncQueryable<TModel> query, IAsyncQueryProvider source)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return new Query<TModel>(source, query.Expression);
        }

        public static IAsyncQueryable<TModel> With<TModel, TRelation>(this IAsyncQueryable<TModel> query, TRelation relation, String? name = null)
        {
            Type type = typeof(TModel);
            PropertyInfo? prop = name != null 
                ? type.GetProperty(name) 
                : type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Single(p => p.PropertyType == typeof(TRelation));

            if (prop == null)
            {
                throw new Exception("Unable to resolve relation");
            }

            // TODO(Chris Kruining) prepare `Join` so that the related data is fetched

            return query;
        }

        public static ValueTaskAwaiter<TModel> GetAwaiter<TModel>(this IAsyncQueryable<TModel> query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            return query.Take(1).Provider.ExecuteAsync<TModel>(query.Expression, CancellationToken.None).GetAwaiter();
        }

        public static async ValueTask<TModel[]?> ToArrayAsync<TModel>(this IAsyncQueryable<TModel> query) => (await query.ToListAsync())?.ToArray();
        public static async ValueTask<List<TModel>?> ToListAsync<TModel>(this IAsyncQueryable<TModel> query) => await AsyncEnumerable.ToListAsync(query);
    }
}