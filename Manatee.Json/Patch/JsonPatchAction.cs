using System;
using Manatee.Json.Pointer;
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

		void IJsonSerializable.FromJson(JsonValue json, JsonSerializer serializer)
		{
			var obj = json.Object;
			Operation = serializer.Deserialize<JsonPatchOperation>(obj["op"]);
			Path = obj.TryGetString("path");
			From = obj.TryGetString("from");
			obj.TryGetValue("value", out var jsonValue);
			Value = jsonValue;

			this.Validate();
		}
		JsonValue IJsonSerializable.ToJson(JsonSerializer serializer)
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
			var (result, success) = JsonPointerFunctions.InsertValue(json, Path, Value);

			if (!success) return new JsonPatchResult(json, "Could not add the value");
			
			return new JsonPatchResult(result);
		}

		private JsonPatchResult _Remove(JsonValue json)
		{
			var (target, key, index, found) = JsonPointerFunctions.ResolvePointer(json, Path);
			if (!found) return new JsonPatchResult(json, $"Path '{Path}' not found.");
			
			switch (target.Type)
			{
				case JsonValueType.Object:
					target.Object.Remove(key);
					break;
				case JsonValueType.Array:
					target.Array.RemoveAt(index);
					break;
				default:
					return new JsonPatchResult(json, $"Cannot remove a value from a '{target.Type}'");
			}

			return new JsonPatchResult(json);
		}

		private JsonPatchResult _Replace(JsonValue json)
		{
			// TODO: This isn't the most efficient way to do this, but it'll get the job done.
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
			var results = JsonPointer.Parse(From).Evaluate(json);
			if (results.Error != null) return new JsonPatchResult(json, results.Error);
			
			var (result, success) = JsonPointerFunctions.InsertValue(json, Path, results.Result);
			if (!success) return new JsonPatchResult(json, "Could not add the value");
			
			return new JsonPatchResult(result);
		}

		private JsonPatchResult _Test(JsonValue json)
		{
			var results = JsonPointer.Parse(Path).Evaluate(json);
			if (results.Error != null) return new JsonPatchResult(json, results.Error);

			if (results.Result != Value) return new JsonPatchResult(json, $"The value at '{Path}' is not the expected value.");
			
			return new JsonPatchResult(json);
		}
	}
}