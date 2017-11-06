using Manatee.Json.Internal;

namespace Manatee.Json.Schema.Validators
{
	internal abstract class TypeSchemaPropertyValidatorBase<T> : IJsonSchemaPropertyValidator<T>
		where T : IJsonSchema
	{
		protected abstract JsonSchemaType GetType(T schema);
		
		public bool Applies(T schema, JsonValue json)
		{
			return GetType(schema) != JsonSchemaType.NotDefined;
		}
		public SchemaValidationResults Validate(T schema, JsonValue json, JsonValue root)
		{
			var type = GetType(schema);
			switch (json.Type)
			{
				case JsonValueType.Number:
					if (type.HasFlag(JsonSchemaType.Number)) break;
					if (json.Number.IsInt() && type.HasFlag(JsonSchemaType.Integer)) break;
					return new SchemaValidationResults(string.Empty, $"Expected: {type}; Actual: {json.Type}.");
				case JsonValueType.String:
					var expected = type.ToJson();
					if (expected.Type == JsonValueType.String && expected == json) break;
					if (expected.Type == JsonValueType.Array && expected.Array.Contains(json)) break;
					if (type.HasFlag(JsonSchemaType.String)) break;
					return new SchemaValidationResults(string.Empty, $"Expected: {type}; Actual: {json.Type}.");
				case JsonValueType.Boolean:
					if (type.HasFlag(JsonSchemaType.Boolean)) break;
					return new SchemaValidationResults(string.Empty, $"Expected: {type}; Actual: {json.Type}.");
				case JsonValueType.Object:
					if (type.HasFlag(JsonSchemaType.Object)) break;
					return new SchemaValidationResults(string.Empty, $"Expected: {type}; Actual: {json.Type}.");
				case JsonValueType.Array:
					if (type.HasFlag(JsonSchemaType.Array)) break;
					return new SchemaValidationResults(string.Empty, $"Expected: {type}; Actual: {json.Type}.");
				case JsonValueType.Null:
					if (type.HasFlag(JsonSchemaType.Null)) break;
					return new SchemaValidationResults(string.Empty, $"Expected: {type}; Actual: {json.Type}.");
			}
			return new SchemaValidationResults();
		}
	}

	internal class TypeSchema04PropertyValidator : TypeSchemaPropertyValidatorBase<JsonSchema04>
	{
		protected override JsonSchemaType GetType(JsonSchema04 schema)
		{
			return schema.Type;
		}
	}

	internal class TypeSchema06PropertyValidator : TypeSchemaPropertyValidatorBase<JsonSchema06>
	{
		protected override JsonSchemaType GetType(JsonSchema06 schema)
		{
			return schema.Type;
		}
	}
}
