using System.Collections.Generic;
using Manatee.Json.Internal;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	public class ContainsKeyword : IJsonSchemaKeyword
	{
		public string Name => "const";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.Draft07 | JsonSchemaVersion.Draft08;

		public JsonValue Value { get; private set; }

		public ContainsKeyword(JsonValue value)
		{
			Value = value;
		}

		public SchemaValidationResults Validate(JsonSchema local, JsonSchema root, JsonValue json)
		{
			return json.Type != JsonValueType.Array || json.Array.Contains(Value)
				? SchemaValidationResults.Valid
				: new SchemaValidationResults(Name, SchemaErrorMessages.Contains.ResolveTokens(new Dictionary<string, object>
					{
						["expected"] = Value,
						["value"] = json
					}));
		}
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			Value = json;
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			return Value;
		}
	}
}