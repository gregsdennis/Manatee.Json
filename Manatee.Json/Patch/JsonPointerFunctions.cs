using System.Linq;

namespace Manatee.Json.Patch
{
    internal static class JsonPointerFunctions
    {
        public static (JsonValue parent, string key, int index, JsonValue target) ResolvePointer(JsonValue json, string path)
        {
            if (path == string.Empty) return (null, null, 0, json);
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
                    if (!current.Object.TryGetValue(key, out JsonValue found)) break;

                    parent = current;
                    current = found;
                }
                else if (current.Type == JsonValueType.Array)
                {
                    // TODO: Account for '-' index
                    if (index == -1) break;
                    if (index == int.MaxValue)
                        index = current.Array.Count - 1;
                    else if (current.Array.Count <= index) break;

                    parent = current;
                    current = current.Array[index];
                }
                else break;
            }

            return (parent, key, index, current);
        }
    }
}