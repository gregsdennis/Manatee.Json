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
 
	File Name:		OneOfSchema.cs
	Namespace:		Manatee.Json.Schema
	Class Name:		OneOfSchema
	Purpose:		Used to define a collection of schema conditions, exactly
					one of which must be satisfied.

***************************************************************************************/

using System.Linq;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Used to define a collection of schema conditions, exactly one of which must
	/// be satisfied.
	/// </summary>
	public class OneOfSchema : JsonSchema
	{
		/// <summary>
		/// Validates a <see cref="JsonValue"/> against the schema.
		/// </summary>
		/// <param name="json">A <see cref="JsonValue"/></param>
		/// <param name="root">The root schema serialized to a <see cref="JsonValue"/>.  Used internally for resolving references.</param>
		/// <returns>True if the <see cref="JsonValue"/> passes validation; otherwise false.</returns>
		public override SchemaValidationResults Validate(JsonValue json, JsonValue root = null)
		{
			var jValue = root ?? ToJson(null);
			var errors = OneOf.Select(s => s.Validate(json, jValue)).ToList();
			var validCount = errors.Count(r => r.Valid);
			switch (validCount)
			{
				case 0:
					return new SchemaValidationResults(errors);
				case 1:
					return new SchemaValidationResults();
				default:
					return new SchemaValidationResults(string.Empty, "More than one option was valid.");
			}
		}
	}
}