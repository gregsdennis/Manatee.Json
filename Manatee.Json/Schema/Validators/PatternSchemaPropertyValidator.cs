using System.Text.RegularExpressions;

namespace Manatee.Json.Schema.Validators
{
	internal abstract class PatternSchemaPropertyValidatorBase<T> : IJsonSchemaPropertyValidator<T>
		where T : IJsonSchema
	{
		protected abstract string GetPattern(T schema);
		
		public bool Applies(T schema, JsonValue json)
		{
			return GetPattern(schema) != null && json.Type == JsonValueType.String;
		}
		public SchemaValidationResults Validate(T schema, JsonValue json, JsonValue root)
		{
			return Regex.IsMatch(json.String, GetPattern(schema))
				       ? new SchemaValidationResults()
				       : new SchemaValidationResults(string.Empty, $"Value [{json.String}] does not match required Regex pattern [{GetPattern(schema)}].");
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
}
