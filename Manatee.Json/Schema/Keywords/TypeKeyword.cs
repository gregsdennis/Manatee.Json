using System.Collections.Generic;
using Manatee.Json.Internal;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	public class TypeKeyword : IJsonSchemaKeyword
	{
		public string Name => "type";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.All;

		public JsonSchemaType Type { get; private set; }

		public TypeKeyword(JsonSchemaType type)
		{
			Type = type;
		}

		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			bool valid = true;
			switch (context.Instance.Type)
			{
				case JsonValueType.Number:
					if (Type.HasFlag(JsonSchemaType.Number)) break;
					if (context.Instance.Number.IsInt() && Type.HasFlag(JsonSchemaType.Integer)) break;
					valid = false;
					break;
				case JsonValueType.String:
					var expected = Type.ToJson();
					if (expected.Type == JsonValueType.String && expected == context.Instance) break;
					if (expected.Type == JsonValueType.Array && expected.Array.Contains(context.Instance)) break;
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
						["actual"] = context.Instance.Type,
						["value"] = context.Instance
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