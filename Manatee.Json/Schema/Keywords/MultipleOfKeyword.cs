using System.Collections.Generic;
using Manatee.Json.Internal;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	public class MultipleOfKeyword : IJsonSchemaKeyword
	{
		public string Name => "multipleOf";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.All;

		public double Value { get; private set; }

		public MultipleOfKeyword(double value)
		{
			Value = value;
		}

		public SchemaValidationResults Validate(JsonSchema local, JsonSchema root, JsonValue json)
		{
			if (json.Type != JsonValueType.Number) return SchemaValidationResults.Valid;

			if ((decimal) json.Number % (decimal?) Value != 0)
			{
				var message = SchemaErrorMessages.MaxItems.ResolveTokens(new Dictionary<string, object>
					{
						["multipleOf"] = Value,
						["actual"] = json.Number % Value,
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