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
 
	File Name:		JsonSchemaReference.cs
	Namespace:		Manatee.Json.Schema
	Class Name:		JsonSchemaReference
	Purpose:		Defines a reference to a schema.

***************************************************************************************/
namespace Manatee.Json.Schema
{
	/// <summary>
	/// Defines a reference to a schema.
	/// </summary>
	public class JsonSchemaReference : IJsonSchema
	{
		/// <summary>
		/// Defines a reference to the root schema.
		/// </summary>
		public static readonly JsonSchemaReference Self = new JsonSchemaReference("#");

		private JsonSchema _schema;

		/// <summary>
		/// Defines the reference in respect to the root schema.
		/// </summary>
		public string Reference { get; private set; }
		/// <summary>
		/// Exposes the schema at the references location.
		/// </summary>
		/// <remarks>
		/// The <see cref="Resolve"/> method must first be called.
		/// </remarks>
		public virtual JsonSchema Resolved { get { return _schema; } }

		internal JsonSchemaReference() {}
		/// <summary>
		/// Creates a new instance of the <see cref="JsonSchemaReference"/> class.
		/// </summary>
		/// <param name="reference"></param>
		public JsonSchemaReference(string reference)
		{
			Reference = reference;
		}

		/// <summary>
		/// Resolves the reference in relation to a specific root.
		/// </summary>
		/// <param name="root"></param>
		public void Resolve(JsonSchema root)
		{
			_schema = null;
		}
		/// <summary>
		/// Builds an object from a JsonValue.
		/// </summary>
		/// <param name="json">The JsonValue representation of the object.</param>
		public void FromJson(JsonValue json)
		{
			Reference = json.Object["$ref"].String;
		}
		/// <summary>
		/// Converts an object to a JsonValue.
		/// </summary>
		/// <returns>The JsonValue representation of the object.</returns>
		public JsonValue ToJson()
		{
			return new JsonObject {{"$ref", Reference}};
		}
	}
}