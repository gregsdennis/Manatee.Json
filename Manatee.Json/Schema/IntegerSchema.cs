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
 
	File Name:		IntegerSchema.cs
	Namespace:		Manatee.Json.Schema
	Class Name:		IntegerSchema
	Purpose:		Defines a schema which expects an integer.

***************************************************************************************/

using System.Collections.Generic;
using Manatee.Json.Internal;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Defines a schema which expects an integer.
	/// </summary>
	public class IntegerSchema : JsonSchema
	{
		/// <summary>
		/// Creates a new instance of the <see cref="IntegerSchema"/> class.
		/// </summary>
		public IntegerSchema() : base(JsonSchemaTypeDefinition.Integer) {}

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
			if (!number.IsInt()) return new SchemaValidationResults(string.Empty, "Expected: Integer; Actual: Number.");
			var integer = (int) number;
			var errors = new List<SchemaValidationError>();
			if (ExclusiveMinimum ?? false)
			{
				if (integer <= Minimum)
					errors.Add(new SchemaValidationError(string.Empty, $"Expected: > {Minimum}; Actual: {integer}."));
			}
			else
			{
				if (integer < Minimum)
					errors.Add(new SchemaValidationError(string.Empty, $"Expected: >= {Minimum}; Actual: {integer}."));
			}
			if (ExclusiveMaximum ?? false)
			{
				if (integer >= Maximum)
					errors.Add(new SchemaValidationError(string.Empty, $"Expected: < {Maximum}; Actual: {integer}."));
			}
			else
			{
				if (integer > Maximum)
					errors.Add(new SchemaValidationError(string.Empty, $"Expected: <= {Maximum}; Actual: {integer}."));
			}
			if (MultipleOf.HasValue && integer%MultipleOf != 0)
				errors.Add(new SchemaValidationError(string.Empty, $"Expected: {integer}%{MultipleOf}=0; Actual: {integer%MultipleOf}."));
			return new SchemaValidationResults(errors);
		}
	}
}