namespace OpenDataEngine.Source
{
    public class Cache : Source
    {
        public Cache() : base(new Connection.Default(), new Adapter.Default(), new Schema.Default()) { }
    }
}