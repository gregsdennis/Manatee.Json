using System.Collections.Generic;
using Manatee.Json.Internal;

namespace Manatee.Json.Schema.Validators
{
	internal abstract class MinPropertiesSchemaPropertyValidatorBase<T> : IJsonSchemaPropertyValidator
		where T : IJsonSchema
	{
		protected abstract uint? GetMinProperties(T schema);
		
		public bool Applies(IJsonSchema schema, JsonValue json)
		{
			return schema is T typed && GetMinProperties(typed).HasValue && json.Type == JsonValueType.Object;
		}
		public SchemaValidationResults Validate(IJsonSchema schema, JsonValue json, JsonValue root)
		{
			var minProperties = GetMinProperties((T) schema);
			if (json.Object.Count < minProperties)
			{
				var message = SchemaErrorMessages.MinProperties.ResolveTokens(new Dictionary<string, object>
					{
						["expected"] = minProperties,
						["actual"] = json.Object.Count
					});
				return new SchemaValidationResults(string.Empty, message);
			}

			return new SchemaValidationResults();
		}
	}
	
	internal class MinPropertiesSchema04PropertyValidator : MinPropertiesSchemaPropertyValidatorBase<JsonSchema04>
	{
		protected override uint? GetMinProperties(JsonSchema04 schema)
		{
			return schema.MinProperties;
		}
	}
	
	internal class MinPropertiesSchema06PropertyValidator : MinPropertiesSchemaPropertyValidatorBase<JsonSchema06>
	{
		protected override uint? GetMinProperties(JsonSchema06 schema)
		{
			return schema.MinProperties;
		}
	}
	
	internal class MinPropertiesSchema07PropertyValidator : MinPropertiesSchemaPropertyValidatorBase<JsonSchema07>
	{
		protected override uint? GetMinProperties(JsonSchema07 schema)
		{
			return schema.MinProperties;
		}
	}
}
