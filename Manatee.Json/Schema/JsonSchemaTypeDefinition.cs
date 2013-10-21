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
 
	File Name:		JsonSchemaTypeDefinition.cs
	Namespace:		Manatee.Json.Schema
	Class Name:		JsonSchemaTypeDefinition
	Purpose:		Defines a single type definition within a schema.

***************************************************************************************/
using System.Linq;
using Manatee.Json.Enumerations;
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Defines a single type definition within a schema.
	/// </summary>
	public class JsonSchemaTypeDefinition : IJsonCompatible
	{
		/// <summary>
		/// Defines the array type.
		/// </summary>
		public static readonly JsonSchemaTypeDefinition Array = new JsonSchemaTypeDefinition("array");
		/// <summary>
		/// Defines the boolean type.
		/// </summary>
		public static readonly JsonSchemaTypeDefinition Boolean = new JsonSchemaTypeDefinition("boolean");
		/// <summary>
		/// Defines the integer type.
		/// </summary>
		public static readonly JsonSchemaTypeDefinition Integer = new JsonSchemaTypeDefinition("integer");
		/// <summary>
		/// Defines the null type.
		/// </summary>
		public static readonly JsonSchemaTypeDefinition Null = new JsonSchemaTypeDefinition("null");
		/// <summary>
		/// Defines the number type.
		/// </summary>
		public static readonly JsonSchemaTypeDefinition Number = new JsonSchemaTypeDefinition("number");
		/// <summary>
		/// Defines the object type.
		/// </summary>
		public static readonly JsonSchemaTypeDefinition Object = new JsonSchemaTypeDefinition("object");
		/// <summary>
		/// Defines the string type.
		/// </summary>
		public static readonly JsonSchemaTypeDefinition String = new JsonSchemaTypeDefinition("string");

		/// <summary>
		/// Defines the name of the type.
		/// </summary>
		public string Name { get; private set; }
		/// <summary>
		/// Defines a schema used to define the type.
		/// </summary>
		public IJsonSchema Definition { get; set; }

		static JsonSchemaTypeDefinition()
		{
			Array.Definition = new ArraySchema();
			Boolean.Definition = new BooleanSchema();
			Integer.Definition = new IntegerSchema();
			Null.Definition = new NullSchema();
			Number.Definition = new NumberSchema();
			Object.Definition = new ObjectSchema();
			String.Definition = new StringSchema();
		}
		internal JsonSchemaTypeDefinition() {}
		/// <summary>
		/// Creates a new instance of the <see cref="JsonSchemaTypeDefinition"/> type.
		/// </summary>
		/// <param name="name">The name of the type.</param>
		public JsonSchemaTypeDefinition(string name)
		{
			Name = name;
		}

		/// <summary>
		/// Builds an object from a JsonValue.
		/// </summary>
		/// <param name="json">The JsonValue representation of the object.</param>
		public void FromJson(JsonValue json)
		{
			if (json.Type == JsonValueType.String)
			{
				Name = json.String;
				return;
			}
			var details = json.Object.First();
			Name = details.Key;
			Definition = JsonSchemaFactory.FromJson(details.Value);
		}
		/// <summary>
		/// Converts an object to a JsonValue.
		/// </summary>
		/// <returns>The JsonValue representation of the object.</returns>
		public JsonValue ToJson()
		{
			if (Definition == null)
				return Name;
			return new JsonObject {{Name, Definition.ToJson()}};
		}
	}
}