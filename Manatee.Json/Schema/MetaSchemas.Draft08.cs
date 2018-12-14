namespace Manatee.Json.Schema
{
	public static partial class MetaSchemas
	{
		/// <summary>
		/// The meta-schema for draft-08.
		/// </summary>
		public static readonly JsonSchema Draft08 =
			new JsonSchema {SupportedVersions = JsonSchemaVersion.Draft08}
				.Schema("http://json-schema.org/draft-08/schema#")
				.Id("http://json-schema.org/draft-08/schema#")
				.RecursiveAnchor(true)
				.Vocabulary(SchemaVocabularies.Core, true)
				.Vocabulary(SchemaVocabularies.Applicator, true)
				.Vocabulary(SchemaVocabularies.Annotation, true)
				.Vocabulary(SchemaVocabularies.Assertion, true)
				.Title("Core schema meta-schema")
				.Property("definitions", new JsonSchema()
					          .Comment("While no longer an official keyword as it is replaced by $defs, this keyword is retained in " +
					                   "the meta-schema to prevent incompatible extensions as it remains in common use.")
					          .Type(JsonSchemaType.Object)
					          .AdditionalProperties(new JsonSchema().RecursiveRefRoot())
					          .Default(new JsonObject()))
				.Property("dependencies", new JsonSchema()
					          .Comment("\"dependencies\" is no longer a keyword, but schema authors should avoid redefining it to " +
					                   "facilitate a smooth transition to \"dependentSchemas\" and \"dependentRequired\"")
					          .Type(JsonSchemaType.Object)
					          .AdditionalProperties(new JsonSchema()
						                                .AnyOf(new JsonSchema().RecursiveRefRoot(),
						                                       new JsonSchema().Ref("#/$defs/stringArray"))))
				.AllOf(new JsonSchema().Ref("http://json-schema.org/draft-08/core"),
				       new JsonSchema().Ref("http://json-schema.org/draft-08/applicator"),
				       new JsonSchema().Ref("http://json-schema.org/draft-08/annotation"),
				       new JsonSchema().Ref("http://json-schema.org/draft-08/assertion"));

		/// <summary>
		/// The core meta-schema for draft-08.
		/// </summary>
		public static readonly JsonSchema Draft08Core =
			new JsonSchema { SupportedVersions = JsonSchemaVersion.Draft08 }
				.Schema("http://json-schema.org/draft-08/schema#")
				.Id("http://json-schema.org/draft-08/core")
				.RecursiveAnchor(true)
				.Vocabulary(SchemaVocabularies.Core, true)
				.Title("Core vocabulary meta-schema")
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
				.Property("$recursiveRef", new JsonSchema()
							  .Type(JsonSchemaType.String)
							  .Format(StringFormat.UriReference))
				.Property("$recursiveAnchor", new JsonSchema()
							  .Const(true)
							  .Default(false))
				.Property("$vocabulary", new JsonSchema()
							  .Type(JsonSchemaType.Object)
							  .PropertyNames(new JsonSchema()
												 .Type(JsonSchemaType.String)
												 .Format(StringFormat.Uri))
							  .AdditionalProperties(new JsonSchema().Type(JsonSchemaType.Boolean)))
				.Property("$comment", new JsonSchema().Type(JsonSchemaType.String))
				.Property("$defs", new JsonSchema()
							  .Type(JsonSchemaType.Object)
							  .AdditionalProperties(new JsonSchema().RecursiveRefRoot())
							  .Default(new JsonObject()));

		/// <summary>
		/// The applicator meta-schema for draft-08.
		/// </summary>
		public static readonly JsonSchema Draft08Applicator =
			new JsonSchema {SupportedVersions = JsonSchemaVersion.Draft08}
				.Schema("http://json-schema.org/draft-08/schema#")
				.Id("https://json-schema.org/draft-08/applicator")
				.RecursiveAnchor(true)
				.Vocabulary(SchemaVocabularies.Core, true)
				.Vocabulary(SchemaVocabularies.Applicator, true)
				.Title("Applicator vocabulary meta-schema")
				.Def("schemaArray", new JsonSchema()
						 .Type(JsonSchemaType.Array)
						 .MinItems(1)
						 .Items(new JsonSchema().RecursiveRefRoot()))
				.Property("additionalItems", new JsonSchema().RecursiveRefRoot())
				.Property("unevaluatedItems", new JsonSchema().RecursiveRefRoot())
				.Property("items", new JsonSchema()
							  .AnyOf(new JsonSchema().RecursiveRefRoot(),
									 new JsonSchema().Ref("#/$defs/schemaArray")))
				.Property("contains", new JsonSchema().RecursiveRefRoot())
				.Property("additionalProperties", new JsonSchema().RecursiveRefRoot())
				.Property("unevaluatedProperties", new JsonSchema()
							  .Type(JsonSchemaType.Object)
							  .AdditionalProperties(new JsonSchema().RecursiveRefRoot()))
				.Property("properties", new JsonSchema()
							  .Type(JsonSchemaType.Object)
							  .AdditionalProperties(new JsonSchema().RecursiveRefRoot())
							  .Default(new JsonObject()))
				.Property("patternProperties", new JsonSchema()
							  .Type(JsonSchemaType.Object)
							  .AdditionalProperties(new JsonSchema().RecursiveRefRoot())
							  .PropertyNames(new JsonSchema().Format(StringFormat.Regex))
							  .Default(new JsonObject()))
				.Property("dependentSchemas", new JsonSchema()
							  .Type(JsonSchemaType.Object)
							  .AdditionalProperties(new JsonSchema().RecursiveRefRoot()))
				.Property("propertyNames", new JsonSchema().RecursiveRefRoot())
				.Property("if", new JsonSchema().RecursiveRefRoot())
				.Property("then", new JsonSchema().RecursiveRefRoot())
				.Property("else", new JsonSchema().RecursiveRefRoot())
				.Property("allOf", new JsonSchema().Ref("#/$defs/schemaArray"))
				.Property("anyOf", new JsonSchema().Ref("#/$defs/schemaArray"))
				.Property("oneOf", new JsonSchema().Ref("#/$defs/schemaArray"))
				.Property("not", new JsonSchema().RecursiveRefRoot());

		/// <summary>
		/// The annotation meta-schema for draft-08.
		/// </summary>
		public static readonly JsonSchema Draft08Annotation =
			new JsonSchema {SupportedVersions = JsonSchemaVersion.Draft08}
				.Schema("http://json-schema.org/draft-08/schema#")
				.Id("http://json-schema.org/draft-08/annotation")
				.Vocabulary(SchemaVocabularies.Core, true)
				.Title("Annotation schema meta-schema")
				.Property("title", new JsonSchema().Type(JsonSchemaType.String))
				.Property("description", new JsonSchema().Type(JsonSchemaType.String))
				.Property("default", true)
				.Property("readOnly", new JsonSchema()
							  .Type(JsonSchemaType.Boolean)
							  .Default(false))
				.Property("examples", new JsonSchema()
							  .Type(JsonSchemaType.Array)
							  .Items(true));

		/// <summary>
		/// The assertion meta-schema for draft-08.
		/// </summary>
		public static readonly JsonSchema Draft08Assertion =
			new JsonSchema {SupportedVersions = JsonSchemaVersion.Draft08}
				.Schema("http://json-schema.org/draft-08/schema#")
				.Id("http://json-schema.org/draft-08/assertion")
				.Vocabulary(SchemaVocabularies.Core, true)
				.Vocabulary(SchemaVocabularies.Applicator, true)
				.RecursiveAnchor(true)
				.Title("Core schema meta-schema")
				.Def("schemaArray", new JsonSchema()
					     .Type(JsonSchemaType.Array)
					     .MinItems(1)
					     .Items(new JsonSchema().RecursiveRefRoot()))
				.Def("nonNegativeInteger", new JsonSchema()
					     .Type(JsonSchemaType.Integer)
					     .Minimum(0))
				.Def("nonNegativeIntegerDefault0", new JsonSchema()
					     .Ref("#/$defs/nonNegativeInteger")
					     .Default(0))
				.Def("simpleTypes", new JsonSchema()
					     .Enum("array", "boolean", "integer", "null", "number", "object", "string"))
				.Def("stringArray", new JsonSchema()
					     .Type(JsonSchemaType.Array)
					     .Items(new JsonSchema().Type(JsonSchemaType.String))
					     .UniqueItems(true)
					     .Default(new JsonArray()))
				.Type(JsonSchemaType.Object | JsonSchemaType.Boolean)
				.Property("multipleOf", new JsonSchema()
					          .Type(JsonSchemaType.Number)
					          .ExclusiveMinimum(0))
				.Property("maximum", new JsonSchema().Type(JsonSchemaType.Number))
				.Property("exclusiveMaximum", new JsonSchema().Type(JsonSchemaType.Number))
				.Property("minimum", new JsonSchema().Type(JsonSchemaType.Number))
				.Property("exclusiveMinimum", new JsonSchema().Type(JsonSchemaType.Number))
				.Property("maxLength", new JsonSchema().Ref("#/$defs/nonNegativeInteger"))
				.Property("minLength", new JsonSchema().Ref("#/$defs/nonNegativeIntegerDefault0"))
				.Property("pattern", new JsonSchema()
					          .Type(JsonSchemaType.String)
					          .Format(StringFormat.Regex))
				.Property("maxItems", new JsonSchema().Ref("#/$defs/nonNegativeInteger"))
				.Property("minItems", new JsonSchema().Ref("#/$defs/nonNegativeIntegerDefault0"))
				.Property("uniqueItems", new JsonSchema()
					          .Type(JsonSchemaType.Boolean)
					          .Default(false))
				.Property("maxContains", new JsonSchema().Ref("#/$defs/nonNegativeInteger"))
				.Property("minContains", new JsonSchema()
					          .Ref("#/$defs/nonNegativeInteger")
					          .Default(1))
				.Property("maxProperties", new JsonSchema().Ref("#/$defs/nonNegativeInteger"))
				.Property("minProperties", new JsonSchema().Ref("#/$defs/nonNegativeIntegerDefault0"))
				.Property("required", new JsonSchema().Ref("#/$defs/stringArray"))
				.Property("const", true)
				.Property("enum", new JsonSchema()
					          .Type(JsonSchemaType.Array)
					          .Items(true)
					          .MinItems(1)
					          .UniqueItems(true))
				.Property("type", new JsonSchema()
					          .AnyOf(new JsonSchema().Ref("#/$defs/simpleTypes"),
					                 new JsonSchema()
						                 .Type(JsonSchemaType.Array)
						                 .Items(new JsonSchema().Ref("#/$defs/simpleTypes"))
						                 .MinItems(1)
						                 .UniqueItems(true)))
				.Property("format", new JsonSchema().Type(JsonSchemaType.String));
	}
}