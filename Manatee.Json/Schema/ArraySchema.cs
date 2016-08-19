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
 
	File Name:		ArraySchema.cs
	Namespace:		Manatee.Json.Schema
	Class Name:		ArraySchema
	Purpose:		Defines a schema which expects an array.

***************************************************************************************/

using System.Collections.Generic;
using System.Linq;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Defines a schema which expects an array.
	/// </summary>
	public class ArraySchema : JsonSchema
	{
		/// <summary>
		/// Creates a new instance of the <see cref="ArraySchema"/> class.
		/// </summary>
		public ArraySchema() : base(JsonSchemaTypeDefinition.Array) {}

		/// <summary>
		/// Validates a <see cref="JsonValue"/> against the schema.
		/// </summary>
		/// <param name="json">A <see cref="JsonValue"/></param>
		/// <param name="root">The root schema serialized to a <see cref="JsonValue"/>.  Used internally for resolving references.</param>
		/// <returns>True if the <see cref="JsonValue"/> passes validation; otherwise false.</returns>
		public override SchemaValidationResults Validate(JsonValue json, JsonValue root = null)
		{
			if (json.Type != JsonValueType.Array)
				return new SchemaValidationResults(string.Empty, $"Expected: Array; Actual: {json.Type}.");
			var array = json.Array;
			var errors = new List<SchemaValidationError>();
			if (MinItems.HasValue && array.Count < MinItems)
				errors.Add(new SchemaValidationError(string.Empty, $"Expected: >= {MinItems} items; Actual: {array.Count} items."));
			if (MaxItems.HasValue && array.Count > MaxItems)
				errors.Add(new SchemaValidationError(string.Empty, $"Expected: <= {MaxItems} items; Actual: {array.Count} items."));
			if ((UniqueItems ?? false) && (array.Count != array.Distinct().Count()))
				errors.Add(new SchemaValidationError(string.Empty, "Expected unique items; Duplicates were found."));
			if (Items != null)
			{
				var jValue = root ?? ToJson(null);
				var itemValidations = array.Select(v => Items.Validate(v, jValue));

				errors.AddRange(itemValidations.SelectMany((v, i) => v.Errors.Select(e => e.PrependPropertyName($"[{i}]"))));
			}
			return new SchemaValidationResults(errors);
		}
	}
}