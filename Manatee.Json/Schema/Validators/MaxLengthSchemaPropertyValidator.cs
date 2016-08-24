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
 
	File Name:		MaxLengthSchemaPropertyValidator.cs
	Namespace:		Manatee.Json.Schema.Validators
	Class Name:		MaxLengthSchemaPropertyValidator
	Purpose:		Validates schema with a "maxLength" property.

***************************************************************************************/
using System.Globalization;

namespace Manatee.Json.Schema.Validators
{
	internal class MaxLengthSchemaPropertyValidator : IJsonSchemaPropertyValidator
	{
		public bool Applies(JsonSchema schema)
		{
			return schema.MaxLength.HasValue;
		}
		public SchemaValidationResults Validate(JsonSchema schema, JsonValue json, JsonValue root)
		{
			if (json.Type == JsonValueType.String)
			{
				var length = new StringInfo(json.String).LengthInTextElements;
				if (schema.MaxLength.HasValue && (length > schema.MaxLength))
					return new SchemaValidationResults(string.Empty, $"Expected: length <= {schema.MaxLength}; Actual: {length}.");
			}
			return new SchemaValidationResults();
		}
	}
}
