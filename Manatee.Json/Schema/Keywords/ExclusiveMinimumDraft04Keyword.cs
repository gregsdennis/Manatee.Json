using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Internal;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	public class ExclusiveMinimumDraft04Keyword : IJsonSchemaKeyword
	{
		public string Name => "exclusiveMinimum";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.Draft04;

		public bool Value { get; private set; }

		public ExclusiveMinimumDraft04Keyword(bool value)
		{
			Value = value;
		}

		public SchemaValidationResults Validate(JsonSchema local, JsonSchema root, JsonValue json)
		{
			if (json.Type != JsonValueType.Number) return SchemaValidationResults.Valid;

			var keyword = local.OfType<MinimumKeyword>().FirstOrDefault();
			if (keyword == null) return SchemaValidationResults.Valid;

			if (!Value) return SchemaValidationResults.Valid;

			if (json.Number <= keyword.Value)
			{
				var message = SchemaErrorMessages.ExclusiveMaximum.ResolveTokens(new Dictionary<string, object>
					{
						["expected"] = Value,
						["actual"] = json.Number,
						["value"] = json
					});

				return new SchemaValidationResults(Name, message);
			}

			return SchemaValidationResults.Valid;
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