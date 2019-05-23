namespace Manatee.Json.Schema
{
	public static partial class MetaSchemas
	{
		/// <summary>
		/// The meta-schema for 2019-04.
		/// </summary>
		public static readonly JsonSchema Draft2019_04 =
			new JsonSchema {SupportedVersions = JsonSchemaVersion.Draft2019_04}
				.Schema("http://json-schema.org/2019-04/schema#")
				.Id("http://json-schema.org/2019-04/schema#")
				.RecursiveAnchor(true)
				.Vocabulary(SchemaVocabularies.Core, true)
				.Vocabulary(SchemaVocabularies.Applicator, true)
				.Vocabulary(SchemaVocabularies.Validation, true)
				.Vocabulary(SchemaVocabularies.MetaData, true)
				.Vocabulary(SchemaVocabularies.Format, true)
				.Vocabulary(SchemaVocabularies.Content, true)
				.Title("Core schema meta-schema")
				.AllOf(new JsonSchema().Ref("meta/core"),
				       new JsonSchema().Ref("meta/applicator"),
				       new JsonSchema().Ref("meta/validation"),
				       new JsonSchema().Ref("meta/meta-data"),
				       new JsonSchema().Ref("meta/format"),
				       new JsonSchema().Ref("meta/content"))
				.Type(JsonSchemaType.Object | JsonSchemaType.Boolean)
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
						                                       new JsonSchema().Ref("meta/validation#/$defs/stringArray"))));

		/// <summary>
		/// The core meta-schema for 2019-04.
		/// </summary>
		public static readonly JsonSchema Draft2019_04_Core =
			new JsonSchema { SupportedVersions = JsonSchemaVersion.Draft2019_04 }
				.Schema("http://json-schema.org/2019-04/schema#")
				.Id("http://json-schema.org/2019-04/meta/core")
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
					          .Type(JsonSchemaType.Boolean)
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
		/// The applicator meta-schema for 2019-04.
		/// </summary>
		public static readonly JsonSchema Draft2019_04_Applicator =
			new JsonSchema {SupportedVersions = JsonSchemaVersion.Draft2019_04}
				.Schema("http://json-schema.org/2019-04/schema#")
				.Id("https://json-schema.org/2019-04/meta/applicator")
				.RecursiveAnchor(true)
				.Vocabulary(SchemaVocabularies.Core, true)
				.Vocabulary(SchemaVocabularies.Applicator, true)
				.Title("Applicator vocabulary meta-schema")
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
				.Property("not", new JsonSchema().RecursiveRefRoot())
				.Def("schemaArray", new JsonSchema()
					     .Type(JsonSchemaType.Array)
					     .MinItems(1)
					     .Items(new JsonSchema().RecursiveRefRoot()));

		/// <summary>
		/// The annotation meta-schema for 2019-04.
		/// </summary>
		public static readonly JsonSchema Draft2019_04_MetaData =
			new JsonSchema {SupportedVersions = JsonSchemaVersion.Draft2019_04}
				.Schema("http://json-schema.org/2019-04/schema#")
				.Id("http://json-schema.org/2019-04/annotation")
				.Vocabulary(SchemaVocabularies.Core, true)
				.Title("Meta-data vocabulary meta-schema")
				.Property("title", new JsonSchema().Type(JsonSchemaType.String))
				.Property("description", new JsonSchema().Type(JsonSchemaType.String))
				.Property("default", true)
				.Property("deprecated", new JsonSchema()
					          .Type(JsonSchemaType.Boolean)
					          .Default(false))
				.Property("readOnly", new JsonSchema()
							  .Type(JsonSchemaType.Boolean)
							  .Default(false))
				.Property("examples", new JsonSchema()
							  .Type(JsonSchemaType.Array)
							  .Items(true));

		/// <summary>
		/// The assertion meta-schema for 2019-04.
		/// </summary>
		public static readonly JsonSchema Draft2019_04_Validation =
			new JsonSchema {SupportedVersions = JsonSchemaVersion.Draft2019_04}
				.Schema("http://json-schema.org/2019-04/schema#")
				.Id("http://json-schema.org/2019-04/meta/validation")
				.Vocabulary(SchemaVocabularies.Core, true)
				.Vocabulary(SchemaVocabularies.Applicator, true)
				.RecursiveAnchor(true)
				.Title("Validation vocabulary meta-schema")
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
				.Property("dependentRequired", new JsonSchema()
					          .Type(JsonSchemaType.Object)
					          .AdditionalProperties(new JsonSchema().Ref("#/$defs/stringArray")))
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
					     .Default(new JsonArray()));

		/// <summary>
		/// The format meta-schema for 2019-04.
		/// </summary>
		public static readonly JsonSchema Draft2019_04_Format =
			new JsonSchema {SupportedVersions = JsonSchemaVersion.Draft2019_04}
				.Schema("http://json-schema.org/2019-04/schema#")
				.Id("http://json-schema.org/2019-04/meta/format")
				.Vocabulary(SchemaVocabularies.Core, true)
				.Vocabulary(SchemaVocabularies.Format, true)
				.RecursiveAnchor(true)
				.Title("Format vocabulary meta-schema")
				.Type(JsonSchemaType.Object | JsonSchemaType.Boolean)
				.Property("format", new JsonSchema().Type(JsonSchemaType.String));

		/// <summary>
		/// The content meta-schema for 2019-04.
		/// </summary>
		public static readonly JsonSchema Draft2019_04_Content =
			new JsonSchema {SupportedVersions = JsonSchemaVersion.Draft2019_04}
				.Schema("http://json-schema.org/2019-04/schema#")
				.Id("http://json-schema.org/2019-04/meta/content")
				.Vocabulary(SchemaVocabularies.Core, true)
				.Vocabulary(SchemaVocabularies.Content, true)
				.RecursiveAnchor(true)
				.Title("Content vocabulary meta-schema")
				.Type(JsonSchemaType.Object | JsonSchemaType.Boolean)
				.Property("contentMediaType", new JsonSchema().Type(JsonSchemaType.String))
				.Property("contentEncoding", new JsonSchema().Type(JsonSchemaType.String))
				.Property("contentSchema", new JsonSchema().RecursiveRefRoot());
	}
}