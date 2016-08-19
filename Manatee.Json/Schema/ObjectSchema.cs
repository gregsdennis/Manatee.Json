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
 
	File Name:		ObjectSchema.cs
	Namespace:		Manatee.Json.Schema
	Class Name:		ObjectSchema
	Purpose:		Defines a schema which expects an object.

***************************************************************************************/
using System.Collections.Generic;
using System.Linq;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Defines a schema which expects an object.
	/// </summary>
	public class ObjectSchema : JsonSchema
	{
		/// <summary>
		/// Creates a new instance of the <see cref="ObjectSchema"/> class.
		/// </summary>
		public ObjectSchema()
			: base(JsonSchemaTypeDefinition.Object) {}

		/// <summary>
		/// Validates a <see cref="JsonValue"/> against the schema.
		/// </summary>
		/// <param name="json">A <see cref="JsonValue"/></param>
		/// <param name="root">The root schema serialized to a <see cref="JsonValue"/>.  Used internally for resolving references.</param>
		/// <returns>True if the <see cref="JsonValue"/> passes validation; otherwise false.</returns>
		public override SchemaValidationResults Validate(JsonValue json, JsonValue root = null)
		{
			if (json.Type != JsonValueType.Object)
				return new SchemaValidationResults(string.Empty, $"Expected: Object; Actual: {json.Type}.");
			var obj = json.Object;
			var errors = new List<SchemaValidationError>();
			var jValue = root ?? ToJson(null);
			var properties = Properties ?? new JsonSchemaPropertyDefinitionCollection();
			foreach (var property in properties)
			{
				if (!obj.ContainsKey(property.Name))
				{
					if (property.IsRequired)
						errors.Add(new SchemaValidationError(property.Name, "Required property not found."));
					continue;
				}
				var result = property.Type.Validate(obj[property.Name], jValue);
				if (!result.Valid)
					errors.AddRange(result.Errors.Select(e => e.PrependPropertyName(property.Name)));
			}
			var extraData = obj.Where(kvp => properties.All(p => p.Name != kvp.Key))
			                   .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
			if (AdditionalProperties != null && AdditionalProperties.Equals(AdditionalProperties.False))
			{
				if (PatternProperties != null)
				{
					foreach (var patternProperty in PatternProperties)
					{
						var pattern = patternProperty.Key;
						var schema = patternProperty.Value;
						var matches = extraData.Keys.Where(k => pattern.IsMatch(k));
						foreach (var match in matches)
						{
							var matchErrors = schema.Validate(extraData[match], jValue).Errors;
							errors.AddRange(matchErrors.Select(e => new SchemaValidationError(match, e.Message)));
						}
						extraData = extraData.Where(kvp => !pattern.IsMatch(kvp.Key))
						                     .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
					}
				}
				errors.AddRange(extraData.Keys.Select(k => new SchemaValidationError(k, "Cannot find a match within Properties or PatternProperties.")));
			}
			else if (AdditionalProperties != null)
			{
				var schema = AdditionalProperties.Definition;
				foreach (var key in extraData.Keys)
				{
					var extraErrors = schema.Validate(extraData[key], jValue).Errors;
					errors.AddRange(extraErrors.Select(e => e.PrependPropertyName(key)));
				}
			}
			return new SchemaValidationResults(errors);
		}
	}
}