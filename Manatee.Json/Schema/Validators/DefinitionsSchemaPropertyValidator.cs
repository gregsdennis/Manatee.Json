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
 
	File Name:		DefinitionsSchemaPropertyValidator.cs
	Namespace:		Manatee.Json.Schema.Validators
	Class Name:		DefinitionsSchemaPropertyValidator
	Purpose:		Validates schema with a "definitions" property.

***************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manatee.Json.Schema.Validators
{
	internal class DefinitionsSchemaPropertyValidator : IJsonSchemaPropertyValidator
	{
		public bool Applies(JsonSchema schema, JsonValue json)
		{
			return json.Type == JsonValueType.Object && json.Object.ContainsKey("definitions");
		}
		public SchemaValidationResults Validate(JsonSchema schema, JsonValue json, JsonValue root)
		{
			var errors = new List<SchemaValidationError>();
			var definitions = json.Object["definitions"];
			if (definitions.Type != JsonValueType.Object)
				errors.Add(new SchemaValidationError("definitions", "Property 'definitions' must contain an object."));
			foreach (var value in definitions.Object.Values)
			{
				errors.AddRange(JsonSchema.Draft04.Validate(value).Errors);
			}
			return new SchemaValidationResults(errors);
		}
	}
}
