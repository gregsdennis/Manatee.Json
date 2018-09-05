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

		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			if (context.Instance.Type != JsonValueType.Number) return SchemaValidationResults.Valid;

			if ((decimal)context.Instance.Number % (decimal?) Value != 0)
			{
				var message = SchemaErrorMessages.MultipleOf.ResolveTokens(new Dictionary<string, object>
					{
						["multipleOf"] = Value,
						["actual"] = context.Instance.Number % Value,
						["value"] = context.Instance
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