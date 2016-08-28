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
 
	File Name:		MinLengthSchemaPropertyValidator.cs
	Namespace:		Manatee.Json.Schema.Validators
	Class Name:		MinLengthSchemaPropertyValidator
	Purpose:		Validates schema with a "minLength" property.

***************************************************************************************/
using System.Globalization;

namespace Manatee.Json.Schema.Validators
{
	internal class MinLengthSchemaPropertyValidator : IJsonSchemaPropertyValidator
	{
		public bool Applies(JsonSchema schema, JsonValue json)
		{
			return schema.MinLength.HasValue && json.Type == JsonValueType.String;
		}
		public SchemaValidationResults Validate(JsonSchema schema, JsonValue json, JsonValue root)
		{
			var length = new StringInfo(json.String).LengthInTextElements;
			if (schema.MinLength.HasValue && (length < schema.MinLength))
				return new SchemaValidationResults(string.Empty, $"Expected: length >= {schema.MinLength}; Actual: {length}.");
			return new SchemaValidationResults();
		}
	}
}
