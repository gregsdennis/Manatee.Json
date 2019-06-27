using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Extends <see cref="JsonSchema"/> to aid in fetching properties.
	/// </summary>
	public static class GetterExtensions
	{
		/// <summary>
		/// Gets the value for the `additionalItems` keyword, if present.
		/// </summary>
		public static JsonSchema AdditionalItems(this JsonSchema schema)
		{
			return schema.Get<AdditionalItemsKeyword>()?.Value;
		}
		/// <summary>
		/// Gets the value for the `additionalProperties` keyword, if present.
		/// </summary>
		public static JsonSchema AdditionalProperties(this JsonSchema schema)
		{
			return schema.Get<AdditionalPropertiesKeyword>()?.Value;
		}
		/// <summary>
		/// Gets the value for the `allOf` keyword, if present.
		/// </summary>
		public static List<JsonSchema> AllOf(this JsonSchema schema)
		{
			return schema.Get<AllOfKeyword>();
		}
		/// <summary>
		/// Gets the value for the `anyOf` keyword, if present.
		/// </summary>
		public static List<JsonSchema> AnyOf(this JsonSchema schema)
		{
			return schema.Get<AnyOfKeyword>();
		}
		/// <summary>
		/// Gets the value for the `$comment` keyword, if present.
		/// </summary>
		public static string Comment(this JsonSchema schema)
		{
			return schema.Get<CommentKeyword>()?.Value;
		}
		/// <summary>
		/// Gets the value for the `const` keyword, if present.
		/// </summary>
		public static JsonValue Const(this JsonSchema schema)
		{
			return schema.Get<ConstKeyword>()?.Value;
		}
		/// <summary>
		/// Gets the value for the `contains` keyword, if present.
		/// </summary>
		public static JsonSchema Contains(this JsonSchema schema)
		{
			return schema.Get<ContainsKeyword>()?.Value;
		}
		/// <summary>
		/// Gets the value for the `contentEncoding` keyword, if present.
		/// </summary>
		public static string ContentEncoding(this JsonSchema schema)
		{
			return schema.Get<ContentEncodingKeyword>()?.Value;
		}
		/// <summary>
		/// Gets the value for the `contentMediaType` keyword, if present.
		/// </summary>
		public static string ContentMediaType(this JsonSchema schema)
		{
			return schema.Get<ContentMediaTypeKeyword>()?.Value;
		}
		/// <summary>
		/// Gets the value for the `contentSchema` keyword, if present.
		/// </summary>
		public static JsonSchema ContentSchema(this JsonSchema schema)
		{
			return schema.Get<ContentSchemaKeyword>()?.Value;
		}
		/// <summary>
		/// Gets the value for the `default` keyword, if present.
		/// </summary>
		public static JsonValue Default(this JsonSchema schema)
		{
			return schema.Get<DefaultKeyword>()?.Value;
		}
		/// <summary>
		/// Gets the value for the `$defs` keyword, if present.
		/// </summary>
		public static Dictionary<string, JsonSchema> Defs(this JsonSchema schema)
		{
			return schema.Get<DefsKeyword>();
		}
		/// <summary>
		/// Gets the value for the `definitions` keyword, if present.
		/// </summary>
		public static Dictionary<string, JsonSchema> Definitions(this JsonSchema schema)
		{
			return schema.Get<DefinitionsKeyword>();
		}
		/// <summary>
		/// Gets the value for the `description` keyword, if present.
		/// </summary>
		public static string Description(this JsonSchema schema)
		{
			return schema.Get<DescriptionKeyword>()?.Value;
		}
		/// <summary>
		/// Gets the value for the `else` keyword, if present.
		/// </summary>
		public static JsonSchema Else(this JsonSchema schema)
		{
			return schema.Get<ElseKeyword>()?.Value;
		}
		/// <summary>
		/// Gets the value for the `enum` keyword, if present.
		/// </summary>
		public static List<JsonValue> Enum(this JsonSchema schema)
		{
			return schema.Get<EnumKeyword>();
		}
		/// <summary>
		/// Gets the value for the `examples` keyword, if present.
		/// </summary>
		public static List<JsonValue> Examples(this JsonSchema schema)
		{
			return schema.Get<ExamplesKeyword>();
		}
		/// <summary>
		/// Gets the value for the `exclusiveMaximum` keyword, if present.
		/// </summary>
		public static double? ExclusiveMaximum(this JsonSchema schema)
		{
			return schema.Get<ExclusiveMaximumKeyword>()?.Value;
		}
		/// <summary>
		/// Gets the value for the `exclusiveMaximum` keyword for draft-04, if present.
		/// </summary>
		public static bool ExclusiveMaximumDraft04(this JsonSchema schema)
		{
			return schema.Get<ExclusiveMaximumDraft04Keyword>()?.Value ?? false;
		}
		/// <summary>
		/// Gets the value for the `exclusiveMinimum` keyword, if present.
		/// </summary>
		public static double? ExclusiveMinimum(this JsonSchema schema)
		{
			return schema.Get<ExclusiveMinimumKeyword>()?.Value;
		}
		/// <summary>
		/// Gets the value for the `exclusiveMinimum` keyword for draft-04, if present.
		/// </summary>
		public static bool ExclusiveMinimumDraft04(this JsonSchema schema)
		{
			return schema.Get<ExclusiveMinimumDraft04Keyword>()?.Value ?? false;
		}
		/// <summary>
		/// Gets the value for the `format` keyword, if present.
		/// </summary>
		public static StringFormat Format(this JsonSchema schema)
		{
			return schema.Get<FormatKeyword>()?.Value;
		}
		/// <summary>
		/// Gets the value for the `$id` (or `id` for draft-04) keyword, if present.
		/// </summary>
		public static string Id(this JsonSchema schema)
		{
			return schema.Get<IdKeyword>()?.Value ?? schema.Get<IdKeywordDraft04>()?.Value;
		}
		/// <summary>
		/// Gets the value for the `items` keyword, if present.
		/// </summary>
		public static List<JsonSchema> Items(this JsonSchema schema)
		{
			return schema.Get<ItemsKeyword>();
		}
		/// <summary>
		/// Gets the value for the `maxContains` keyword, if present.
		/// </summary>
		public static double? MaxContains(this JsonSchema schema)
		{
			return schema.Get<MaxContainsKeyword>()?.Value;
		}
		/// <summary>
		/// Gets the value for the `maximum` keyword, if present.
		/// </summary>
		public static double? Maximum(this JsonSchema schema)
		{
			return schema.Get<MaximumKeyword>()?.Value;
		}
		/// <summary>
		/// Gets the value for the `maxItems` keyword, if present.
		/// </summary>
		public static double? MaxItems(this JsonSchema schema)
		{
			return schema.Get<MaxItemsKeyword>()?.Value;
		}
		/// <summary>
		/// Gets the value for the `maxLength` keyword, if present.
		/// </summary>
		public static double? MaxLength(this JsonSchema schema)
		{
			return schema.Get<MaxLengthKeyword>()?.Value;
		}
		/// <summary>
		/// Gets the value for the `maxProperties` keyword, if present.
		/// </summary>
		public static double? MaxProperties(this JsonSchema schema)
		{
			return schema.Get<MaxPropertiesKeyword>()?.Value;
		}
		/// <summary>
		/// Gets the value for the `minContains` keyword, if present.
		/// </summary>
		public static double? MinContains(this JsonSchema schema)
		{
			return schema.Get<MinContainsKeyword>()?.Value;
		}
		/// <summary>
		/// Gets the value for the `minimum` keyword, if present.
		/// </summary>
		public static double? Minimum(this JsonSchema schema)
		{
			return schema.Get<MinimumKeyword>()?.Value;
		}
		/// <summary>
		/// Gets the value for the `minItems` keyword, if present.
		/// </summary>
		public static double? MinItems(this JsonSchema schema)
		{
			return schema.Get<MinItemsKeyword>()?.Value;
		}
		/// <summary>
		/// Gets the value for the `minLength` keyword, if present.
		/// </summary>
		public static double? MinLength(this JsonSchema schema)
		{
			return schema.Get<MinLengthKeyword>()?.Value;
		}
		/// <summary>
		/// Gets the value for the `minProperties` keyword, if present.
		/// </summary>
		public static double? MinProperties(this JsonSchema schema)
		{
			return schema.Get<MinPropertiesKeyword>()?.Value;
		}
		/// <summary>
		/// Gets the value for the `multipleOf` keyword, if present.
		/// </summary>
		public static double? MultipleOf(this JsonSchema schema)
		{
			return schema.Get<MultipleOfKeyword>()?.Value;
		}
		/// <summary>
		/// Gets the value for the `not` keyword, if present.
		/// </summary>
		public static JsonSchema Not(this JsonSchema schema)
		{
			return schema.Get<NotKeyword>()?.Value;
		}
		/// <summary>
		/// Gets the value for the `oneOf` keyword, if present.
		/// </summary>
		public static List<JsonSchema> OneOf(this JsonSchema schema)
		{
			return schema.Get<OneOfKeyword>();
		}
		/// <summary>
		/// Gets the value for the `pattern` keyword, if present.
		/// </summary>
		public static Regex Pattern(this JsonSchema schema)
		{
			return schema.Get<PatternKeyword>()?.Value;
		}
		/// <summary>
		/// Gets the value for the `patternProperties` keyword, if present.
		/// </summary>
		public static Dictionary<string, JsonSchema> PatternProperties(this JsonSchema schema)
		{
			return schema.Get<PatternPropertiesKeyword>();
		}
		/// <summary>
		/// Gets the value for the `properties` keyword, if present.
		/// </summary>
		public static Dictionary<string, JsonSchema> Properties(this JsonSchema schema)
		{
			return schema.Get<PropertiesKeyword>();
		}
		/// <summary>
		/// Gets the value for the `propertyNames` keyword, if present.
		/// </summary>
		public static JsonSchema PropertyNames(this JsonSchema schema)
		{
			return schema.Get<PropertyNamesKeyword>()?.Value;
		}
		/// <summary>
		/// Gets the value for the `readOnly` keyword, if present.
		/// </summary>
		public static bool ReadOnly(this JsonSchema schema)
		{
			return schema.Get<ReadOnlyKeyword>()?.Value ?? false;
		}
		/// <summary>
		/// Gets the value for the `$recursiveAnchor` keyword, if present.
		/// </summary>
		public static bool? RecursiveAnchor(this JsonSchema schema)
		{
			return schema.Get<RecursiveAnchorKeyword>()?.Value;
		}
		/// <summary>
		/// Gets the value for the `recursiveRef` keyword, if present.
		/// </summary>
		public static string RecursiveRef(this JsonSchema schema)
		{
			return schema.Get<RecursiveRefKeyword>()?.Reference;
		}
		/// <summary>
		/// Gets the value for the `$ref` keyword, if present.
		/// </summary>
		public static string Ref(this JsonSchema schema)
		{
			return schema.Get<RefKeyword>()?.Reference;
		}
		/// <summary>
		/// Gets the resolved schema for the `$ref` keyword, if present.
		/// </summary>
		public static JsonSchema RefResolved(this JsonSchema schema)
		{
			return schema.Get<RefKeyword>()?.Resolved;
		}
		/// <summary>
		/// Gets the value for the `required` keyword, if present.
		/// </summary>
		public static List<string> Required(this JsonSchema schema)
		{
			return schema.Get<RequiredKeyword>();
		}
		/// <summary>
		/// Gets the value for the `$schema` keyword, if present.
		/// </summary>
		public static string Schema(this JsonSchema schema)
		{
			return schema.Get<SchemaKeyword>()?.Value;
		}
		/// <summary>
		/// Gets the value for the `then` keyword, if present.
		/// </summary>
		public static JsonSchema Then(this JsonSchema schema)
		{
			return schema.Get<ThenKeyword>()?.Value;
		}
		/// <summary>
		/// Gets the value for the `title` keyword, if present.
		/// </summary>
		public static string Title(this JsonSchema schema)
		{
			return schema.Get<TitleKeyword>()?.Value;
		}
		/// <summary>
		/// Gets the value for the `unevaluatedItems` keyword, if present.
		/// </summary>
		public static JsonSchema UnevaluatedItems(this JsonSchema schema)
		{
			return schema.Get<UnevaluatedItemsKeyword>()?.Value;
		}
		/// <summary>
		/// Gets the value for the `unevaluatedProperties` keyword, if present.
		/// </summary>
		public static JsonSchema UnevaluatedProperties(this JsonSchema schema)
		{
			return schema.Get<UnevaluatedPropertiesKeyword>()?.Value;
		}
		/// <summary>
		/// Gets the value for the `uniqueItems` keyword, if present.
		/// </summary>
		public static bool UniqueItems(this JsonSchema schema)
		{
			return schema.Get<UniqueItemsKeyword>()?.Value ?? false;
		}
		/// <summary>
		/// Gets the value for the `uniqueItems` keyword, if present.
		/// </summary>
		public static Dictionary<SchemaVocabulary, bool> Vocabulary(this JsonSchema schema)
		{
			return schema.Get<VocabularyKeyword>();
		}
		/// <summary>
		/// Gets the value for the `writeOnly` keyword, if present.
		/// </summary>
		public static bool WriteOnly(this JsonSchema schema)
		{
			return schema.Get<WriteOnlyKeyword>()?.Value ?? false;
		}

		/// <summary>
		/// Gets the indicated keyword, if present.
		/// </summary>
		public static T Get<T>(this JsonSchema schema)
			where T : IJsonSchemaKeyword
		{
			return schema.OfType<T>().FirstOrDefault();
		}
	}
}