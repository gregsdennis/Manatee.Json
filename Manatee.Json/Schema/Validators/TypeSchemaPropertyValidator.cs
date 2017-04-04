using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Internal;

namespace Manatee.Json.Schema.Validators
{
	internal class TypeSchemaPropertyValidator : IJsonSchemaPropertyValidator
	{
		public bool Applies(JsonSchema schema, JsonValue json)
		{
			return schema.Type != null;
		}
		public SchemaValidationResults Validate(JsonSchema schema, JsonValue json, JsonValue root)
		{
			var multitype = schema.Type as JsonSchemaMultiTypeDefinition;
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
					if (Equals(schema.Type, JsonSchemaTypeDefinition.Number)) break;
					if (json.Number.IsInt() && Equals(schema.Type, JsonSchemaTypeDefinition.Integer)) break;
					return new SchemaValidationResults(string.Empty, $"Expected: {schema.Type.Name}; Actual: {json.Type}.");
				case JsonValueType.String:
					if (JsonSchemaTypeDefinition.PrimitiveDefinitions.Any(d => d.Name == json.String)) break;
					if (Equals(schema.Type, JsonSchemaTypeDefinition.String)) break;
					return new SchemaValidationResults(string.Empty, $"Expected: String; Actual: {json.Type}.");
				case JsonValueType.Boolean:
					if (Equals(schema.Type, JsonSchemaTypeDefinition.Boolean)) break;
					return new SchemaValidationResults(string.Empty, $"Expected: Boolean; Actual: {json.Type}.");
				case JsonValueType.Object:
					if (Equals(schema.Type, JsonSchemaTypeDefinition.Object)) break;
					return new SchemaValidationResults(string.Empty, $"Expected: Object; Actual: {json.Type}.");
				case JsonValueType.Array:
					if (Equals(schema.Type, JsonSchemaTypeDefinition.Array)) break;
					return new SchemaValidationResults(string.Empty, $"Expected: Array; Actual: {json.Type}.");
				case JsonValueType.Null:
					if (Equals(schema.Type, JsonSchemaTypeDefinition.Null)) break;
					return new SchemaValidationResults(string.Empty, $"Expected: Null; Actual: {json.Type}.");
			}
			return new SchemaValidationResults();
		}
	}
}
