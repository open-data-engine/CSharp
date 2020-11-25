using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using OpenDataEngine.Attribute;
using OpenDataEngine.Query;
using OpenDataEngine.Source;

namespace OpenDataEngine.Strategy
{
    public abstract class Base : QueryProvider, IStrategy
    {
        public override abstract ValueTask<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken token);
    }

    public sealed class SourceReference
    {
        public String? Key { get; private set; }
        public ISource? Source { get; private set; }

        public ISource? Resolve<TModel>() => Source ?? GetModelType<TModel>().GetCustomAttribute<SourcesAttribute>()?[Key!];

        private Type GetModelType<T>() => typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(IAsyncEnumerable<>)
            ? typeof(T).GetGenericArguments()[0]
            : typeof(T).GetElementType() ?? typeof(T);

        public static implicit operator SourceReference(String key) => new SourceReference { Key = key };
        public static implicit operator SourceReference(Source.Source source) => new SourceReference { Source = source };
    }
}