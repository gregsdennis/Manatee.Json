using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Internal;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	public class UniqueItemsKeyword : JsonSchemaKeyword
	{
		public virtual string Name => "uniqueItems";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.Draft06 | JsonSchemaVersion.Draft07 | JsonSchemaVersion.Draft08;

		public bool Value { get; private set; }

		public UniqueItemsKeyword(bool value = true)
		{
			Value = value;
		}

		public SchemaValidationResults Validate(JsonSchema local, JsonSchema root, JsonValue json)
		{
			return json.Type != JsonValueType.Array ||
			       json.Array.Distinct().Count() == json.Array.Count
				       ? SchemaValidationResults.Valid
				       : new SchemaValidationResults(Name, SchemaErrorMessages.UniqueItems.ResolveTokens(new Dictionary<string, object>
					       {
						       ["value"] = json
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