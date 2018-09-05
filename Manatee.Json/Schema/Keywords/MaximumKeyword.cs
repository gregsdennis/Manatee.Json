using System.Collections.Generic;
using Manatee.Json.Internal;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	public class MaximumKeyword : IJsonSchemaKeyword
	{
		public string Name => "maximum";
		public virtual JsonSchemaVersion SupportedVersions { get; } = JsonSchemaVersion.All;
		public int ValidationSequence => 1;

		public double Value { get; private set; }

		public MaximumKeyword(double value)
		{
			Value = value;
		}

		public SchemaValidationResults Validate(SchemaValidationContext context)
		{
			if (context.Instance.Type != JsonValueType.Number) return SchemaValidationResults.Valid;

			if (context.Instance.Number > Value)
			{
				var message = SchemaErrorMessages.Maximum.ResolveTokens(new Dictionary<string, object>
					{
						["expected"] = Value,
						["actual"] = context.Instance.Number,
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