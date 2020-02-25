using OpenDataEngine.Query.Clause;

namespace OpenDataEngine
{
    public class Model<T> where T : Model<T>, new()
    {
        public static Query<T> Select(Select filter) => Query<T>.Select(filter);
        public static Query<T> Where(Where filter) => Query<T>.Where(filter);
        public static Query<T> Limit(Limit filter) => Query<T>.Limit(filter);
    }
}
