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
using Manatee.Json.Serialization;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Provides base functionality for the basic <see cref="IJsonSchema"/> implementations.S
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
								Definition = new ArraySchema {MinItems = 1, Items = JsonSchemaReference.Root},
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
										JsonSchemaTypeDefinition.CreateEnumValue("array"),
										JsonSchemaTypeDefinition.CreateEnumValue("boolean"),
										JsonSchemaTypeDefinition.CreateEnumValue("integer"),
										JsonSchemaTypeDefinition.CreateEnumValue("null"),
										JsonSchemaTypeDefinition.CreateEnumValue("number"),
										JsonSchemaTypeDefinition.CreateEnumValue("object"),
										JsonSchemaTypeDefinition.CreateEnumValue("string")
									}}
							},
						new JsonSchemaTypeDefinition("stringArray")
							{
								Definition = new ArraySchema {Items = new StringSchema(), MinItems = 1, UniqueItems = true}
							}
					},
				Properties = new JsonSchemaPropertyDefinitionCollection
					{
						new JsonSchemaPropertyDefinition("id") {Type = new StringSchema {Format = StringFormat.Uri}},
						new JsonSchemaPropertyDefinition("$schema") {Type = new StringSchema {Format = StringFormat.Uri}},
						new JsonSchemaPropertyDefinition("title") {Type = new StringSchema()},
						new JsonSchemaPropertyDefinition("description") {Type = new StringSchema()},
						new JsonSchemaPropertyDefinition("default") {Type = Empty},
						new JsonSchemaPropertyDefinition("multipleOf") {Type = new NumberSchema {Minimum = 0, ExclusiveMinimum = true}},
						new JsonSchemaPropertyDefinition("maximum") {Type = new NumberSchema()},
						new JsonSchemaPropertyDefinition("exclusiveMaximum") {Type = new BooleanSchema {Default = false}},
						new JsonSchemaPropertyDefinition("minimum") {Type = new NumberSchema()},
						new JsonSchemaPropertyDefinition("exclusiveMinimum") {Type = new BooleanSchema {Default = false}},
						new JsonSchemaPropertyDefinition("maxLength") {Type = new JsonSchemaReference("#/definitions/positiveInteger")},
						new JsonSchemaPropertyDefinition("minLength") {Type = new JsonSchemaReference("#/definitions/positiveIntegerDefault0")},
						new JsonSchemaPropertyDefinition("pattern") {Type = new StringSchema {Format = StringFormat.Regex}},
						new JsonSchemaPropertyDefinition("additionalItems") {Type = new AnyOfSchema
							{
								Options = new List<IJsonSchema> {new BooleanSchema(), JsonSchemaReference.Root},
								Default = new JsonObject()
							}},
						new JsonSchemaPropertyDefinition("items") {Type = new AnyOfSchema
							{
								Options = new List<IJsonSchema> {JsonSchemaReference.Root, new JsonSchemaReference("#/definitions/schemaArray")},
								Default = new JsonObject()
							}},
						new JsonSchemaPropertyDefinition("maxItems") {Type = new JsonSchemaReference("#/definitions/positiveInteger")},
						new JsonSchemaPropertyDefinition("minItems") {Type = new JsonSchemaReference("#/definitions/positiveIntegerDefault0")},
						new JsonSchemaPropertyDefinition("uniqueItems") {Type = new BooleanSchema {Default = false}},
						new JsonSchemaPropertyDefinition("maxProperties") {Type = new JsonSchemaReference("#/definitions/positiveInteger")},
						new JsonSchemaPropertyDefinition("minProperties") {Type = new JsonSchemaReference("#/definitions/positiveIntegerDefault0")},
						new JsonSchemaPropertyDefinition("required") {Type = new JsonSchemaReference("#/definitions/stringArray")},
						new JsonSchemaPropertyDefinition("additionalProperties") {Type = new AnyOfSchema
							{
								Options = new List<IJsonSchema> {new BooleanSchema(), JsonSchemaReference.Root},
								Default = new JsonObject()
							}},
						new JsonSchemaPropertyDefinition("definitions") {Type = new ObjectSchema
							{
								AdditionalProperties = new AdditionalProperties {Definition = JsonSchemaReference.Root},
								Default = new JsonObject()
							}},
						new JsonSchemaPropertyDefinition("properties") {Type = new ObjectSchema
							{
								AdditionalProperties = new AdditionalProperties {Definition = JsonSchemaReference.Root},
								Default = new JsonObject()
							}},
						new JsonSchemaPropertyDefinition("patternProperties") {Type = new ObjectSchema
							{
								AdditionalProperties = new AdditionalProperties {Definition = JsonSchemaReference.Root},
								Default = new JsonObject()
							}},
						new JsonSchemaPropertyDefinition("dependencies") {Type = new ObjectSchema
							{
								AdditionalProperties = new AdditionalProperties
									{
										Definition =  new AnyOfSchema
											{
												Options = new List<IJsonSchema>
													{
														JsonSchemaReference.Root,
														new JsonSchemaReference("#/definitions/stringArray")
													}
											}
									}
							}},
						new JsonSchemaPropertyDefinition("enum") {Type = new ArraySchema {MinItems = 1, UniqueItems = true}},
						new JsonSchemaPropertyDefinition("type") {Type = new AnyOfSchema
							{
								Options = new List<IJsonSchema>
									{
										new JsonSchemaReference("#/definitions/simpleTypes"),
										new ArraySchema {Items = new JsonSchemaReference("#/definitions/simpleTypes"), MinItems = 1, UniqueItems = true}
									}
							}},
						new JsonSchemaPropertyDefinition("allOf") {Type = new JsonSchemaReference("#/definitions/schemaArray")},
						new JsonSchemaPropertyDefinition("anyOf") {Type = new JsonSchemaReference("#/definitions/schemaArray")},
						new JsonSchemaPropertyDefinition("oneOf") {Type = new JsonSchemaReference("#/definitions/schemaArray")},
						new JsonSchemaPropertyDefinition("not") {Type = JsonSchemaReference.Root},
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
		/// <summary>
		/// Used to specify which this schema defines.
		/// </summary>
		public string Id { get; set; }
		/// <summary>
		/// Defines a title for this schema.
		/// </summary>
		public string Title { get; set; }
		/// <summary>
		/// Defines a description for this schema.
		/// </summary>
		public string Description { get; set; }

		internal JsonSchema() {}
		/// <summary>
		/// Creates a new instance of the indicated type.
		/// </summary>
		/// <param name="type">The JSON Schema type which defines this schema.</param>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="type"/> is null.</exception>
		protected JsonSchema(JsonSchemaTypeDefinition type)
		{
			if (type == null)
				throw new ArgumentNullException(nameof(type));
			Type = type;
		}

		/// <summary>
		/// Validates a <see cref="JsonValue"/> against the schema.
		/// </summary>
		/// <param name="json">A <see cref="JsonValue"/></param>
		/// <param name="root">The root schema serialized to a <see cref="JsonValue"/>.  Used internally for resolving references.</param>
		/// <returns>True if the <see cref="JsonValue"/> passes validation; otherwise false.</returns>
		public virtual SchemaValidationResults Validate(JsonValue json, JsonValue root = null)
		{
			return new SchemaValidationResults();
		}
		/// <summary>
		/// Builds an object from a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="json">The <see cref="JsonValue"/> representation of the object.</param>
		/// <param name="serializer">The <see cref="JsonSerializer"/> instance to use for additional
		/// serialization of values.</param>
		public virtual void FromJson(JsonValue json, JsonSerializer serializer)
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
		/// Converts an object to a <see cref="JsonValue"/>.
		/// </summary>
		/// <param name="serializer">The <see cref="JsonSerializer"/> instance to use for additional
		/// serialization of values.</param>
		/// <returns>The <see cref="JsonValue"/> representation of the object.</returns>
		public virtual JsonValue ToJson(JsonSerializer serializer)
		{
			var json = new JsonObject();
			if (Type != null) json["type"] = Type.Name;
			if (Default != null) json["default"] = Default;
			return json;
		}
		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <returns>
		/// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
		/// </returns>
		/// <param name="other">An object to compare with this object.</param>
		public virtual bool Equals(IJsonSchema other)
		{
			var schema = other as JsonSchema;
			return (schema != null) && (schema.Type == Type);
		}
	}
}