using System;
using Manatee.Json.Serialization;

namespace Manatee.Json.Patch
{
    public class JsonPatchAction : IJsonSerializable
    {
        public JsonPatchOperation Operation { get; set; }
        public string Path { get; set; }
        public string From { get; set; }
        public JsonValue Value { get; set; }

        internal JsonPatchResult TryApply(JsonValue json)
        {
            switch (Operation)
            {
                case JsonPatchOperation.Add:
                    return _Add(json);
                case JsonPatchOperation.Remove:
                    return _Remove(json);
                case JsonPatchOperation.Replace:
                    return _Replace(json);
                case JsonPatchOperation.Move:
                    return _Move(json);
                case JsonPatchOperation.Copy:
                    return _Copy(json);
                case JsonPatchOperation.Test:
                    return _Test(json);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void FromJson(JsonValue json, JsonSerializer serializer)
        {
            var obj = json.Object;
            Operation = serializer.Deserialize<JsonPatchOperation>(obj["op"]);
            Path = obj.TryGetString("path");
            From = obj.TryGetString("from");
            JsonValue jsonValue;
            obj.TryGetValue("value", out jsonValue);
            Value = jsonValue;

            this.Validate();
        }
        public JsonValue ToJson(JsonSerializer serializer)
        {
            var json = new JsonObject
                {
                    ["op"] = serializer.Serialize(Operation),
                    ["path"] = Path
                };
            if (From != null)
                json["from"] = From;
            if (Value != null)
                json["value"] = Value;

            return json;
        }

        private JsonPatchResult _Add(JsonValue json)
        {
            return _Add(json, Value);
        }

        private JsonPatchResult _Remove(JsonValue json)
        {
            var (target, key, index, value) = JsonPointerFunctions.ResolvePointer(json, Path);
            if (target == null || key == null || index == -1)
                // TODO: What should happen if I can't create the path?
                throw new NotImplementedException();
            
            switch (target.Type)
            {
                case JsonValueType.Object:
                    target.Object.Remove(key);
                    break;
                case JsonValueType.Array:
                    target.Array.RemoveAt(index);
                    break;
                default:
                    return new JsonPatchResult(json, $"Cannot add a value to a '{target.Type}'");
            }

            return new JsonPatchResult(json);
        }

        private JsonPatchResult _Replace(JsonValue json)
        {
            var remove = _Remove(json);
            return remove.Success ? remove : _Add(json);
        }

        private JsonPatchResult _Move(JsonValue json)
        {
            // TODO: This isn't the most efficient way to do this, but it'll get the job done.
            var copy = _Copy(json);
            return !copy.Success ? copy : _Remove(json);
        }

        private JsonPatchResult _Copy(JsonValue json)
        {
            var (_, _, _, value) = JsonPointerFunctions.ResolvePointer(json, From);
            if (value == null) return new JsonPatchResult(json, $"The path '{From}' does not exist.");

            return _Add(json, value);
        }
        
        private JsonPatchResult _Add(JsonValue json, JsonValue value)
        {
            var (target, key, index, _) = JsonPointerFunctions.ResolvePointer(json, Path);
            if (target == null || key == null || index == -1)
                // TODO: What should happen if I can't create the path?
                throw new NotImplementedException();

            switch (target.Type)
            {
                case JsonValueType.Object:
                    target.Object[key] = value;
                    break;
                case JsonValueType.Array:
                    index = Math.Min(target.Array.Count, index);
                    target.Array.Insert(index, value);
                    break;
                default:
                    return new JsonPatchResult(json, $"Cannot add a value to a '{target.Type}'");
            }

            return new JsonPatchResult(json);
        }

        private JsonPatchResult _Test(JsonValue json)
        {
            var (_, _, _, value) = JsonPointerFunctions.ResolvePointer(json, Path);
            
            if (value == null) return new JsonPatchResult(json, $"The path '{Path}' does not exist.");
            if (value != Value) return new JsonPatchResult(json, $"The value at '{Path}' is not the expected value.");
            return new JsonPatchResult(json);
        }
    }
}