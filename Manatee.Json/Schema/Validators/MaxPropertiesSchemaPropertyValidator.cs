namespace Manatee.Json.Schema.Validators
{
	internal abstract class MaxPropertiesSchemaPropertyValidatorBase<T> : IJsonSchemaPropertyValidator
		where T : IJsonSchema
	{
		protected abstract uint? GetMaxProperties(T schema);
		
		public bool Applies(IJsonSchema schema, JsonValue json)
		{
			return schema is T typed && GetMaxProperties(typed).HasValue && json.Type == JsonValueType.Object;
		}
		public SchemaValidationResults Validate(IJsonSchema schema, JsonValue json, JsonValue root)
		{
			var maxProperties = GetMaxProperties((T)schema);
			return json.Object.Count > maxProperties
					   ? new SchemaValidationResults(string.Empty, $"Expected: <= {maxProperties} properties; Actual: {json.Object.Count} properties.")
				       : new SchemaValidationResults();
		}
	}
	
	internal class MaxPropertiesSchema04PropertyValidator : MaxPropertiesSchemaPropertyValidatorBase<JsonSchema04>
	{
		protected override uint? GetMaxProperties(JsonSchema04 schema)
		{
			return schema.MaxProperties;
		}
	}
	
	internal class MaxPropertiesSchema06PropertyValidator : MaxPropertiesSchemaPropertyValidatorBase<JsonSchema06>
	{
		protected override uint? GetMaxProperties(JsonSchema06 schema)
		{
			return schema.MaxProperties;
		}
	}
	
	internal class MaxPropertiesSchema07PropertyValidator : MaxPropertiesSchemaPropertyValidatorBase<JsonSchema07>
	{
		protected override uint? GetMaxProperties(JsonSchema07 schema)
		{
			return schema.MaxProperties;
		}
	}
}
