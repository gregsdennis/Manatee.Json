namespace Manatee.Json.Schema
{
	/// <summary>
	/// Defines the official draft meta-schemas.
	/// </summary>
	public static partial class MetaSchemas
	{
		/// <summary>
		/// The meta-schema for draft-04.
		/// </summary>
		public static readonly JsonSchema Draft04 =
			new JsonSchema {SupportedVersions = JsonSchemaVersion.Draft04}
				.IdDraft04("http://json-schema.org/draft-04/schema#")
				.Schema("http://json-schema.org/draft-04/schema#")
				.Description("Core schema meta-schema")
				.Definition("schemaArray", new JsonSchema()
					            .Type(JsonSchemaType.Array)
					            .MinItems(1)
					            .Items(new JsonSchema().RefRoot()))
				.Definition("positiveInteger", new JsonSchema()
					            .Type(JsonSchemaType.Integer)
					            .Minimum(0))
				.Definition("positiveIntegerDefault0", new JsonSchema()
					            .AllOf(new JsonSchema().Ref("#/definitions/positiveInteger"),
					                   new JsonSchema().Default(0)))
				.Definition("simpleTypes", new JsonSchema().Enum("array", "boolean", "integer", "null", "number", "object", "string"))
				.Definition("stringArray", new JsonSchema()
					            .Type(JsonSchemaType.Array)
					            .Items(new JsonSchema().Type(JsonSchemaType.String))
					            .MinItems(1)
					            .UniqueItems(true))
				.Type(JsonSchemaType.Object)
				.Property("id", new JsonSchema().Type(JsonSchemaType.String))
				.Property("$schema", new JsonSchema().Type(JsonSchemaType.String))
				.Property("title", new JsonSchema().Type(JsonSchemaType.String))
				.Property("description", new JsonSchema().Type(JsonSchemaType.String))
				.Property("default", new JsonSchema())
				.Property("multipleOf", new JsonSchema()
					          .Type(JsonSchemaType.Number)
					          .Minimum(0)
					          .ExclusiveMinimumDraft04(true))
				.Property("maximum", new JsonSchema().Type(JsonSchemaType.Number))
				.Property("exclusiveMaximum", new JsonSchema().Type(JsonSchemaType.Boolean).Default(false))
				.Property("minimum", new JsonSchema().Type(JsonSchemaType.Number))
				.Property("exclusiveMinimum", new JsonSchema().Type(JsonSchemaType.Boolean).Default(false))
				.Property("maxLength", new JsonSchema().Ref("#/definitions/positiveInteger"))
				.Property("minLength", new JsonSchema().Ref("#/definitions/positiveIntegerDefault0"))
				.Property("pattern", new JsonSchema()
					          .Type(JsonSchemaType.String)
					          .Format(StringFormat.Regex))
				.Property("additionalItems", new JsonSchema()
					          .AnyOf(new JsonSchema().Type(JsonSchemaType.Boolean),
					                 new JsonSchema().RefRoot())
					          .Default(new JsonObject()))
				.Property("items", new JsonSchema()
					          .AnyOf(new JsonSchema().RefRoot(),
					                 new JsonSchema().Ref("#/definitions/schemaArray"))
					          .Default(new JsonObject()))
				.Property("maxItems", new JsonSchema().Ref("#/definitions/positiveInteger"))
				.Property("minItems", new JsonSchema().Ref("#/definitions/positiveIntegerDefault0"))
				.Property("uniqueItems", new JsonSchema()
					          .Type(JsonSchemaType.Boolean)
					          .Default(false))
				.Property("maxProperties", new JsonSchema().Ref("#/definitions/positiveInteger"))
				.Property("minProperties", new JsonSchema().Ref("#/definitions/positiveIntegerDefault0"))
				.Property("required", new JsonSchema().Ref("#/definitions/stringArray"))
				.Property("additionalProperties", new JsonSchema()
					          .AnyOf(new JsonSchema().Type(JsonSchemaType.Boolean),
					                 new JsonSchema().RefRoot())
					          .Default(new JsonObject()))
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
				.Dependency("exclusiveMaximum", "maximum")
				.Dependency("exclusiveMinimum", "minimum")
				.Default(new JsonObject());
	}
}