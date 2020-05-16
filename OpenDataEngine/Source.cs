using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using OpenDataEngine.Adapter;
using OpenDataEngine.Connection;
using OpenDataEngine.Query;
using OpenDataEngine.Schema;

namespace OpenDataEngine
{
    public class Source<TModel>: QueryProvider
    {
        public readonly IConnection Connection;
        public readonly IAdapter Adapter;
        public readonly ISchema Schema;

        public Source(IConnection connection, IAdapter adapter, ISchema schema)
        {
            Connection = connection;
            Adapter = adapter;
            Schema = schema;
        }

        public override async ValueTask<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken token)
        {

            (String command, Object[] arguments) = Adapter.Translate(CreateQuery<TModel>(expression));

            await foreach (dynamic record in Connection.Execute(command, arguments).WithCancellation(token).ConfigureAwait(false))
            {
                
            }

            return (TResult)Connection.Execute(command, arguments);
        }
    }
}
