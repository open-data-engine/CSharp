using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
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

        public static dynamic ToObject(this IDictionary<String, Object> source) => source;

        public static Byte DecimalPlaces(this Decimal input)
        {
            String toDisplay = input.ToString(CultureInfo.InvariantCulture);
            toDisplay = toDisplay.Replace('.', ',');

            if (toDisplay.Contains(','))
            {
                toDisplay = toDisplay.Trim('0');
            }

            return !toDisplay.Contains(',') 
                ? (Byte)0 
                : (Byte)(toDisplay.Length - (toDisplay.IndexOf(',') + 1));
        }


        private static readonly Char[] Digits52 = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
        public static String Base52Encode(this UInt64 value)
        {
            String encoded = "";
            do
            {
                encoded = Digits52[value % 52] + encoded;
            }
            while ((value /= 52) != 0);
            return encoded;
        }

        // Resolves some laziness around anonymnous type, likely error prone
        public static T ValueOf<T>(this Object source, String property) => (T)source.GetType()?.GetProperty(property)?.GetValue(source, null)!;
    }
}