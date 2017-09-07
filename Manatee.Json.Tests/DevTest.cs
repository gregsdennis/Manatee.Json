using System;
using System.Collections.Generic;
using Manatee.Json.Schema;
using NUnit.Framework;

namespace Manatee.Json.Tests
{
	[TestFixture]
	public class DevTest
	{
		[Test]
		// TOOD: Add categories to exclude this test.
		[Ignore("This test for development purposes only.")]
		public void Test1()
		{
			var text = "http://www.google.com/file/";
			var uri = new Uri(text);

			Console.WriteLine(uri);
		}

		[Test]
		public void SchemaLayoutTest()
		{
			var schema = new JsonSchema06
				{
					Schema = "http://json-schema.org/draft-04/schema#",
					Id = "http://json-schema.org/draft-04/schema#",
					Title = "Core schema meta-schema",
					Definitions = new JsonSchemaTypeDefinitionCollection
						{
							["schemaArray"] = new JsonSchema06
								{
									Type = JsonSchemaTypeDefinition.Array,
									MinItems = 1,
									Items = JsonSchemaReference.Root
								},
							["nonNegativeInteger"] = new JsonSchema06
								{
									Type = JsonSchemaTypeDefinition.Integer,
									Minimum = 0
								},
							["nonNegativeIntegerDefault0"] = new JsonSchema06
								{
									AllOf = new List<IJsonSchema>
										{
											new JsonSchemaReference(new JsonSchema06(), "#/definitions/positiveInteger"),
											new JsonSchema06 {Default = 0}
										}
								},
							["simpleTypes"] = new JsonSchema06
								{
									Enum = new List<EnumSchemaValue>
										{
											new EnumSchemaValue("array"),
											new EnumSchemaValue("boolean"),
											new EnumSchemaValue("integer"),
											new EnumSchemaValue("null"),
											new EnumSchemaValue("number"),
											new EnumSchemaValue("object"),
											new EnumSchemaValue("string")
										}
								},
							["stringArray"] = new JsonSchema06
								{
									Type = JsonSchemaTypeDefinition.Array,
									Items = new JsonSchema06 {Type = JsonSchemaTypeDefinition.String},
									UniqueItems = true,
									Default = new JsonArray()
								}
						},
					Type = JsonSchemaTypeDefinition.Object,
					Properties = new JsonSchemaPropertyDefinitionCollection
						{
							["id"] = new JsonSchema06
								{
									Type = JsonSchemaTypeDefinition.String,
									Format = StringFormat.UriReference
								},
							["$schema"] = new JsonSchema06
								{
									Type = JsonSchemaTypeDefinition.String,
									Format = StringFormat.Uri
								},
							["$ref"] = new JsonSchema06
								{
									Type = JsonSchemaTypeDefinition.Object,
									Format = StringFormat.UriReference
								},
							["title"] = new JsonSchema06 {Type = JsonSchemaTypeDefinition.String},
							["description"] = new JsonSchema06 {Type = JsonSchemaTypeDefinition.String},
							["default"] = JsonSchema06.Empty,
							["multipleOf"] = new JsonSchema06
								{
									Type = JsonSchemaTypeDefinition.Number,
									ExclusiveMinimum = 0
								},
							["maximum"] = new JsonSchema06 {Type = JsonSchemaTypeDefinition.Number},
							["exclusiveMaximum"] = new JsonSchema06 {Type = JsonSchemaTypeDefinition.Number},
							["minimum"] = new JsonSchema06 {Type = JsonSchemaTypeDefinition.Number},
							["exclusiveMinimum"] = new JsonSchema06 {Type = JsonSchemaTypeDefinition.Number},
							["maxLength"] = new JsonSchemaReference(new JsonSchema06(), "#/definitions/nonNegativeInteger"),
							["minLength"] = new JsonSchemaReference(new JsonSchema06(), "#/definitions/nonNegativeIntegerDefault0"),
							["pattern"] = new JsonSchema06
								{
									Type = JsonSchemaTypeDefinition.String,
									Format = StringFormat.Regex
								},
							["additionalItems"] = JsonSchemaReference.Root,
							["items"] = new JsonSchema06
								{
									AnyOf = new List<IJsonSchema>
										{
											JsonSchemaReference.Root,
											new JsonSchemaReference(new JsonSchema06(), "#/definitions/schemaArray")
										},
									Default = new JsonObject()
								},
							["maxItems"] = new JsonSchemaReference(new JsonSchema06(), "#/definitions/nonNegativeInteger"),
							["minItems"] = new JsonSchemaReference(new JsonSchema06(), "#/definitions/nonNegativeIntegerDefault0"),
							["uniqueItems"] = new JsonSchema06
								{
									Type = JsonSchemaTypeDefinition.Boolean,
									Default = false
								},
							["contains"] = JsonSchemaReference.Root,
							["maxProperties"] = new JsonSchemaReference(new JsonSchema06(), "#/definitions/nonNegativeInteger"),
							["minProperties"] = new JsonSchemaReference(new JsonSchema06(), "#/definitions/nonNegativeIntegerDefault0"),
							["required"] = new JsonSchemaReference(new JsonSchema06(), "#/definitions/stringArray"),
							["additionalProperties"] = JsonSchemaReference.Root,
							["definitions"] = new JsonSchema06
								{
									Type = JsonSchemaTypeDefinition.Object,
									AdditionalProperties = new AdditionalProperties {Definition = JsonSchemaReference.Root},
									Default = new JsonObject()
								},
							["properties"] = new JsonSchema06
								{
									Type = JsonSchemaTypeDefinition.Object,
									AdditionalProperties = new AdditionalProperties {Definition = JsonSchemaReference.Root},
									Default = new JsonObject()
								},
							["patternProperties"] = new JsonSchema06
								{
									Type = JsonSchemaTypeDefinition.Object,
									AdditionalProperties = new AdditionalProperties {Definition = JsonSchemaReference.Root},
									Default = new JsonObject()
								},
							["dependencies"] = new JsonSchema06
								{
									Type = JsonSchemaTypeDefinition.Object,
									AdditionalProperties = new JsonSchema06
										{
											AnyOf = new List<IJsonSchema>
												{
													JsonSchemaReference.Root,
													new JsonSchemaReference(new JsonSchema06(), "#/definitions/stringArray")
												}
										}
								},
							["propertyNames"] = JsonSchemaReference.Root,
							["const"] = JsonSchema06.Empty,
							["enum"] = new JsonSchema06
								{
									Type = JsonSchemaTypeDefinition.Array,
									MinItems = 1,
									UniqueItems = true
								},
							["type"] = new JsonSchema06
								{
									AnyOf = new List<IJsonSchema>
										{
											new JsonSchemaReference(new JsonSchema06(), "#/definitions/simpleTypes"),
											new JsonSchema06
												{
													Type = JsonSchemaTypeDefinition.Array,
													Items = new JsonSchemaReference(new JsonSchema06(), "#/definitions/simpleTypes"),
													MinItems = 1,
													UniqueItems = true
												}
										}
								},
							["format"] = new JsonSchema06 {Type = JsonSchemaTypeDefinition.String},
							["allOf"] = new JsonSchemaReference(new JsonSchema06(), "#/definitions/schemaArray"),
							["anyOf"] = new JsonSchemaReference(new JsonSchema06(), "#/definitions/schemaArray"),
							["oneOf"] = new JsonSchemaReference(new JsonSchema06(), "#/definitions/schemaArray"),
							["not"] = JsonSchemaReference.Root
						},
					Default = new JsonObject()
				};
			Console.WriteLine(schema.ToJson(null).GetIndentedString());
		}
	}
}