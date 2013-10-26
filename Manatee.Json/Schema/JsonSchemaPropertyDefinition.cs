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
 
	File Name:		JsonSchemaPropertyDefinition.cs
	Namespace:		Manatee.Json.Schema
	Class Name:		JsonSchemaPropertyDefinition
	Purpose:		Defines a single property within a schema.

***************************************************************************************/
using System.Linq;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Defines a single property within a schema.
	/// </summary>
	public class JsonSchemaPropertyDefinition : IJsonCompatible
	{
		/// <summary>
		/// Defines the name of the property.
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Defines a schema used to represent the type of this property.
		/// </summary>
		public IJsonSchema Type { get; set; }
		/// <summary>
		/// Defines whether this property is required.
		/// </summary>
		public bool IsRequired { get; set; }

		public JsonSchemaPropertyDefinition()
		{
			Type = JsonSchema.Empty;
		}

		/// <summary>
		/// Builds an object from a JsonValue.
		/// </summary>
		/// <param name="json">The JsonValue representation of the object.</param>
		public void FromJson(JsonValue json)
		{
			var details = json.Object.First();
			Name = details.Key;
			Type = JsonSchemaFactory.FromJson(details.Value);
		}
		/// <summary>
		/// Converts an object to a JsonValue.
		/// </summary>
		/// <returns>The JsonValue representation of the object.</returns>
		public JsonValue ToJson()
		{
			return new JsonObject {{Name, Type.ToJson()}};
		}
	}
}