using System.Collections.Generic;
using Manatee.Json.Internal;

namespace Manatee.Json.Schema.Validators
{
	internal class MaximumSchema04PropertyValidator : IJsonSchemaPropertyValidator
	{
		public bool Applies(IJsonSchema schema, JsonValue json)
		{
			return schema is JsonSchema04 typed && (typed.Maximum.HasValue || (typed.ExclusiveMaximum ?? false)) &&
			       json.Type == JsonValueType.Number;
		}
		public SchemaValidationResults Validate(IJsonSchema schema, JsonValue json, JsonValue root)
		{
			var typed = (JsonSchema04) schema;
			if (typed.ExclusiveMaximum ?? false)
			{
				if (json.Number >= typed.Maximum)
				{
					var message = SchemaErrorMessages.ExclusiveMaximum.ResolveTokens(new Dictionary<string, object>
						{
							["expected"] = typed.Maximum,
							["value"] = json,
					});
					return new SchemaValidationResults(string.Empty, message);
				}
			}
			else
			{
				if (json.Number > typed.Maximum)
				{
					var message = SchemaErrorMessages.Maximum.ResolveTokens(new Dictionary<string, object>
						{
							["expected"] = typed.Maximum,
							["value"] = json
					});
					return new SchemaValidationResults(string.Empty, message);
				}
			}
			return new SchemaValidationResults();
		}
	}
	
	// TODO: extract a base class for 6/7 
	internal class MaximumSchema06PropertyValidator : IJsonSchemaPropertyValidator
	{
		public bool Applies(IJsonSchema schema, JsonValue json)
		{
			return schema is JsonSchema06 typed && (typed.Maximum.HasValue || typed.ExclusiveMaximum.HasValue) &&
			       json.Type == JsonValueType.Number;
		}
		public SchemaValidationResults Validate(IJsonSchema schema, JsonValue json, JsonValue root)
		{
			var typed = (JsonSchema06) schema;
			var max = typed.ExclusiveMaximum ?? typed.Maximum;
			var template = typed.ExclusiveMaximum.HasValue 
				               ? SchemaErrorMessages.ExclusiveMaximum 
				               : SchemaErrorMessages.Maximum;

			if (json.Number > max || typed.ExclusiveMaximum.HasValue && json.Number >= max)
			{
				var message = template.ResolveTokens(new Dictionary<string, object>
					{
						["expected"] = max,
						["value"] = json
				});
				return new SchemaValidationResults(string.Empty, message);
			}
			
			return new SchemaValidationResults();
		}
	}
	
	internal class MaximumSchema07PropertyValidator : IJsonSchemaPropertyValidator
	{
		public bool Applies(IJsonSchema schema, JsonValue json)
		{
			return schema is JsonSchema07 typed && (typed.Maximum.HasValue || typed.ExclusiveMaximum.HasValue) &&
			       json.Type == JsonValueType.Number;
		}
		public SchemaValidationResults Validate(IJsonSchema schema, JsonValue json, JsonValue root)
		{
			var typed = (JsonSchema07) schema;
			var max = typed.ExclusiveMaximum ?? typed.Maximum;
			var template = typed.ExclusiveMaximum.HasValue
				               ? SchemaErrorMessages.ExclusiveMaximum
				               : SchemaErrorMessages.Maximum;

			if (json.Number > max || typed.ExclusiveMaximum.HasValue && json.Number >= max)
			{
				var message = template.ResolveTokens(new Dictionary<string, object>
					{
						["expected"] = max,
						["value"] = json
				});
				return new SchemaValidationResults(string.Empty, message);
			}

			return new SchemaValidationResults();
		}
	}
}
