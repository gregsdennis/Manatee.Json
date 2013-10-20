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
 
	File Name:		EnumSchema.cs
	Namespace:		Manatee.Json.Schema
	Class Name:		EnumSchema
	Purpose:		Defines a schema which expects one of an explicit list of values.

***************************************************************************************/
using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Extensions;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Defines a schema which expects one of an explicit list of values.
	/// </summary>
	public class EnumSchema : IJsonSchema
	{
		/// <summary>
		/// A collection of acceptable values.
		/// </summary>
		public IEnumerable<JsonSchemaTypeDefinition> Values { get; set; }

		/// <summary>
		/// Builds an object from a JsonValue.
		/// </summary>
		/// <param name="json">The JsonValue representation of the object.</param>
		public void FromJson(JsonValue json)
		{
			Values = json.Object["enum"].Array.Select(v =>
				{
					var defn = new JsonSchemaTypeDefinition();
					defn.FromJson(v);
					return defn;
				});
		}
		/// <summary>
		/// Converts an object to a JsonValue.
		/// </summary>
		/// <returns>The JsonValue representation of the object.</returns>
		public JsonValue ToJson()
		{
			return new JsonObject {{"enum", Values.ToJson()}};
		}
	}
}