using System.Collections.Generic;
using System.Linq;

namespace Manatee.Json.Schema.Validators
{
	internal class ItemsSchemaPropertyValidator : IJsonSchemaPropertyValidator
	{
		public bool Applies(JsonSchema04 schema, JsonValue json)
		{
			return (schema.Items != null || schema.AdditionalItems != null) &&
			       json.Type == JsonValueType.Array;
		}

		public SchemaValidationResults Validate(JsonSchema04 schema, JsonValue json, JsonValue root)
		{
			var errors = new List<SchemaValidationError>();
			var array = json.Array;
			var items = schema.Items as JsonSchemaCollection;
			if (items != null)
			{
				// have array of schemata: validate in sequence
				var i = 0;
				while (i < array.Count && i < items.Count)
				{
					errors.AddRange(items[i].Validate(array[i], root).Errors);
					i++;
				}
				if (i < array.Count && schema.AdditionalItems != null)
					if (Equals(schema.AdditionalItems, AdditionalItems.False))
						errors.Add(new SchemaValidationError(string.Empty, "Schema indicates no additional items are allowed."));
					else if (!Equals(schema.AdditionalItems, AdditionalItems.True))
						errors.AddRange(array.Skip(i).SelectMany(j => schema.AdditionalItems.Definition.Validate(j, root).Errors));
			}
			else if (schema.Items != null)
			{
				// have single schema: validate all against this
				var itemValidations = array.Select(v => schema.Items.Validate(v, root));
				errors.AddRange(itemValidations.SelectMany((v, i) => v.Errors.Select(e => e.PrependPropertyName($"[{i}]"))));
			}
			return new SchemaValidationResults(errors);
		}
	}
}
