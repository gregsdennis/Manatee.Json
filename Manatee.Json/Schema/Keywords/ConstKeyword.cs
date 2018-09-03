using System.Collections.Generic;
using Manatee.Json.Internal;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	public class ConstKeyword : IJsonSchemaKeyword
	{
		public string Name => "const";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.Draft07 | JsonSchemaVersion.Draft08;

		public JsonValue Value { get; private set; }

		public ConstKeyword(JsonValue value)
		{
			Value = value;
		}

		public SchemaValidationResults Validate(JsonSchema local, JsonSchema root, JsonValue json)
		{
			return json == Value
				       ? SchemaValidationResults.Valid
				       : new SchemaValidationResults(Name, SchemaErrorMessages.Const.ResolveTokens(new Dictionary<string, object>
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