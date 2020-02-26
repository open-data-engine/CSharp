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
        private readonly List<IClause> clauses = new List<IClause>();

        public static Query<T> Select(params Select[] clauses) => new Query<T>().Select(clauses);
        public static Query<T> Where(params Where[] clauses) => new Query<T>().Where(clauses);
        public static Query<T> Limit(params Limit[] clauses) => new Query<T>().Limit(clauses);
        public static Query<T> Order(params Order[] clauses) => new Query<T>().Order(clauses);

        public async Task<T> Find(dynamic args = null)
        {
            return new T();
        }
        public async IAsyncEnumerable<T> FindAll(dynamic args = null)
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

        public void Add(IClause clause) => this.clauses.Add(clause);
        public void Add(params IClause[] clauses) => this.clauses.AddRange(clauses);

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

        public static Query<T> operator +(Query<T> self, IClause clause)
        {
            self.Add(clause);

            return self;
        }
    }

    public static class QueryExtension
    {
        public static Query<T> Select<T>(this Query<T> subject, params Select[] clauses) where T : Model<T>, new()
        {
            subject.Add(clauses);

            return subject;
        }

        public static Query<T> Where<T>(this Query<T> subject, params Where[] clauses) where T : Model<T>, new()
        {
            subject.Add(clauses);

            return subject;
        }

        public static Query<T> And<T>(this Query<T> subject, params Where[] clauses) where T : Model<T>, new()
        {
            subject.Add(clauses);

            return subject;
        }

        public static Query<T> Limit<T>(this Query<T> subject, params Limit[] clauses) where T : Model<T>, new()
        {
            subject.Add(clauses);

            return subject;
        }

        public static Query<T> Order<T>(this Query<T> subject, params Order[] clauses) where T : Model<T>, new()
        {
            subject.Add(clauses);

            return subject;
        }
    }
}
