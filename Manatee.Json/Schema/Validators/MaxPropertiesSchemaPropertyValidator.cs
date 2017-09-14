namespace Manatee.Json.Schema.Validators
{
	internal abstract class MaxPropertiesSchemaPropertyValidatorBase<T> : IJsonSchemaPropertyValidator<T>
		where T : IJsonSchema
	{
		protected abstract uint? GetMaxProperties(T schema);
		
		public bool Applies(T schema, JsonValue json)
		{
			return GetMaxProperties(schema).HasValue && json.Type == JsonValueType.Object;
		}
		public SchemaValidationResults Validate(T schema, JsonValue json, JsonValue root)
		{
			var maxProperties = GetMaxProperties(schema);
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
}
