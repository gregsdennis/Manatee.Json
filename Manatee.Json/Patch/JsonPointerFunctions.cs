using System;
using System.Linq;

namespace Manatee.Json.Patch
{
    internal static class JsonPointerFunctions
    {
        private static readonly ValueTuple<JsonValue, string, int, JsonValue> _empty = (null, null, -1, null);
        
        public static (JsonValue parent, string key, int index, JsonValue target) ResolvePointer(JsonValue json, string path)
        {
            if (path == string.Empty) return _empty;
            var parts = path.Split('/').Skip(1);
            JsonValue parent = null;
            var current = json;
            string key = null;
            int index = -1;
            foreach (var part in parts)
            {
                key = part;
                if (!int.TryParse(part, out index))
                    index = key == "-" ? int.MaxValue : -1;
                if (current.Type == JsonValueType.Object)
                {
                    if (!current.Object.TryGetValue(key, out JsonValue found)) return _empty;

                    parent = current;
                    current = found;
                }
                else if (current.Type == JsonValueType.Array)
                {
                    if (index == -1) return _empty;
                    if (index == int.MaxValue)
                        index = current.Array.Count - 1;
                    else if (current.Array.Count <= index) return _empty;

                    parent = current;
                    current = current.Array[index];
                }
                else return _empty;
            }

            return (parent, key, index, current);
        }
    }
}