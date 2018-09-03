using System.Collections.Generic;
using Manatee.Json.Internal;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	public class MaximumKeyword : IJsonSchemaKeyword
	{
		public string Name => "maximum";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.All;

		public double Value { get; private set; }

		public MaximumKeyword(double value)
		{
			Value = value;
		}

		public SchemaValidationResults Validate(JsonSchema local, JsonSchema root, JsonValue json)
		{
			if (json.Type != JsonValueType.Number) return SchemaValidationResults.Valid;

			if (json.Number > Value)
			{
				var message = SchemaErrorMessages.Maximum.ResolveTokens(new Dictionary<string, object>
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
			Value = (int) json.Number;
		}
		public JsonValue ToJson(JsonSerializer serializer)
		{
			return Value;
		}
	}
}