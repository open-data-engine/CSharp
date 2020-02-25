using OpenDataEngine.Query.Clause;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenDataEngine
{
    public class Query<T>: IEnumerable<IClause> where T: Model<T>, new()
    {
        public static Query<T> Select(Select filter) => new Query<T>().Select(filter);
        public static Query<T> Where(Where filter) => new Query<T>().Where(filter);
        public static Query<T> Limit(Limit filter) => new Query<T>().Limit(filter);

        public async Task<T> Find()
        {
            return default;
        }

        public async IAsyncEnumerable<T> FindAll()
        {
            yield return new T();
        }

        public IEnumerator<IClause> GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        public void Add(IClause clause)
        {

        }

        public static implicit operator T(Query<T> self)
        {
            Task<T> task = self.Find();
            task.Wait();

            return task.Result;
        }

        public static implicit operator List<T>(Query<T> self)
        {
            Task<List<T>> task = self.FindAll().ToListAsync().AsTask();
            task.Wait();

            return task.Result;
        }
    }

    public static class QueryExtension
    {
        public static Query<T> Select<T>(this Query<T> subject, Select clause) where T : Model<T>, new()
        {
            subject.Add(clause);

            return subject;
        }

        public static Query<T> Where<T>(this Query<T> subject, Where clause) where T : Model<T>, new()
        {
            subject.Add(clause);

            return subject;
        }

        public static Query<T> And<T>(this Query<T> subject, Where clause) where T : Model<T>, new()
        {
            subject.Add(clause);

            return subject;
        }

        public static Query<T> Limit<T>(this Query<T> subject, Limit clause) where T : Model<T>, new()
        {
            subject.Add(clause);

            return subject;
        }
    }
}
