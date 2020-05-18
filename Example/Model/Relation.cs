using System;
using OpenDataEngine.Query;

namespace Example.Model
{
    public class Relation : Queryable<Relation>
    {
        public UInt32 ID { get; set; }
        public String Username { get; set; }
    }
}