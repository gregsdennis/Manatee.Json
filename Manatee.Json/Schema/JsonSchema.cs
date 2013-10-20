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
 
	File Name:		JsonSchema.cs
	Namespace:		Manatee.Json.Schema
	Class Name:		JsonSchema
	Purpose:		Provides base functionality for the basic ISchema implementations.

***************************************************************************************/

using System;
using System.Collections.Generic;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Provides base functionality for the basic ISchema implementations.
	/// </summary>
	public class JsonSchema : IJsonSchema
	{
		/// <summary>
		/// Defines an empty Schema.  Useful for specifying that any schema is valid.
		/// </summary>
		public static readonly JsonSchema Empty = new JsonSchema();
		/// <summary>
		/// Defines the Draft-04 Schema as presented at http://json-schema.org/draft-04/schema#
		/// </summary>
		public static readonly JsonSchema Draft04 = new ObjectSchema
			{
				Id = "http://json-schema.org/draft-04/schema#",
				Schema = "http://json-schema.org/draft-04/schema#",
				Description = "Core schema meta-schema",
				Definitions = new JsonSchemaTypeDefinitionCollection
					{
						new JsonSchemaTypeDefinition("schemaArray")
							{
								Definition = new ArraySchema {MinItems = 1, Items = JsonSchemaReference.Self},
							},
						new JsonSchemaTypeDefinition("positiveInteger")
							{
								Definition = new IntegerSchema {Minimum = 0},
							},
						new JsonSchemaTypeDefinition("positiveIntegerDefault0")
							{
								Definition = new AllOfSchema
									{
										Requirements = new List<IJsonSchema>
											{
												new JsonSchemaReference("#/definitions/positiveInteger"),
												new JsonSchema {Default = 0}
											}
									},
							},
						new JsonSchemaTypeDefinition("simpleTypes")
							{
								Definition = new EnumSchema {Values = new List<JsonSchemaTypeDefinition>
									{
										JsonSchemaTypeDefinition.Array,
										JsonSchemaTypeDefinition.Boolean,
										JsonSchemaTypeDefinition.Integer,
										JsonSchemaTypeDefinition.Null,
										JsonSchemaTypeDefinition.Number,
										JsonSchemaTypeDefinition.Object,
										JsonSchemaTypeDefinition.String
									}}
							},
						new JsonSchemaTypeDefinition("stringArray")
							{
								Definition = new ArraySchema {Items = new StringSchema(), MinItems = 1, UniqueItems = true}
							}
					},
				Properties = new JsonSchemaPropertyDefinitionCollection
					{
						new JsonSchemaPropertyDefinition {Name = "id", Type = new StringSchema {Format = "uri"}},
						new JsonSchemaPropertyDefinition {Name = "$schema", Type = new StringSchema {Format = "uri"}},
						new JsonSchemaPropertyDefinition {Name = "title", Type = new StringSchema()},
						new JsonSchemaPropertyDefinition {Name = "description", Type = new StringSchema()},
						new JsonSchemaPropertyDefinition {Name = "default", Type = Empty},
						new JsonSchemaPropertyDefinition {Name = "multipleOf", Type = new NumberSchema {Minimum = 0, ExclusiveMinimum = true}},
						new JsonSchemaPropertyDefinition {Name = "maximum", Type = new NumberSchema()},
						new JsonSchemaPropertyDefinition {Name = "exclusiveMaximum", Type = new BooleanSchema {Default = false}},
						new JsonSchemaPropertyDefinition {Name = "minimum", Type = new NumberSchema()},
						new JsonSchemaPropertyDefinition {Name = "exclusiveMinimum", Type = new BooleanSchema {Default = false}},
						new JsonSchemaPropertyDefinition {Name = "maxLength", Type = new JsonSchemaReference("#/definitions/positiveInteger")},
						new JsonSchemaPropertyDefinition {Name = "minLength", Type = new JsonSchemaReference("#/definitions/positiveIntegerDefault0")},
						new JsonSchemaPropertyDefinition {Name = "pattern", Type = new StringSchema {Format = "regex"}},
						new JsonSchemaPropertyDefinition {Name = "additionalItems", Type = new AnyOfSchema
							{
								Options = new List<IJsonSchema> {new BooleanSchema(), JsonSchemaReference.Self},
								Default = new JsonObject()
							}},
						new JsonSchemaPropertyDefinition {Name = "items", Type = new AnyOfSchema
							{
								Options = new List<IJsonSchema> {JsonSchemaReference.Self, new JsonSchemaReference("#/definitions/schemaArray")},
								Default = new JsonObject()
							}},
						new JsonSchemaPropertyDefinition {Name = "maxItems", Type = new JsonSchemaReference("#/definitions/positiveInteger")},
						new JsonSchemaPropertyDefinition {Name = "minItems", Type = new JsonSchemaReference("#/definitions/positiveIntegerDefault0")},
						new JsonSchemaPropertyDefinition {Name = "uniqueItems", Type = new BooleanSchema {Default = false}},
						new JsonSchemaPropertyDefinition {Name = "maxProperties", Type = new JsonSchemaReference("#/definitions/positiveInteger")},
						new JsonSchemaPropertyDefinition {Name = "minProperties", Type = new JsonSchemaReference("#/definitions/positiveIntegerDefault0")},
						new JsonSchemaPropertyDefinition {Name = "required", Type = new JsonSchemaReference("#/definitions/stringArray")},
						new JsonSchemaPropertyDefinition {Name = "additionalProperties", Type = new AnyOfSchema
							{
								Options = new List<IJsonSchema> {new BooleanSchema(), JsonSchemaReference.Self},
								Default = new JsonObject()
							}},
						new JsonSchemaPropertyDefinition {Name = "definitions", Type = new ObjectSchema
							{
								AdditionalProperties = JsonSchemaReference.Self,
								Default = new JsonObject()
							}},
						new JsonSchemaPropertyDefinition {Name = "properties", Type = new ObjectSchema
							{
								AdditionalProperties = JsonSchemaReference.Self,
								Default = new JsonObject()
							}},
						new JsonSchemaPropertyDefinition {Name = "patternProperties", Type = new ObjectSchema
							{
								AdditionalProperties = JsonSchemaReference.Self,
								Default = new JsonObject()
							}},
						new JsonSchemaPropertyDefinition {Name = "dependencies", Type = new ObjectSchema
							{
								AdditionalProperties = new AnyOfSchema
									{
										Options = new List<IJsonSchema>
											{
												JsonSchemaReference.Self,
												new JsonSchemaReference("#/definitions/stringArray")
											}
									}
							}},
						new JsonSchemaPropertyDefinition {Name = "enum", Type = new ArraySchema {MinItems = 1, UniqueItems = true}},
						new JsonSchemaPropertyDefinition {Name = "type", Type = new AnyOfSchema
							{
								Options = new List<IJsonSchema>
									{
										new JsonSchemaReference("#/definitions/simpleTypes"),
										new ArraySchema {Items = new JsonSchemaReference("#/definitions/simpleTypes"), MinItems = 1, UniqueItems = true}
									}
							}},
						new JsonSchemaPropertyDefinition {Name = "allOf", Type = new JsonSchemaReference("#/definitions/schemaArray")},
						new JsonSchemaPropertyDefinition {Name = "anyOf", Type = new JsonSchemaReference("#/definitions/schemaArray")},
						new JsonSchemaPropertyDefinition {Name = "oneOf", Type = new JsonSchemaReference("#/definitions/schemaArray")},
						new JsonSchemaPropertyDefinition {Name = "not", Type = JsonSchemaReference.Self},
					},
				Dependencies = new Dictionary<string, IEnumerable<string>>
					{
						{"exclusiveMaximum", new List<string> {"maximum"}},
						{"exclusiveMinimum", new List<string> {"minimum"}}
					},
				Default = new JsonObject()
			};

		/// <summary>
		/// The JSON Schema type which defines this schema.
		/// </summary>
		public JsonSchemaTypeDefinition Type { get; private set; }
		/// <summary>
		/// The default value for this schema.
		/// </summary>
		/// <remarks>
		/// The default value is defined as a JSON value which may need to be deserialized
		/// to a .Net data structure.
		/// </remarks>
		public JsonValue Default { get; set; }

		internal JsonSchema() {}
		/// <summary>
		/// Creates a new instance of the indicated type.
		/// </summary>
		/// <param name="type">The JSON Schema type which defines this schema.</param>
		public JsonSchema(JsonSchemaTypeDefinition type)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			Type = type;
		}

		/// <summary>
		/// Builds an object from a JsonValue.
		/// </summary>
		/// <param name="json">The JsonValue representation of the object.</param>
		public virtual void FromJson(JsonValue json)
		{
			var obj = json.Object;
			if (obj.ContainsKey("type"))
			{
				switch (obj["type"].String)
				{
					case "array":
						Type = JsonSchemaTypeDefinition.Array;
						break;
					case "boolean":
						Type = JsonSchemaTypeDefinition.Boolean;
						break;
					case "integer":
						Type = JsonSchemaTypeDefinition.Integer;
						break;
					case "null":
						Type = JsonSchemaTypeDefinition.Null;
						break;
					case "number":
						Type = JsonSchemaTypeDefinition.Number;
						break;
					case "object":
						Type = JsonSchemaTypeDefinition.Object;
						break;
					case "string":
						Type = JsonSchemaTypeDefinition.String;
						break;
				}
			}
			if (obj.ContainsKey("default")) Default = obj["default"];
		}
		/// <summary>
		/// Converts an object to a JsonValue.
		/// </summary>
		/// <returns>The JsonValue representation of the object.</returns>
		public virtual JsonValue ToJson()
		{
			var json = new JsonObject();
			if (Type != null) json["type"] = Type.Name;
			if (Default != null) json["default"] = Default;
			return json;
		}

	}
}