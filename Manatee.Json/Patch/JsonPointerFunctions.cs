using System;
using System.Linq;
using Manatee.Json.Internal;

namespace Manatee.Json.Patch
{
    internal static class JsonPointerFunctions
    {
        private static readonly ValueTuple<JsonValue, string, int, JsonValue, bool> _empty = (null, null, -1, null, false);
        private static readonly ValueTuple<JsonValue, bool> _empty2 = (null, false);
        
        public static (JsonValue parent, string key, int index, JsonValue target, bool success) ResolvePointer(JsonValue json, string path)
        {
            if (path == string.Empty) return _empty;
            var parts = path.Split('/').Skip(1);
            JsonValue parent = null;
            var current = json;
            string key = null;
            int index = -1;
            foreach (var part in parts)
            {
                key = part.UnescapePointer();
                if (key.StartsWith("0") && key != "0")
                    index = -1;
                else if (!int.TryParse(part, out index))
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

            return (parent, key, index, current, true);
        }

        public static JsonValue RetrieveValue(JsonValue json, string path)
        {
            if (path == string.Empty) return null;
            var parts = path.Split('/').Skip(1);
            var current = json;
            foreach (var part in parts)
            {
                var key = part.UnescapePointer();
                int index;
                if (key.StartsWith("0") && key != "0")
                    index = -1;
                else if (!int.TryParse(part, out index))
                    index = key == "-" ? int.MaxValue : -1;
                if (current.Type == JsonValueType.Object)
                {
                    if (!current.Object.TryGetValue(key, out JsonValue found)) return null;

                    current = found;
                }
                else if (current.Type == JsonValueType.Array)
                {
                    if (index == -1) return null;
                    if (index == int.MaxValue)
                        index = current.Array.Count - 1;
                    else if (current.Array.Count <= index) return null;

                    current = current.Array[index];
                }
                else return null;
            }

            return current;
        }

        public static bool InsertValue(JsonValue json, string path, JsonValue value)
        {
            if (path == string.Empty) return false;
            var parts = path.Split('/').Skip(1);
            var current = json;
            Action<JsonValue> addValue = null;
            foreach (var part in parts)
            {
                var key = part.UnescapePointer();
                int index;
                if (key.StartsWith("0") && key != "0")
                    index = -1;
                else if (!int.TryParse(part, out index))
                    index = key == "-" ? int.MaxValue : -1;
                if (ReferenceEquals(current, null))
                {
                    if (index != -1 && index != int.MaxValue)
                    {
                        current = new JsonArray();
                        addValue?.Invoke(current);
                        var array = current.Array;
                        var tempIndex = index;
                        addValue = v => array.Insert(tempIndex, v);
                    }
                    else
                    {
                        current = new JsonObject();
                        addValue?.Invoke(current);
                        var obj = current.Object;
                        addValue = v => obj[key] = v;
                    }
                    current = null; // from here on out we need to stay in this clause
                }
                else if (current.Type == JsonValueType.Object)
                {
                    if (current.Object.TryGetValue(key, out JsonValue found))
                    {
                        current = found;
                        continue;
                    }

                    var obj = current.Object;
                    addValue = v => obj[key] = v;
                }
                else if (current.Type == JsonValueType.Array)
                {
                    if (index == -1) return false;
                    if (index == int.MaxValue)
                        index = current.Array.Count - 1;
                    else if (index < current.Array.Count)
                    {
                        current = current.Array[index];
                        continue;
                    }
                    
                    var array = current.Array;
                    var tempIndex = index;
                    addValue = v => array.Insert(tempIndex, v);
                }
                else return false;
            }

            if (addValue == null) return false;
            addValue(value);
            return true;
        }
    }
}