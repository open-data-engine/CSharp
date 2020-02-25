using OpenDataEngine.Query.Clause;

namespace OpenDataEngine
{
    public class Model<T> where T : Model<T>, new()
    {
        public static Query<T> Select(params Select[] clauses) => Query<T>.Select(clauses);
        public static Query<T> Where(params Where[] clauses) => Query<T>.Where(clauses);
        public static Query<T> Limit(params Limit[] clauses) => Query<T>.Limit(clauses);
        public static Query<T> Order(params Order[] clauses) => Query<T>.Order(clauses);

        public I Get<I>(Field<I> field) => default;
    }
}
