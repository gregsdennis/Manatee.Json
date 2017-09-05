using System.Collections.Generic;
using System.Linq;

namespace Manatee.Json.Schema.Validators
{
	internal abstract class ItemsSchemaPropertyValidatorBase<T> : IJsonSchemaPropertyValidator<T>
		where T : IJsonSchema
	{
		protected abstract IJsonSchema GetItems(T schema);
		protected abstract AdditionalItems GetAdditionalItems(T schema);
		
		public bool Applies(T schema, JsonValue json)
		{
			return (GetItems(schema) != null || GetAdditionalItems(schema) != null) &&
			       json.Type == JsonValueType.Array;
		}

		public SchemaValidationResults Validate(T schema, JsonValue json, JsonValue root)
		{
			var errors = new List<SchemaValidationError>();
			var array = json.Array;
			var items = GetItems(schema) as JsonSchemaCollection;
			if (items != null)
			{
				// have array of schemata: validate in sequence
				var i = 0;
				while (i < array.Count && i < items.Count)
				{
					errors.AddRange(items[i].Validate(array[i], root).Errors);
					i++;
				}
				if (i < array.Count && GetAdditionalItems(schema) != null)
					if (Equals(GetAdditionalItems(schema), AdditionalItems.False))
						errors.Add(new SchemaValidationError(string.Empty, "Schema indicates no additional items are allowed."));
					else if (!Equals(GetAdditionalItems(schema), AdditionalItems.True))
						errors.AddRange(array.Skip(i).SelectMany(j => GetAdditionalItems(schema).Definition.Validate(j, root).Errors));
			}
			else if (GetItems(schema) != null)
			{
				// have single schema: validate all against this
				var itemValidations = array.Select(v => GetItems(schema).Validate(v, root));
				errors.AddRange(itemValidations.SelectMany((v, i) => v.Errors.Select(e => e.PrependPropertyName($"[{i}]"))));
			}
			return new SchemaValidationResults(errors);
		}
	}
	
	internal class ItemsSchema04PropertyValidator : ItemsSchemaPropertyValidatorBase<JsonSchema04>
	{
		protected override IJsonSchema GetItems(JsonSchema04 schema)
		{
			return schema.Items;
		}
		protected override AdditionalItems GetAdditionalItems(JsonSchema04 schema)
		{
			return schema.AdditionalItems;
		}
	}
	
	internal class ItemsSchema06PropertyValidator : ItemsSchemaPropertyValidatorBase<JsonSchema06>
	{
		protected override IJsonSchema GetItems(JsonSchema06 schema)
		{
			return schema.Items;
		}
		protected override AdditionalItems GetAdditionalItems(JsonSchema06 schema)
		{
			return schema.AdditionalItems;
		}
	}
}
