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
 
	File Name:		BooleanSchema.cs
	Namespace:		Manatee.Json.Schema
	Class Name:		BooleanSchema
	Purpose:		Defines a schema which expects a null value.

***************************************************************************************/

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Defines a schema which expects a null value.
	/// </summary>
	public class NullSchema : JsonSchema
	{
		/// <summary>
		/// Creates an instance of the <see cref="NullSchema"/> class.
		/// </summary>
		public NullSchema() : base(JsonSchemaTypeDefinition.Null) {}

		/// <summary>
		/// Validates a <see cref="JsonValue"/> against the schema.
		/// </summary>
		/// <param name="json">A <see cref="JsonValue"/></param>
		/// <param name="root">The root schema serialized to a <see cref="JsonValue"/>.  Used internally for resolving references.</param>
		/// <returns>True if the <see cref="JsonValue"/> passes validation; otherwise false.</returns>
		public override SchemaValidationResults Validate(JsonValue json, JsonValue root = null)
		{
			return json.Type != JsonValueType.Null
				? new SchemaValidationResults(string.Empty, $"Expected: Null; Actual: {json.Type}.")
				: new SchemaValidationResults();
		}
	}
}