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
		/// Gets the value for the <code>additionalItems</code> keyword, if present.
		/// </summary>
		public static JsonSchema AdditionalItems(this JsonSchema schema)
		{
			return schema.Get<AdditionalItemsKeyword>()?.Value;
		}
		/// <summary>
		/// Gets the value for the <code>additionalProperties</code> keyword, if present.
		/// </summary>
		public static JsonSchema AdditionalProperties(this JsonSchema schema)
		{
			return schema.Get<AdditionalPropertiesKeyword>()?.Value;
		}
		/// <summary>
		/// Gets the value for the <code>allOf</code> keyword, if present.
		/// </summary>
		public static List<JsonSchema> AllOf(this JsonSchema schema)
		{
			return schema.Get<AllOfKeyword>();
		}
		/// <summary>
		/// Gets the value for the <code>anyOf</code> keyword, if present.
		/// </summary>
		public static List<JsonSchema> AnyOf(this JsonSchema schema)
		{
			return schema.Get<AnyOfKeyword>();
		}
		/// <summary>
		/// Gets the value for the <code>$comment</code> keyword, if present.
		/// </summary>
		public static string Comment(this JsonSchema schema)
		{
			return schema.Get<CommentKeyword>()?.Value;
		}
		/// <summary>
		/// Gets the value for the <code>const</code> keyword, if present.
		/// </summary>
		public static JsonValue Const(this JsonSchema schema)
		{
			return schema.Get<ConstKeyword>()?.Value;
		}
		/// <summary>
		/// Gets the value for the <code>contains</code> keyword, if present.
		/// </summary>
		public static JsonSchema Contains(this JsonSchema schema)
		{
			return schema.Get<ContainsKeyword>()?.Value;
		}
		/// <summary>
		/// Gets the value for the <code>contentEncoding</code> keyword, if present.
		/// </summary>
		public static string ContentEncoding(this JsonSchema schema)
		{
			return schema.Get<ContentEncodingKeyword>()?.Value;
		}
		/// <summary>
		/// Gets the value for the <code>contentMediaType</code> keyword, if present.
		/// </summary>
		public static string ContentMediaType(this JsonSchema schema)
		{
			return schema.Get<ContentMediaTypeKeyword>()?.Value;
		}
		/// <summary>
		/// Gets the value for the <code>default</code> keyword, if present.
		/// </summary>
		public static JsonValue Default(this JsonSchema schema)
		{
			return schema.Get<DefaultKeyword>()?.Value;
		}
		/// <summary>
		/// Gets the value for the <code>$defs</code> keyword, if present.
		/// </summary>
		public static Dictionary<string, JsonSchema> Defs(this JsonSchema schema)
		{
			return schema.Get<DefsKeyword>();
		}
		/// <summary>
		/// Gets the value for the <code>definitions</code> keyword, if present.
		/// </summary>
		public static Dictionary<string, JsonSchema> Definitions(this JsonSchema schema)
		{
			return schema.Get<DefinitionsKeyword>();
		}
		/// <summary>
		/// Gets the value for the <code>description</code> keyword, if present.
		/// </summary>
		public static string Description(this JsonSchema schema)
		{
			return schema.Get<DescriptionKeyword>()?.Value;
		}
		/// <summary>
		/// Gets the value for the <code>else</code> keyword, if present.
		/// </summary>
		public static JsonSchema Else(this JsonSchema schema)
		{
			return schema.Get<ElseKeyword>()?.Value;
		}
		/// <summary>
		/// Gets the value for the <code>enum</code> keyword, if present.
		/// </summary>
		public static List<JsonValue> Enum(this JsonSchema schema)
		{
			return schema.Get<EnumKeyword>();
		}
		/// <summary>
		/// Gets the value for the <code>examples</code> keyword, if present.
		/// </summary>
		public static List<JsonValue> Examples(this JsonSchema schema)
		{
			return schema.Get<ExamplesKeyword>();
		}
		/// <summary>
		/// Gets the value for the <code>exclusiveMaximum</code> keyword, if present.
		/// </summary>
		public static double? ExclusiveMaximum(this JsonSchema schema)
		{
			return schema.Get<ExclusiveMaximumKeyword>()?.Value;
		}
		/// <summary>
		/// Gets the value for the <code>exclusiveMaximum</code> keyword for draft-04, if present.
		/// </summary>
		public static bool ExclusiveMaximumDraft04(this JsonSchema schema)
		{
			return schema.Get<ExclusiveMaximumDraft04Keyword>()?.Value ?? false;
		}
		/// <summary>
		/// Gets the value for the <code>exclusiveMinimum</code> keyword, if present.
		/// </summary>
		public static double? ExclusiveMinimum(this JsonSchema schema)
		{
			return schema.Get<ExclusiveMinimumKeyword>()?.Value;
		}
		/// <summary>
		/// Gets the value for the <code>exclusiveMinimum</code> keyword for draft-04, if present.
		/// </summary>
		public static bool ExclusiveMinimumDraft04(this JsonSchema schema)
		{
			return schema.Get<ExclusiveMinimumDraft04Keyword>()?.Value ?? false;
		}
		/// <summary>
		/// Gets the value for the <code>format</code> keyword, if present.
		/// </summary>
		public static StringFormat Format(this JsonSchema schema)
		{
			return schema.Get<FormatKeyword>()?.Value;
		}
		/// <summary>
		/// Gets the value for the <code>$id</code> (or <code>id</code> for draft-04) keyword, if present.
		/// </summary>
		public static string Id(this JsonSchema schema)
		{
			return schema.Get<IdKeyword>()?.Value ?? schema.Get<IdKeywordDraft04>()?.Value;
		}
		/// <summary>
		/// Gets the value for the <code>items</code> keyword, if present.
		/// </summary>
		public static List<JsonSchema> Items(this JsonSchema schema)
		{
			return schema.Get<ItemsKeyword>();
		}
		/// <summary>
		/// Gets the value for the <code>maxContains</code> keyword, if present.
		/// </summary>
		public static double? MaxContains(this JsonSchema schema)
		{
			return schema.Get<MaxContainsKeyword>()?.Value;
		}
		/// <summary>
		/// Gets the value for the <code>maximum</code> keyword, if present.
		/// </summary>
		public static double? Maximum(this JsonSchema schema)
		{
			return schema.Get<MaximumKeyword>()?.Value;
		}
		/// <summary>
		/// Gets the value for the <code>maxItems</code> keyword, if present.
		/// </summary>
		public static double? MaxItems(this JsonSchema schema)
		{
			return schema.Get<MaxItemsKeyword>()?.Value;
		}
		/// <summary>
		/// Gets the value for the <code>maxLength</code> keyword, if present.
		/// </summary>
		public static double? MaxLength(this JsonSchema schema)
		{
			return schema.Get<MaxLengthKeyword>()?.Value;
		}
		/// <summary>
		/// Gets the value for the <code>maxProperties</code> keyword, if present.
		/// </summary>
		public static double? MaxProperties(this JsonSchema schema)
		{
			return schema.Get<MaxPropertiesKeyword>()?.Value;
		}
		/// <summary>
		/// Gets the value for the <code>minContains</code> keyword, if present.
		/// </summary>
		public static double? MinContains(this JsonSchema schema)
		{
			return schema.Get<MinContainsKeyword>()?.Value;
		}
		/// <summary>
		/// Gets the value for the <code>minimum</code> keyword, if present.
		/// </summary>
		public static double? Minimum(this JsonSchema schema)
		{
			return schema.Get<MinimumKeyword>()?.Value;
		}
		/// <summary>
		/// Gets the value for the <code>minItems</code> keyword, if present.
		/// </summary>
		public static double? MinItems(this JsonSchema schema)
		{
			return schema.Get<MinItemsKeyword>()?.Value;
		}
		/// <summary>
		/// Gets the value for the <code>minLength</code> keyword, if present.
		/// </summary>
		public static double? MinLength(this JsonSchema schema)
		{
			return schema.Get<MinLengthKeyword>()?.Value;
		}
		/// <summary>
		/// Gets the value for the <code>minProperties</code> keyword, if present.
		/// </summary>
		public static double? MinProperties(this JsonSchema schema)
		{
			return schema.Get<MinPropertiesKeyword>()?.Value;
		}
		/// <summary>
		/// Gets the value for the <code>multipleOf</code> keyword, if present.
		/// </summary>
		public static double? MultipleOf(this JsonSchema schema)
		{
			return schema.Get<MultipleOfKeyword>()?.Value;
		}
		/// <summary>
		/// Gets the value for the <code>not</code> keyword, if present.
		/// </summary>
		public static JsonSchema Not(this JsonSchema schema)
		{
			return schema.Get<NotKeyword>()?.Value;
		}
		/// <summary>
		/// Gets the value for the <code>oneOf</code> keyword, if present.
		/// </summary>
		public static List<JsonSchema> OneOf(this JsonSchema schema)
		{
			return schema.Get<OneOfKeyword>();
		}
		/// <summary>
		/// Gets the value for the <code>pattern</code> keyword, if present.
		/// </summary>
		public static Regex Pattern(this JsonSchema schema)
		{
			return schema.Get<PatternKeyword>()?.Value;
		}
		/// <summary>
		/// Gets the value for the <code>patternProperties</code> keyword, if present.
		/// </summary>
		public static Dictionary<string, JsonSchema> PatternProperties(this JsonSchema schema)
		{
			return schema.Get<PatternPropertiesKeyword>();
		}
		/// <summary>
		/// Gets the value for the <code>properties</code> keyword, if present.
		/// </summary>
		public static Dictionary<string, JsonSchema> Properties(this JsonSchema schema)
		{
			return schema.Get<PropertiesKeyword>();
		}
		/// <summary>
		/// Gets the value for the <code>propertyNames</code> keyword, if present.
		/// </summary>
		public static JsonSchema PropertyNames(this JsonSchema schema)
		{
			return schema.Get<PropertyNamesKeyword>()?.Value;
		}
		/// <summary>
		/// Gets the value for the <code>readOnly</code> keyword, if present.
		/// </summary>
		public static bool ReadOnly(this JsonSchema schema)
		{
			return schema.Get<ReadOnlyKeyword>()?.Value ?? false;
		}
		/// <summary>
		/// Gets the value for the <code>$recursiveAnchor</code> keyword, if present.
		/// </summary>
		public static bool? RecursiveAnchor(this JsonSchema schema)
		{
			return schema.Get<RecursiveAnchorKeyword>()?.Value;
		}
		/// <summary>
		/// Gets the value for the <code>recursiveRef</code> keyword, if present.
		/// </summary>
		public static string RecursiveRef(this JsonSchema schema)
		{
			return schema.Get<RecursiveRefKeyword>()?.Reference;
		}
		/// <summary>
		/// Gets the value for the <code>$ref</code> keyword, if present.
		/// </summary>
		public static string Ref(this JsonSchema schema)
		{
			return schema.Get<RefKeyword>()?.Reference;
		}
		/// <summary>
		/// Gets the resolved schema for the <code>$ref</code> keyword, if present.
		/// </summary>
		public static JsonSchema RefResolved(this JsonSchema schema)
		{
			return schema.Get<RefKeyword>()?.Resolved;
		}
		/// <summary>
		/// Gets the value for the <code>required</code> keyword, if present.
		/// </summary>
		public static List<string> Required(this JsonSchema schema)
		{
			return schema.Get<RequiredKeyword>();
		}
		/// <summary>
		/// Gets the value for the <code>$schema</code> keyword, if present.
		/// </summary>
		public static string Schema(this JsonSchema schema)
		{
			return schema.Get<SchemaKeyword>()?.Value;
		}
		/// <summary>
		/// Gets the value for the <code>then</code> keyword, if present.
		/// </summary>
		public static JsonSchema Then(this JsonSchema schema)
		{
			return schema.Get<ThenKeyword>()?.Value;
		}
		/// <summary>
		/// Gets the value for the <code>title</code> keyword, if present.
		/// </summary>
		public static string Title(this JsonSchema schema)
		{
			return schema.Get<TitleKeyword>()?.Value;
		}
		/// <summary>
		/// Gets the value for the <code>unevaluatedProperties</code> keyword, if present.
		/// </summary>
		public static JsonSchema UnevaluatedProperties(this JsonSchema schema)
		{
			return schema.Get<UnevaluatedPropertiesKeyword>()?.Value;
		}
		/// <summary>
		/// Gets the value for the <code>uniqueItems</code> keyword, if present.
		/// </summary>
		public static bool UniqueItems(this JsonSchema schema)
		{
			return schema.Get<UniqueItemsKeyword>()?.Value ?? false;
		}
		/// <summary>
		/// Gets the value for the <code>writeOnly</code> keyword, if present.
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