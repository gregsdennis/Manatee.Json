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
 
	File Name:		NumberSchema.cs
	Namespace:		Manatee.Json.Schema
	Class Name:		NumberSchema
	Purpose:		Defines a schema which expects a number.

***************************************************************************************/

using System.Collections.Generic;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Defines a schema which expects a number.
	/// </summary>
	public class NumberSchema : JsonSchema
	{

		/// <summary>
		/// Creates a new instance of the <see cref="NumberSchema"/> class.
		/// </summary>
		public NumberSchema() : base(JsonSchemaTypeDefinition.Number) { }

		/// <summary>
		/// Validates a <see cref="JsonValue"/> against the schema.
		/// </summary>
		/// <param name="json">A <see cref="JsonValue"/></param>
		/// <param name="root">The root schema serialized to a <see cref="JsonValue"/>.  Used internally for resolving references.</param>
		/// <returns>True if the <see cref="JsonValue"/> passes validation; otherwise false.</returns>
		public override SchemaValidationResults Validate(JsonValue json, JsonValue root = null)
		{
			if (json.Type != JsonValueType.Number)
				return new SchemaValidationResults(string.Empty, $"Expected: Integer; Actual: {json.Type}.");
			var number = json.Number;
			var errors = new List<SchemaValidationError>();
			if (ExclusiveMinimum ?? false)
			{
				if (number <= Minimum)
					errors.Add(new SchemaValidationError(string.Empty, $"Expected: > {Minimum}; Actual: {number}."));
			}
			else
			{
				if (number < Minimum)
					errors.Add(new SchemaValidationError(string.Empty, $"Expected: >= {Minimum}; Actual: {number}."));
			}
			if (ExclusiveMaximum ?? false)
			{
				if (number >= Maximum)
					errors.Add(new SchemaValidationError(string.Empty, $"Expected: < {Maximum}; Actual: {number}."));
			}
			else
			{
				if (number > Maximum)
					errors.Add(new SchemaValidationError(string.Empty, $"Expected: <= {Maximum}; Actual: {number}."));
			}
			if (MultipleOf.HasValue && number%MultipleOf != 0)
				errors.Add(new SchemaValidationError(string.Empty, $"Expected: {number}%{MultipleOf}=0; Actual: {number%MultipleOf}."));
			return new SchemaValidationResults(errors);
		}
	}
}