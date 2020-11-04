using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using OpenDataEngine.Query;

namespace OpenDataEngine.Strategy
{
    public abstract class Base : QueryProvider, IStrategy
    {
        public override abstract ValueTask<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken token);

        protected Type GetModelType<T>() => typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(IAsyncEnumerable<>)
            ? typeof(T).GetGenericArguments()[0]
            : typeof(T).GetElementType() ?? typeof(T);
    }
}