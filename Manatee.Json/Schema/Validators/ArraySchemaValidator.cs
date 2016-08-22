/***************************************************************************************

	Copyright 2016 Greg Dennis

	   Licensed under the Apache License, Version 2.0 (the "License");
	   you may not use this file except in compliance with the License.
	   You may obtain a copy of the License at

		 http://www.apache.org/licenses/LICENSE-2.0

	   Unless required by applicable law or agreed to in writing, software
	   distributed under the License is distributed on an "AS IS" BASIS,
	   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	   See the License for the specific language governing permissions and
	   limitations under the License.
 
	File Name:		ArraySchemaValidator.cs
	Namespace:		Manatee.Json.Schema.Validators
	Class Name:		ArraySchemaValidator
	Purpose:		Validates schema with array-specific properties.

***************************************************************************************/

using System.Collections.Generic;
using System.Linq;

namespace Manatee.Json.Schema.Validators
{
	internal class ArraySchemaValidator : IJsonSchemaValidator
	{
		public bool Applies(JsonSchema schema)
		{
			return Equals(schema.Type, JsonSchemaTypeDefinition.Array) ||
			       schema.Items != null ||
			       schema.MinItems.HasValue ||
			       schema.MaxItems.HasValue ||
			       schema.UniqueItems.HasValue ||
			       schema.AdditionalItems != null;
		}

		public SchemaValidationResults Validate(JsonSchema schema, JsonValue json, JsonValue root)
		{
			if (json.Type != JsonValueType.Array) return new SchemaValidationResults();
			var array = json.Array;
			var errors = new List<SchemaValidationError>();
			if (Equals(schema.Type, JsonSchemaTypeDefinition.Array))
				errors.Add(new SchemaValidationError(string.Empty, $"Expected: Array; Actual: {json.Type}."));
			if (schema.MinItems.HasValue && array.Count < schema.MinItems)
				errors.Add(new SchemaValidationError(string.Empty, $"Expected: >= {schema.MinItems} items; Actual: {array.Count} items."));
			if (schema.MaxItems.HasValue && array.Count > schema.MaxItems)
				errors.Add(new SchemaValidationError(string.Empty, $"Expected: <= {schema.MaxItems} items; Actual: {array.Count} items."));
			if ((schema.UniqueItems ?? false) && (array.Count != array.Distinct().Count()))
				errors.Add(new SchemaValidationError(string.Empty, "Expected unique items; Duplicates were found."));
			if (schema.Items != null)
			{
				var items = schema.Items as JsonSchemaCollection;
				if (items != null)
				{
					// have array of schemata: validate in sequence
					var i = 0;
					while (i < array.Count && i < items.Count)
					{
						errors.AddRange(items[i].Validate(array[i]).Errors);
						i++;
					}
					if (i < array.Count && schema.AdditionalItems != null)
						if (Equals(schema.AdditionalItems, AdditionalItems.False))
							errors.Add(new SchemaValidationError(string.Empty, "Schema indicates no additional items are allowed."));
						else if (!Equals(schema.AdditionalItems, AdditionalItems.True))
							errors.AddRange(array.Skip(i).SelectMany(j => schema.AdditionalItems.Definition.Validate(j, root).Errors));
				}
				else
				{
					// have single schema: validate all against this
					var itemValidations = array.Select(v => schema.Items.Validate(v, root));
					errors.AddRange(itemValidations.SelectMany((v, i) => v.Errors.Select(e => e.PrependPropertyName($"[{i}]"))));
				}
			}
			return new SchemaValidationResults(errors);
		}
	}
}
