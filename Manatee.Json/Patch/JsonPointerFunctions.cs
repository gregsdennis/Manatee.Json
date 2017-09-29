using System;
using System.Linq;
using Manatee.Json.Internal;

namespace Manatee.Json.Patch
{
    internal static class JsonPointerFunctions
    {
        private static readonly ValueTuple<JsonValue, string, int, JsonValue, bool> _empty = (null, null, -1, null, false);
        
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

        public static (JsonValue result, bool success) InsertValue(JsonValue json, string path, JsonValue value)
        {
            if (path == string.Empty) return (value, true);
            var parts = path.Split('/').Skip(1);
            var current = json;
            Func<JsonValue, bool> addValue = null;
            foreach (var part in parts)
            {
                var key = part.UnescapePointer();
                int index;
                if (key.StartsWith("0") && key != "0")
                    index = -1;
                else if (!int.TryParse(part, out index))
                    index = key == "-" ? int.MaxValue : -1;

                if (ReferenceEquals(current, null)) return (null, false);
                
                if (current.Type == JsonValueType.Object)
                {
                    var obj = current.Object;
                    addValue = v =>
                        {
                            obj[key] = v;
                            return true;
                        };

                    current.Object.TryGetValue(key, out current);
                }
                else if (current.Type == JsonValueType.Array)
                {
                    if (index == -1) return (null, false);
                    if (index == int.MaxValue)
                        index = Math.Max(0, current.Array.Count - 1);

                    var array = current.Array;
                    var tempIndex = index;
                    addValue = v =>
                        {
                            if (tempIndex > array.Count) return false;
                            array.Insert(tempIndex, v);
                            return true;
                        };

                    current = index < current.Array.Count
                                  ? current.Array[index]
                                  : null;
                }
                else return (null, false);
            }

            if (addValue == null) return (null, false);
            var success = addValue(value);
            return (success ? json : null, success);
        }
    }
}