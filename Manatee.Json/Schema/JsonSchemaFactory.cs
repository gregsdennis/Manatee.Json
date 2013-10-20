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
 
	File Name:		JsonSchemaFactory.cs
	Namespace:		Manatee.Json.Schema
	Class Name:		JsonSchemaFactory
	Purpose:		Defines methods to build schema objects.

***************************************************************************************/
namespace Manatee.Json.Schema
{
	/// <summary>
	/// Defines methods to build schema objects.
	/// </summary>
	public static class JsonSchemaFactory
	{
		/// <summary>
		/// Creates a schema object from its JSON representation.
		/// </summary>
		/// <param name="json">A JSON object.</param>
		/// <returns>A schema object</returns>
		public static IJsonSchema FromJson(JsonValue json)
		{
			if (json == null) return null;
			IJsonSchema schema = new JsonSchema();
			var obj = json.Object;
			if (obj.ContainsKey("type"))
			{
				switch (obj["type"].String)
				{
					case "array":
						schema = new ArraySchema();
						break;
					case "boolean":
						schema = new BooleanSchema();
						break;
					case "integer":
						schema = new IntegerSchema();
						break;
					case "null":
						schema = new NullSchema();
						break;
					case "number":
						schema = new NumberSchema();
						break;
					case "object":
						schema = new ObjectSchema();
						break;
					case "string":
						schema = new StringSchema();
						break;
				}
			}
			else if (obj.ContainsKey("$ref"))
			{
				// if has "$ref" key, select SchemaReference
				schema = new JsonSchemaReference();
			}
			else if (obj.ContainsKey("anyOf"))
			{
				// if has "anyOf" key, select AnyOfSchema
				schema = new AnyOfSchema();
			}
			else if (obj.ContainsKey("allOf"))
			{
				// if has "allOf" key, select AllOfSchema
				schema = new AllOfSchema();
			}
			else if (obj.ContainsKey("oneOf"))
			{
				// if has "oneOf" key, select OneOfSchema
				schema = new OneOfSchema();
			}
			else if (obj.ContainsKey("not"))
			{
				// if has "not" key, select NotSchema
				schema = new NotSchema();
			}
			else if (obj.ContainsKey("enum"))
			{
				// if has "not" key, select NotSchema
				schema = new EnumSchema();
			}
			schema.FromJson(json);
			return schema;
		}
	}
}