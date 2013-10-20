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
 
	File Name:		AllOfSchema.cs
	Namespace:		Manatee.Json.Schema
	Class Name:		AllOfSchema
	Purpose:		Used to define a collection of schema conditions, all of which
					must be satisfied.

***************************************************************************************/
using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Extensions;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Used to define a collection of schema conditions, all of which must be satisfied.
	/// </summary>
	public class AllOfSchema : IJsonSchema
	{
		/// <summary>
		/// A collection of required schema which must be satisfied.
		/// </summary>
		public IEnumerable<IJsonSchema> Requirements { get; set; }
		/// <summary>
		/// The default value for this schema.
		/// </summary>
		/// <remarks>
		/// The default value is defined as a JSON value which may need to be deserialized
		/// to a .Net data structure.
		/// </remarks>
		public JsonValue Default { get; set; }

		/// <summary>
		/// Builds an object from a JsonValue.
		/// </summary>
		/// <param name="json">The JsonValue representation of the object.</param>
		public void FromJson(JsonValue json)
		{
			var obj = json.Object;
			Requirements = obj["allOf"].Array.Select(JsonSchemaFactory.FromJson);
			if (obj.ContainsKey("default")) Default = obj["default"];
		}
		/// <summary>
		/// Converts an object to a JsonValue.
		/// </summary>
		/// <returns>The JsonValue representation of the object.</returns>
		public JsonValue ToJson()
		{
			var json = new JsonObject {{"allOf", Requirements.ToJson()}};
			if (Default != null) json["default"] = Default;
			return json;
		}
	}
}