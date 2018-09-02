using System.Collections.Generic;
using Manatee.Json.Internal;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	public class TypeKeyword : JsonSchemaKeyword
	{
		public string Name => "type";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.All;

		public JsonSchemaType Type { get; private set; }

		public TypeKeyword(JsonSchemaType type)
		{
			Type = type;
		}

		public SchemaValidationResults Validate(JsonSchema local, JsonSchema root, JsonValue json)
		{
			bool valid = true;
			switch (json.Type)
			{
				case JsonValueType.Number:
					if (Type.HasFlag(JsonSchemaType.Number)) break;
					if (json.Number.IsInt() && Type.HasFlag(JsonSchemaType.Integer)) break;
					valid = false;
					break;
				case JsonValueType.String:
					var expected = Type.ToJson();
					if (expected.Type == JsonValueType.String && expected == json) break;
					if (expected.Type == JsonValueType.Array && expected.Array.Contains(json)) break;
					if (Type.HasFlag(JsonSchemaType.String)) break;
					valid = false;
					break;
				case JsonValueType.Boolean:
					if (Type.HasFlag(JsonSchemaType.Boolean)) break;
					valid = false;
					break;
				case JsonValueType.Object:
					if (Type.HasFlag(JsonSchemaType.Object)) break;
					valid = false;
					break;
				case JsonValueType.Array:
					if (Type.HasFlag(JsonSchemaType.Array)) break;
					valid = false;
					break;
				case JsonValueType.Null:
					if (Type.HasFlag(JsonSchemaType.Null)) break;
					valid = false;
					break;
			}

			if (!valid)
			{
				var message = SchemaErrorMessages.Type.ResolveTokens(new Dictionary<string, object>
					{
						["expected"] = Type,
						["actual"] = json.Type,
						["value"] = json
					});
				return new SchemaValidationResults(string.Empty, message);
			}

			return new SchemaValidationResults();
		}

		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			Type = serializer.Deserialize<JsonSchemaType>(json);
		}

		public JsonValue ToJson(JsonSerializer serializer)
		{
			return Type.ToJson();
		}
	}
}