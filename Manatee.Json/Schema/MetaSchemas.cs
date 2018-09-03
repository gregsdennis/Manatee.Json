namespace Manatee.Json.Schema
{
	public static class MetaSchemas
	{
		public static readonly JsonSchema Draft04 = new JsonSchema
			{
				new IdKeywordDraft04("http://json-schema.org/draft-04/schema#"),
				new SchemaKeyword("http://json-schema.org/draft-04/schema#"),
				new DescriptionKeyword("Core schema met-schema"),
				new DefinitionsKeyword
					{
						["schemaArray"] = new JsonSchema
							{
								new TypeKeyword(JsonSchemaType.Array),
								new MinItemsKeyword(1),
								new ItemsKeyword{new JsonSchema {RefKeyword.Root}}
							},
						["positiveInteger"] = new JsonSchema
							{
								new TypeKeyword(JsonSchemaType.Integer),
								new MinimumKeyword(0)
							},
						["positiveIntegerDefault0"] = new JsonSchema
							{
								new AllOfKeyword
									{
										new JsonSchema {new RefKeyword("#/definitions/positiveInteger")},
										new JsonSchema {new DefaultKeyword(0)}
									}
							},
						["simpleTypes"] = new JsonSchema {new EnumKeyword {"array", "boolean", "integer", "null", "number", "object", "string"}},
						["stringArray"] = new JsonSchema
							{
								new TypeKeyword(JsonSchemaType.Array),
								new ItemsKeyword{new JsonSchema {new TypeKeyword(JsonSchemaType.String)}},
								new MinItemsKeyword(1),
								new UniqueItemsKeyword()
							}
					},
				new TypeKeyword(JsonSchemaType.Object),
				new PropertiesKeyword
					{
						["id"] = new JsonSchema {new TypeKeyword(JsonSchemaType.String)},
						["$schema"] = new JsonSchema {new TypeKeyword(JsonSchemaType.String)},
						["title"] = new JsonSchema {new TypeKeyword(JsonSchemaType.String)},
						["description"] = new JsonSchema {new TypeKeyword(JsonSchemaType.String)},
						["default"] = new JsonSchema(),
						["multipleOf"] = new JsonSchema
							{
								new TypeKeyword(JsonSchemaType.Number),
								new MinimumKeyword(0),
								new ExclusiveMinimumDraft04Keyword(true)
							},
						["maximum"] = new JsonSchema {new TypeKeyword(JsonSchemaType.Number)},
						["exclusiveMaximum"] = new JsonSchema
							{
								new TypeKeyword(JsonSchemaType.Boolean),
								new DefaultKeyword(false)
							},
						["minimum"] = new JsonSchema {new TypeKeyword(JsonSchemaType.Number)},
						["exclusiveMinimum"] = new JsonSchema
							{
								new TypeKeyword(JsonSchemaType.Boolean),
								new DefaultKeyword(false)
							},
						["maxLength"] = new JsonSchema {new RefKeyword("#/definitions/positiveInteger")},
						["minLength"] = new JsonSchema {new RefKeyword("#/definitions/positiveIntegerDefault0")},
						["pattern"] = new JsonSchema
							{
								new TypeKeyword(JsonSchemaType.String),
								new FormatKeyword(StringFormat.Regex)
							},
						["additionalItems"] = new JsonSchema
							{
								new AnyOfKeyword
									{
										new JsonSchema {new TypeKeyword(JsonSchemaType.Boolean)},
										new JsonSchema {RefKeyword.Root}
									},
								new DefaultKeyword(new JsonObject())
							},
						["items"] = new JsonSchema
							{
								new AnyOfKeyword
									{
										new JsonSchema {RefKeyword.Root},
										new JsonSchema {new RefKeyword("#/definitions/schemaArray")}
									},
								new DefaultKeyword(new JsonObject())
							},
						["maxItems"] = new JsonSchema {new RefKeyword("#/definitions/positiveInteger")},
						["minItems"] = new JsonSchema {new RefKeyword("#/definitions/positiveIntegerDefault0")},
						["required"] = new JsonSchema {new RefKeyword("#/definitions/stringArray")},
						["additionalProperties"] = new JsonSchema
							{
								new AnyOfKeyword
									{
										new JsonSchema {new TypeKeyword(JsonSchemaType.Boolean)},
										new JsonSchema {RefKeyword.Root}
									},
								new DefaultKeyword(new JsonObject())
							},
						["definitions"] = new JsonSchema
							{
								new TypeKeyword(JsonSchemaType.Object),
								new AdditionalPropertiesKeyword(new JsonSchema {RefKeyword.Root}),
								new DefaultKeyword(new JsonObject())
							},
						["properties"] = new JsonSchema
							{
								new TypeKeyword(JsonSchemaType.Object),
								new AdditionalPropertiesKeyword(new JsonSchema {RefKeyword.Root}),
								new DefaultKeyword(new JsonObject())
							},
						["patternProperties"] = new JsonSchema
							{
								new TypeKeyword(JsonSchemaType.Object),
								new AdditionalPropertiesKeyword(new JsonSchema {RefKeyword.Root}),
								new DefaultKeyword(new JsonObject())
							},
						["dependencies"] = new JsonSchema
							{
								new TypeKeyword(JsonSchemaType.Object),
								new AdditionalPropertiesKeyword(new JsonSchema
									{
										new AnyOfKeyword
											{
												new JsonSchema {RefKeyword.Root},
												new JsonSchema {new RefKeyword("#/definitions/stringArray")}
											}
									})
							},
						["enum"] = new JsonSchema
							{
								new TypeKeyword(JsonSchemaType.Array),
								new MinItemsKeyword(1),
								new UniqueItemsKeyword()
							},
						["type"] = new JsonSchema
							{
								new AnyOfKeyword
									{
										new JsonSchema {new RefKeyword("#/definitions/simpleTypes")},
										new JsonSchema
											{
												new TypeKeyword(JsonSchemaType.Array),
												new ItemsKeyword {new JsonSchema {new RefKeyword("#/definitions/simpleTypes")}},
												new MinItemsKeyword(1),
												new UniqueItemsKeyword()
											}
									}
							},
						["format"] = new JsonSchema {new TypeKeyword(JsonSchemaType.String)},
						["allOf"] = new JsonSchema {new RefKeyword("#/definitions/schemaArray")},
						["anyOf"] = new JsonSchema {new RefKeyword("#/definitions/schemaArray")},
						["oneOf"] = new JsonSchema {new RefKeyword("#/definitions/schemaArray")},
						["not"] = new JsonSchema {RefKeyword.Root}
					},
				// TODO: Make this better
				new DependenciesKeyword
					{
						new PropertyDependency("exclusiveMinimum", new[] {"minimum"}),
						new PropertyDependency("exclusiveMaximum", new[] {"maximum"}),
					}
			};

		public static readonly JsonSchema Draft06 =
			new JsonSchema()
				.Id("http://json-schema.org/draft-06/schema#")
				.Schema("http://json-schema.org/draft-06/schema#")
				.Title("Core schema meta-schema")
				.Definition("schemaArray", new JsonSchema()
					            .Type(JsonSchemaType.Array)
					            .MinItems(1)
					            .Items(new JsonSchema().RefRoot()))
				.Definition("nonNegativeInteger", new JsonSchema()
					            .Type(JsonSchemaType.Integer)
					            .Minimum(0))
				.Definition("nonNegativeIntegerDefault0", new JsonSchema()
					            .AllOf(new JsonSchema().Ref("#/definitions/nonNegativeInteger"),
					                   new JsonSchema().Default(0)))
				.Definition("simpleTypes", new JsonSchema()
					            .Enum("array", "boolean", "integer", "null", "number", "object", "string"))
				.Definition("stringArray", new JsonSchema()
					            .Type(JsonSchemaType.Array)
					            .Items(new JsonSchema().Type(JsonSchemaType.String))
					            .UniqueItems(true)
					            .Default(new JsonArray()))
				.Type(JsonSchemaType.Object | JsonSchemaType.Boolean)
				.Property("$id", new JsonSchema()
					          .Type(JsonSchemaType.String)
					          .Format(StringFormat.UriReference))
				.Property("$schema", new JsonSchema()
					          .Type(JsonSchemaType.String)
					          .Format(StringFormat.Uri))
				.Property("$ref", new JsonSchema()
					          .Type(JsonSchemaType.String)
					          .Format(StringFormat.UriReference))
				.Property("title", new JsonSchema().Type(JsonSchemaType.String))
				.Property("description", new JsonSchema().Type(JsonSchemaType.String))
				.Property("default", new JsonSchema().Default(new JsonObject()))
				.Property("examples", new JsonSchema()
					          .Type(JsonSchemaType.Array)
					          .Items(new JsonSchema()))
				.Property("multipleOf", new JsonSchema()
					          .Type(JsonSchemaType.Number)
					          .ExclusiveMinimum(0))
				.Property("maximum", new JsonSchema().Type(JsonSchemaType.Number))
				.Property("exclusiveMaximum", new JsonSchema().Type(JsonSchemaType.Number))
				.Property("minimum", new JsonSchema().Type(JsonSchemaType.Number))
				.Property("exclusiveMinimum", new JsonSchema().Type(JsonSchemaType.Number))
				.Property("maxLength", new JsonSchema().Ref("#/definitions/nonNegativeInteger"))
				.Property("minLength", new JsonSchema().Ref("#/definitions/nonNegativeIntegerDefault0"))
				.Property("pattern", new JsonSchema()
					          .Type(JsonSchemaType.String)
					          .Format(StringFormat.Regex))
				.Property("additionalItems", new JsonSchema().RefRoot())
				.Property("items", new JsonSchema()
					          .AnyOf(new JsonSchema().RefRoot(),
					                 new JsonSchema().Ref("#/definitions/schemaArray"))
					          .Default(new JsonObject()))
				.Property("maxItems", new JsonSchema().Ref("#/definitions/nonNegativeInteger"))
				.Property("minItems", new JsonSchema().Ref("#/definitions/nonNegativeIntegerDefault0"))
				.Property("uniqueItems", new JsonSchema()
					          .Type(JsonSchemaType.Boolean)
					          .Default(false))
				.Property("contains", new JsonSchema().RefRoot())
				.Property("maxProperties", new JsonSchema().Ref("#/definitions/nonNegativeInteger"))
				.Property("minProperties", new JsonSchema().Ref("#/definitions/nonNegativeIntegerDefault0"))
				.Property("required", new JsonSchema().Ref("#/definitions/stringArray"))
				.Property("additionalProperties", new JsonSchema().RefRoot())
				.Property("definitions", new JsonSchema()
					          .Type(JsonSchemaType.Object)
					          .AdditionalProperties(new JsonSchema().RefRoot())
					          .Default(new JsonObject()))
				.Property("properties", new JsonSchema()
					          .Type(JsonSchemaType.Object)
					          .AdditionalProperties(new JsonSchema().RefRoot())
					          .Default(new JsonObject()))
				.Property("patternProperties", new JsonSchema()
					          .Type(JsonSchemaType.Object)
					          .AdditionalProperties(new JsonSchema().RefRoot())
					          .Default(new JsonObject()))
				.Property("dependencies", new JsonSchema()
					          .Type(JsonSchemaType.Object)
					          .AdditionalProperties(new JsonSchema()
						                                .AnyOf(new JsonSchema().RefRoot(),
						                                       new JsonSchema().Ref("#/definitions/stringArray"))))
				.Property("propertyNames", new JsonSchema().RefRoot())
				.Property("const", new JsonSchema())
				.Property("enum", new JsonSchema()
					          .Type(JsonSchemaType.Array)
					          .MinItems(1)
					          .UniqueItems(true))
				.Property("type", new JsonSchema()
					          .AnyOf(new JsonSchema().Ref("#/definitions/simpleTypes"),
					                 new JsonSchema()
						                 .Type(JsonSchemaType.Array)
						                 .Items(new JsonSchema().Ref("#/definitions/simpleTypes"))
						                 .MinItems(1)
						                 .UniqueItems(true)))
				.Property("format", new JsonSchema().Type(JsonSchemaType.String))
				.Property("allOf", new JsonSchema().Ref("#/definitions/schemaArray"))
				.Property("anyOf", new JsonSchema().Ref("#/definitions/schemaArray"))
				.Property("oneOf", new JsonSchema().Ref("#/definitions/schemaArray"))
				.Property("not", new JsonSchema().RefRoot())
				.Default(new JsonObject());

		public static JsonSchema Draft07 = new JsonSchema
			{
				new IdKeyword("http://json-schema.org/draft-07/schema#"),
				new SchemaKeyword("http://json-schema.org/draft-07/schema#"),
				new TypeKeyword(JsonSchemaType.Object | JsonSchemaType.Boolean),
			};

		public static JsonSchema Draft08 = new JsonSchema
			{
				new IdKeyword("http://json-schema.org/draft-08/schema#"),
				new SchemaKeyword("http://json-schema.org/draft-08/schema#"),
				new TypeKeyword(JsonSchemaType.Object | JsonSchemaType.Boolean),
			};
	}
}