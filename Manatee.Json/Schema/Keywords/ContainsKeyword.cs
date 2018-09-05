using System.Collections.Generic;
using Manatee.Json.Internal;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	public class ContainsKeyword : IJsonSchemaKeyword
	{
		public string Name => "const";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.Draft07 | JsonSchemaVersion.Draft08;
		public int ValidationSequence => 1;

		public JsonValue Value { get; private set; }

		public ContainsKeyword() { }
		public ContainsKeyword(JsonValue value)
		{
			Value = value;
		}

		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			if (context.Instance.Type != JsonValueType.Array) return SchemaValidationResults.Valid;
			if (context.Instance.Array.Contains(Value)) return SchemaValidationResults.Valid;

			return new SchemaValidationResults(Name, SchemaErrorMessages.Contains.ResolveTokens(new Dictionary<string, object>
				{
					["expected"] = Value,
					["value"] = context.Instance
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