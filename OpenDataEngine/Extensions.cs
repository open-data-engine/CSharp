using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace OpenDataEngine
{
    public static class Extensions
    {
        public static IDictionary<TKey, TValue> TopoLogicalSort<TKey, TValue>(this IDictionary<TKey, TValue> source, IDictionary<TKey, TKey[]> dependencies) where TKey: notnull
        {
            Dictionary<TKey, UInt32> keys = source.Keys.ToDictionary<TKey, TKey, UInt32>(k => k, _ => 0);

            foreach ((TKey _, TKey[] edges) in dependencies)
            {
                foreach (TKey edge in edges)
                {
                    if (keys.ContainsKey(edge))
                    {
                        keys[edge]++;
                    }
                }
            }

            List<KeyValuePair<TKey, UInt32>> list = keys.ToList();
            list.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));
            list.Reverse();

            return list.ToDictionary(kvp => kvp.Key, kvp => source[kvp.Key]);
        }

        public static dynamic ToObject(this IDictionary<String, Object> source)
        {
            dynamic output = new ExpandoObject();

            foreach ((String key, Object value) in source)
            {
                output[key] = value;
            }

            return output;
        }
    }
}