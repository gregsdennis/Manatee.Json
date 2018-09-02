using System.Collections.Generic;
using Manatee.Json.Internal;

namespace Manatee.Json.Schema.Validators
{
	internal abstract class TypeSchemaPropertyValidatorBase<T> : JsonSchemaPropertyValidator
		where T : JsonSchema
	{
		protected abstract JsonSchemaType GetType(T schema);
		
		public bool Applies(JsonSchema schema, JsonValue json)
		{
			return schema is T typed && GetType(typed) != JsonSchemaType.NotDefined;
		}
		public SchemaValidationResults Validate(JsonSchema schema, JsonValue json, JsonValue root)
		{
			var type = GetType((T)schema);
			bool valid = true;
			switch (json.Type)
			{
				case JsonValueType.Number:
					if (type.HasFlag(JsonSchemaType.Number)) break;
					if (json.Number.IsInt() && type.HasFlag(JsonSchemaType.Integer)) break;
					valid = false;
					break;
				case JsonValueType.String:
					var expected = type.ToJson();
					if (expected.Type == JsonValueType.String && expected == json) break;
					if (expected.Type == JsonValueType.Array && expected.Array.Contains(json)) break;
					if (type.HasFlag(JsonSchemaType.String)) break;
					valid = false;
					break;
				case JsonValueType.Boolean:
					if (type.HasFlag(JsonSchemaType.Boolean)) break;
					valid = false;
					break;
				case JsonValueType.Object:
					if (type.HasFlag(JsonSchemaType.Object)) break;
					valid = false;
					break;
				case JsonValueType.Array:
					if (type.HasFlag(JsonSchemaType.Array)) break;
					valid = false;
					break;
				case JsonValueType.Null:
					if (type.HasFlag(JsonSchemaType.Null)) break;
					valid = false;
					break;
			}

			if (!valid)
			{
				var message = SchemaErrorMessages.Type.ResolveTokens(new Dictionary<string, object>
					{
						["expected"] = type,
						["actual"] = json.Type,
						["value"] = json
				});
				return new SchemaValidationResults(string.Empty, message);
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

	internal class TypeSchema07PropertyValidator : TypeSchemaPropertyValidatorBase<JsonSchema07>
	{
		protected override JsonSchemaType GetType(JsonSchema07 schema)
		{
			return schema.Type;
		}
	}
}
