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
 
	File Name:		NumberSchema.cs
	Namespace:		Manatee.Json.Schema
	Class Name:		NumberSchema
	Purpose:		Defines a schema which expects a number.

***************************************************************************************/

using System.Collections.Generic;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Defines a schema which expects a number.
	/// </summary>
	public class NumberSchema : JsonSchema
	{
		/// <summary>
		/// Defines a minimum acceptable value.
		/// </summary>
		public double? Minimum { get; set; }
		/// <summary>
		/// Defines a maximum acceptable value;
		/// </summary>
		public double? Maximum { get; set; }
		/// <summary>
		/// Defines whether the minimum value is itself acceptable.
		/// </summary>
		public bool ExclusiveMinimum { get; set; }
		/// <summary>
		/// Defines whether the maximum value is itself acceptable.
		/// </summary>
		public bool ExclusiveMaximum { get; set; }

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
			if (ExclusiveMinimum)
			{
				if (number <= Minimum)
					errors.Add(new SchemaValidationError(string.Empty, $"Expected: > {Minimum}; Actual: {number}."));
			}
			else
			{
				if (number < Minimum)
					errors.Add(new SchemaValidationError(string.Empty, $"Expected: >= {Minimum}; Actual: {number}."));
			}
			if (ExclusiveMaximum)
			{
				if (number >= Maximum)
					errors.Add(new SchemaValidationError(string.Empty, $"Expected: < {Maximum}; Actual: {number}."));
			}
			else
			{
				if (number > Maximum)
					errors.Add(new SchemaValidationError(string.Empty, $"Expected: <= {Maximum}; Actual: {number}."));
			}
			return new SchemaValidationResults(errors);
		}
		/// <summary>
		/// Builds an object from a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="json">The <see cref="JsonValue"/> representation of the object.</param>
		/// <param name="serializer">The <see cref="JsonSerializer"/> instance to use for additional
		/// serialization of values.</param>
		public override void FromJson(JsonValue json, JsonSerializer serializer)
		{
			base.FromJson(json, serializer);
			var obj = json.Object;
			Minimum = obj.TryGetNumber("minimum");
			Maximum = obj.TryGetNumber("maximum");
			if (obj.ContainsKey("exclusiveMinimum")) ExclusiveMinimum = obj["exclusiveMinimum"].Boolean;
			if (obj.ContainsKey("exclusiveMaximum")) ExclusiveMaximum = obj["minimum"].Boolean;
		}
		/// <summary>
		/// Converts an object to a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="serializer">The <see cref="JsonSerializer"/> instance to use for additional
		/// serialization of values.</param>
		/// <returns>The <see cref="JsonValue"/> representation of the object.</returns>
		public override JsonValue ToJson(JsonSerializer serializer)
		{
			var json = base.ToJson(serializer).Object;
			if (Minimum.HasValue) json["minimum"] = Minimum;
			if (Maximum.HasValue) json["maximum"] = Maximum;
			if (ExclusiveMinimum) json["exclusiveMinimum"] = ExclusiveMinimum;
			if (ExclusiveMaximum) json["exclusiveMaximum"] = ExclusiveMaximum;
			return json;
		}
		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <returns>
		/// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
		/// </returns>
		/// <param name="other">An object to compare with this object.</param>
		public override bool Equals(IJsonSchema other)
		{
			var schema = other as NumberSchema;
			return base.Equals(schema) &&
				   Minimum == schema.Minimum &&
				   Maximum == schema.Maximum &&
				   ExclusiveMinimum == schema.ExclusiveMinimum &&
				   ExclusiveMaximum == schema.ExclusiveMaximum;
		}
	}
}