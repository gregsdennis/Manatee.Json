using System.Linq;

namespace Manatee.Json.Schema.Validators
{
	internal abstract class UniqueItemsSchemaPropertyValidatorBase<T> : IJsonSchemaPropertyValidator
		where T : IJsonSchema
	{
		protected abstract bool? GetUniqueItems(T schema);
		
		public bool Applies(IJsonSchema schema, JsonValue json)
		{
			return schema is T typed && (GetUniqueItems(typed) ?? false) && json.Type == JsonValueType.Array;
		}
		public SchemaValidationResults Validate(IJsonSchema schema, JsonValue json, JsonValue root)
		{
			return json.Array.Count != json.Array.Distinct().Count()
				       ? new SchemaValidationResults(string.Empty, "Expected unique items; Duplicates were found.")
				       : new SchemaValidationResults();
		}
	}

	internal class UniqueItemsSchema04PropertyValidator : UniqueItemsSchemaPropertyValidatorBase<JsonSchema04>
	{
		protected override bool? GetUniqueItems(JsonSchema04 schema)
		{
			return schema.UniqueItems;
		}
	}

	internal class UniqueItemsSchema06PropertyValidator : UniqueItemsSchemaPropertyValidatorBase<JsonSchema06>
	{
		protected override bool? GetUniqueItems(JsonSchema06 schema)
		{
			return schema.UniqueItems;
		}
	}

	internal class UniqueItemsSchema07PropertyValidator : UniqueItemsSchemaPropertyValidatorBase<JsonSchema07>
	{
		protected override bool? GetUniqueItems(JsonSchema07 schema)
		{
			return schema.UniqueItems;
		}
	}
}
