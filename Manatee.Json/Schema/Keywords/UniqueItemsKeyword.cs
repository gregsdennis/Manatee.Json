using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Internal;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	public class UniqueItemsKeyword : IJsonSchemaKeyword
	{
		public virtual string Name => "uniqueItems";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.Draft06 | JsonSchemaVersion.Draft07 | JsonSchemaVersion.Draft08;
		public int ValidationSequence => 1;

		public bool Value { get; private set; }

		public UniqueItemsKeyword(bool value = true)
		{
			Value = value;
		}

		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			if (context.Instance.Type != JsonValueType.Array) return SchemaValidationResults.Valid;
			if (context.Instance.Array.Distinct().Count() == context.Instance.Array.Count) return SchemaValidationResults.Valid;

			return new SchemaValidationResults(Name, SchemaErrorMessages.UniqueItems.ResolveTokens(new Dictionary<string, object>
				{
					["value"] = context.Instance
				}));
		}
		public void FromJson(JsonValue json, JsonSerializer serializer)
		{
			Value = json.Boolean;
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			return Value;
		}
	}
}