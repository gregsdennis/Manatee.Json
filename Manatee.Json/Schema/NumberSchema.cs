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
using Manatee.Json.Extensions;

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
		/// Builds an object from a JsonValue.
		/// </summary>
		/// <param name="json">The JsonValue representation of the object.</param>
		public override void FromJson(JsonValue json)
		{
			base.FromJson(json);
			var obj = json.Object;
			Minimum = obj.TryGetNumber("minimum");
			Maximum = obj.TryGetNumber("maximum");
			if (obj.ContainsKey("exclusiveMinimum")) ExclusiveMinimum = obj["exclusiveMinimum"].Boolean;
			if (obj.ContainsKey("exclusiveMaximum")) ExclusiveMaximum = obj["minimum"].Boolean;
		}
		/// <summary>
		/// Converts an object to a JsonValue.
		/// </summary>
		/// <returns>The JsonValue representation of the object.</returns>
		public override JsonValue ToJson()
		{
			var json = base.ToJson().Object;
			if (Minimum.HasValue) json["minimum"] = Minimum;
			if (Maximum.HasValue) json["maximum"] = Maximum;
			if (ExclusiveMinimum) json["exclusiveMinimum"] = ExclusiveMinimum;
			if (ExclusiveMaximum) json["exclusiveMaximum"] = ExclusiveMaximum;
			return json;
		}
	}
}