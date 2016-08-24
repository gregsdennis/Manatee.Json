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
 
	File Name:		TypeShemaPropertyValidator.cs
	Namespace:		Manatee.Json.Schema.Validators
	Class Name:		TypeShemaPropertyValidator
	Purpose:		Validates schema with a "type" property.

***************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Manatee.Json.Internal;

namespace Manatee.Json.Schema.Validators
{
	internal class TypeShemaPropertyValidator : IJsonSchemaPropertyValidator
	{
		public bool Applies(JsonSchema schema)
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
