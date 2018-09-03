using System.Collections.Generic;
using Manatee.Json.Internal;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	public class MinItemsKeyword : IJsonSchemaKeyword
	{
		public string Name => "minItems";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.All;

		public uint Value { get; private set; }

		public MinItemsKeyword(uint value)
		{
			Value = value;
		}

		public SchemaValidationResults Validate(JsonSchema local, JsonSchema root, JsonValue json)
		{
			if (json.Type != JsonValueType.Array) return SchemaValidationResults.Valid;

			if (json.Array.Count < Value)
			{
				var message = SchemaErrorMessages.MinItems.ResolveTokens(new Dictionary<string, object>
					{
						["expected"] = Value,
						["actual"] = json.Array.Count,
						["value"] = json
					});
				return new SchemaValidationResults(string.Empty, message);
			}

			return SchemaValidationResults.Valid;
		}
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			Value = (uint) json.Number;
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			return Value;
		}
	}
}