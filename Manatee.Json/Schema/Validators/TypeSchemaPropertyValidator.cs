using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Internal;

namespace Manatee.Json.Schema.Validators
{
	internal abstract class TypeSchemaPropertyValidatorBase<T> : IJsonSchemaPropertyValidator<T>
		where T : IJsonSchema
	{
		protected abstract JsonSchemaTypeDefinition GetType(T schema);
		
		public bool Applies(T schema, JsonValue json)
		{
			return GetType(schema) != null;
		}
		public SchemaValidationResults Validate(T schema, JsonValue json, JsonValue root)
		{
			var multitype = GetType(schema) as JsonSchemaMultiTypeDefinition;
			if (multitype != null)
			{
				var schemata = multitype.Defintions.Select(d => d.Definition);
				var validations = schemata.Select(s => s.Validate(json)).ToList();
				if (validations.Any(v => v.Valid))
					return new SchemaValidationResults();
				var errors = new List<SchemaValidationError>
					{
						new SchemaValidationError(string.Empty, "None of the indicated schemas passed.")
					};
				errors.AddRange(validations.SelectMany(v => v.Errors));
				return new SchemaValidationResults(errors);
			}
			switch (json.Type)
			{
				case JsonValueType.Number:
					if (Equals(GetType(schema), JsonSchemaTypeDefinition.Number)) break;
					if (json.Number.IsInt() && Equals(GetType(schema), JsonSchemaTypeDefinition.Integer)) break;
					return new SchemaValidationResults(string.Empty, $"Expected: {GetType(schema).Name}; Actual: {json.Type}.");
				case JsonValueType.String:
					if (GetType(schema).Name == json.String) break;
					if (Equals(GetType(schema), JsonSchemaTypeDefinition.String)) break;
					return new SchemaValidationResults(string.Empty, $"Expected: {GetType(schema).Name}; Actual: {json.Type}.");
				case JsonValueType.Boolean:
					if (Equals(GetType(schema), JsonSchemaTypeDefinition.Boolean)) break;
					return new SchemaValidationResults(string.Empty, $"Expected: {GetType(schema).Name}; Actual: {json.Type}.");
				case JsonValueType.Object:
					if (Equals(GetType(schema), JsonSchemaTypeDefinition.Object)) break;
					return new SchemaValidationResults(string.Empty, $"Expected: {GetType(schema).Name}; Actual: {json.Type}.");
				case JsonValueType.Array:
					if (Equals(GetType(schema), JsonSchemaTypeDefinition.Array)) break;
					return new SchemaValidationResults(string.Empty, $"Expected: {GetType(schema).Name}; Actual: {json.Type}.");
				case JsonValueType.Null:
					if (Equals(GetType(schema), JsonSchemaTypeDefinition.Null)) break;
					return new SchemaValidationResults(string.Empty, $"Expected: {GetType(schema).Name}; Actual: {json.Type}.");
			}
			return new SchemaValidationResults();
		}
	}

	internal class TypeSchema04PropertyValidator : TypeSchemaPropertyValidatorBase<JsonSchema04>
	{
		protected override JsonSchemaTypeDefinition GetType(JsonSchema04 schema)
		{
			return schema.Type;
		}
	}

	internal class TypeSchema06PropertyValidator : TypeSchemaPropertyValidatorBase<JsonSchema06>
	{
		protected override JsonSchemaTypeDefinition GetType(JsonSchema06 schema)
		{
			return schema.Type;
		}
	}
}
