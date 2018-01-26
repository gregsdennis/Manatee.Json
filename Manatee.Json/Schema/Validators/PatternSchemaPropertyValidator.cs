using System.Text.RegularExpressions;

namespace Manatee.Json.Schema.Validators
{
	internal abstract class PatternSchemaPropertyValidatorBase<T> : IJsonSchemaPropertyValidator
		where T : IJsonSchema
	{
		protected abstract string GetPattern(T schema);
		
		public bool Applies(IJsonSchema schema, JsonValue json)
		{
			return schema is T typed && GetPattern(typed) != null && json.Type == JsonValueType.String;
		}
		public SchemaValidationResults Validate(IJsonSchema schema, JsonValue json, JsonValue root)
		{
			var pattern = GetPattern((T) schema);
			return Regex.IsMatch(json.String, pattern)
				       ? new SchemaValidationResults()
				       : new SchemaValidationResults(string.Empty, $"Value [{json.String}] does not match required Regex pattern [{pattern}].");
		}
	}
	
	internal class PatternSchema04PropertyValidator : PatternSchemaPropertyValidatorBase<JsonSchema04>
	{
		protected override string GetPattern(JsonSchema04 schema)
		{
			return schema.Pattern;
		}
	}
	
	internal class PatternSchema06PropertyValidator : PatternSchemaPropertyValidatorBase<JsonSchema06>
	{
		protected override string GetPattern(JsonSchema06 schema)
		{
			return schema.Pattern;
		}
	}
	
	internal class PatternSchema07PropertyValidator : PatternSchemaPropertyValidatorBase<JsonSchema07>
	{
		protected override string GetPattern(JsonSchema07 schema)
		{
			return schema.Pattern;
		}
	}
}
