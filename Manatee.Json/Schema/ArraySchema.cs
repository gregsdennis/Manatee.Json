/***************************************************************************************

	Copyright 2012 Greg Dennis

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
		/// Gets and sets a minimum number of items required for the array.
		/// </summary>
		public uint? MinItems { get; set; }
		/// <summary>
		/// Defines a maximum number of items required for the array.
		/// </summary>
		public uint? MaxItems { get; set; }
		/// <summary>
		/// Defines the schema for the items contained in the array.
		/// </summary>
		public IJsonSchema Items { get; set; }
		/// <summary>
		/// Defines whether the array should contain only unique items.
		/// </summary>
		public bool UniqueItems { get; set; }

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
				return new SchemaValidationResults(string.Empty, string.Format("Expected: Array; Actual: {0}.", json.Type));
			var array = json.Array;
			var errors = new List<SchemaValidationError>();
			if (MinItems.HasValue && array.Count < MinItems)
				errors.Add(new SchemaValidationError(string.Empty, string.Format("Expected: >= {0} items; Actual: {1} items.", MinItems, array.Count)));
			if (MaxItems.HasValue && array.Count <= MaxItems)
				errors.Add(new SchemaValidationError(string.Empty, string.Format("Expected: <= {0} items; Actual: {1} items.", MaxItems, array.Count)));
			if (UniqueItems)
			{
				var grouped = array.GroupBy(v => v);
				if (grouped.Select(g => g.Count()).Max() != 1)
					errors.Add(new SchemaValidationError(string.Empty, "Expected unique items; Duplicates were found."));
			}
			if (Items != null)
			{
				var jValue = root ?? ToJson();
				var itemValidations = array.Select(v => Items.Validate(v, jValue)).Where(r => !r.Valid).ToList();
				if (itemValidations.Any())
					errors.Add(new SchemaValidationError(string.Empty, string.Format("{0} items failed type validation.", itemValidations.Count)));
			}
			return new SchemaValidationResults(errors);
		}
		/// <summary>
		/// Builds an object from a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="json">The <see cref="JsonValue"/> representation of the object.</param>
		public override void FromJson(JsonValue json)
		{
			base.FromJson(json);
			var obj = json.Object;
			MinItems = (uint?)obj.TryGetNumber("minItems");
			MaxItems = (uint?)obj.TryGetNumber("maxItems");
			if (obj.ContainsKey("items")) Items = JsonSchemaFactory.FromJson(obj["items"]);
			if (obj.ContainsKey("uniqueItems")) UniqueItems = obj["uniqueItems"].Boolean;
		}
		/// <summary>
		/// Converts an object to a <see cref="JsonValue"/>.
		/// </summary>
		/// <returns>The <see cref="JsonValue"/> representation of the object.</returns>
		public override JsonValue ToJson()
		{
			var json = base.ToJson().Object;
			if (Items != null) json["items"] = Items.ToJson();
			if (MinItems.HasValue) json["minItems"] = MinItems;
			if (MaxItems.HasValue) json["maxItems"] = MinItems;
			if (UniqueItems) json["uniqueItems"] = UniqueItems;
			return json;
		}
	}
}