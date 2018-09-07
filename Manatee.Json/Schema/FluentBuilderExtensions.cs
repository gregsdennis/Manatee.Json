using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace Manatee.Json.Schema
{
	public static class FluentBuilderExtensions
	{
		public static JsonSchema Id(this JsonSchema schema, string id)
		{
			schema.Add(new IdKeyword(id));

			return schema;
		}
		public static JsonSchema IdDraft04(this JsonSchema schema, string id)
		{
			schema.Add(new IdKeywordDraft04(id));

			return schema;
		}
		public static JsonSchema Schema(this JsonSchema schema, string schemaCallout)
		{
			schema.Add(new SchemaKeyword(schemaCallout));

			return schema;
		}
		public static JsonSchema Type(this JsonSchema schema, JsonSchemaType type)
		{
			schema.Add(new TypeKeyword(type));

			return schema;
		}
		public static JsonSchema Definition(this JsonSchema schema, string name, JsonSchema definition)
		{
			var keyword = schema.OfType<DefinitionsKeyword>().FirstOrDefault();

			if (keyword == null)
			{
				keyword = new DefinitionsKeyword();
				schema.Add(keyword);
			}

			keyword.Add(name, definition);

			return schema;
		}
		public static JsonSchema Property(this JsonSchema schema, string name, JsonSchema property)
		{
			var keyword = schema.OfType<PropertiesKeyword>().FirstOrDefault();

			if (keyword == null)
			{
				keyword = new PropertiesKeyword();
				schema.Add(keyword);
			}

			keyword.Add(name, property);

			return schema;
		}
		public static JsonSchema PatternProperty(this JsonSchema schema, [RegexPattern] string name, JsonSchema property)
		{
			var keyword = schema.OfType<PatternPropertiesKeyword>().FirstOrDefault();

			if (keyword == null)
			{
				keyword = new PatternPropertiesKeyword();
				schema.Add(keyword);
			}

			keyword.Add(name, property);

			return schema;
		}
		public static JsonSchema Dependency(this JsonSchema schema, string name, params string[] dependencies)
		{
			var keyword = schema.OfType<DependenciesKeyword>().FirstOrDefault();

			if (keyword == null)
			{
				keyword = new DependenciesKeyword();
				schema.Add(keyword);
			}

			keyword.Add(new PropertyDependency(name, dependencies));

			return schema;
		}
		public static JsonSchema Dependency(this JsonSchema schema, string name, JsonSchema dependency)
		{
			var keyword = schema.OfType<DependenciesKeyword>().FirstOrDefault();

			if (keyword == null)
			{
				keyword = new DependenciesKeyword();
				schema.Add(keyword);
			}

			keyword.Add(new SchemaDependency(name, dependency));

			return schema;
		}
		public static JsonSchema Items(this JsonSchema schema, params JsonSchema[] definitions)
		{
			var keyword = schema.OfType<ItemsKeyword>().FirstOrDefault();

			if (keyword == null)
			{
				keyword = new ItemsKeyword();
				schema.Add(keyword);
			}

			keyword.AddRange(definitions);

			return schema;
		}
		public static JsonSchema AllOf(this JsonSchema schema, params JsonSchema[] definitions)
		{
			var keyword = schema.OfType<AllOfKeyword>().FirstOrDefault();

			if (keyword == null)
			{
				keyword = new AllOfKeyword();
				schema.Add(keyword);
			}

			keyword.AddRange(definitions);

			return schema;
		}
		public static JsonSchema AnyOf(this JsonSchema schema, params JsonSchema[] definitions)
		{
			var keyword = schema.OfType<AnyOfKeyword>().FirstOrDefault();

			if (keyword == null)
			{
				keyword = new AnyOfKeyword();
				schema.Add(keyword);
			}

			keyword.AddRange(definitions);

			return schema;
		}
		public static JsonSchema OneOf(this JsonSchema schema, params JsonSchema[] definitions)
		{
			var keyword = schema.OfType<OneOfKeyword>().FirstOrDefault();

			if (keyword == null)
			{
				keyword = new OneOfKeyword();
				schema.Add(keyword);
			}

			keyword.AddRange(definitions);

			return schema;
		}
		public static JsonSchema Description(this JsonSchema schema, string description)
		{
			schema.Add(new DescriptionKeyword(description));

			return schema;
		}
		public static JsonSchema Pattern(this JsonSchema schema, [RegexPattern] string pattern)
		{
			schema.Add(new PatternKeyword(pattern));

			return schema;
		}
		public static JsonSchema Pattern(this JsonSchema schema, Regex pattern)
		{
			schema.Add(new PatternKeyword(pattern));

			return schema;
		}
		public static JsonSchema Format(this JsonSchema schema, StringFormat format)
		{
			schema.Add(new FormatKeyword(format));

			return schema;
		}
		public static JsonSchema Title(this JsonSchema schema, string title)
		{
			schema.Add(new TitleKeyword(title));

			return schema;
		}
		public static JsonSchema AdditionalProperties(this JsonSchema schema, JsonSchema otherSchema)
		{
			schema.Add(new AdditionalPropertiesKeyword(otherSchema));

			return schema;
		}
		public static JsonSchema PropertyNames(this JsonSchema schema, JsonSchema otherSchema)
		{
			schema.Add(new PropertyNamesKeyword(otherSchema));

			return schema;
		}
		public static JsonSchema Enum(this JsonSchema schema, params JsonValue[] values)
		{
			schema.Add(new EnumKeyword(values));

			return schema;
		}
		public static JsonSchema Required(this JsonSchema schema, params string[] values)
		{
			schema.Add(new RequiredKeyword(values));

			return schema;
		}
		public static JsonSchema Default(this JsonSchema schema, JsonValue value)
		{
			schema.Add(new DefaultKeyword(value));

			return schema;
		}
		public static JsonSchema Const(this JsonSchema schema, JsonValue value)
		{
			schema.Add(new ConstKeyword(value));

			return schema;
		}
		public static JsonSchema Examples(this JsonSchema schema, params JsonValue[] value)
		{
			schema.Add(new ExamplesKeyword(value));

			return schema;
		}
		public static JsonSchema Minimum(this JsonSchema schema, double minimum)
		{
			schema.Add(new MinimumKeyword(minimum));

			return schema;
		}
		public static JsonSchema Maximum(this JsonSchema schema, double maximum)
		{
			schema.Add(new MaximumKeyword(maximum));

			return schema;
		}
		public static JsonSchema RefRoot(this JsonSchema schema)
		{
			schema.Add(RefKeyword.Root);

			return schema;
		}
		public static JsonSchema Ref(this JsonSchema schema, string reference)
		{
			schema.Add(new RefKeyword(reference));

			return schema;
		}
		public static JsonSchema MinItems(this JsonSchema schema, uint count)
		{
			schema.Add(new MinItemsKeyword(count));

			return schema;
		}
		public static JsonSchema MaxItems(this JsonSchema schema, uint count)
		{
			schema.Add(new MaxItemsKeyword(count));

			return schema;
		}
		public static JsonSchema MinLength(this JsonSchema schema, uint length)
		{
			schema.Add(new MinLengthKeyword(length));

			return schema;
		}
		public static JsonSchema MaxLength(this JsonSchema schema, uint length)
		{
			schema.Add(new MaxLengthKeyword(length));

			return schema;
		}
		public static JsonSchema MinProperties(this JsonSchema schema, uint count)
		{
			schema.Add(new MinPropertiesKeyword(count));

			return schema;
		}
		public static JsonSchema MaxProperties(this JsonSchema schema, uint count)
		{
			schema.Add(new MaxPropertiesKeyword(count));

			return schema;
		}
		public static JsonSchema UniqueItems(this JsonSchema schema, bool unique)
		{
			schema.Add(new UniqueItemsKeyword(unique));

			return schema;
		}
		public static JsonSchema AdditionalItems(this JsonSchema schema, JsonSchema additionalItems)
		{
			schema.Add(new AdditionalItemsKeyword(additionalItems));

			return schema;
		}
		public static JsonSchema ExclusiveMinimumDraft04(this JsonSchema schema, bool isExclusive)
		{
			schema.Add(new ExclusiveMinimumDraft04Keyword(isExclusive));

			return schema;
		}
		public static JsonSchema ReadOnly(this JsonSchema schema, bool isReadOnly)
		{
			schema.Add(new ReadOnlyKeyword(isReadOnly));

			return schema;
		}
		public static JsonSchema WriteOnly(this JsonSchema schema, bool isWriteOnly)
		{
			schema.Add(new WriteOnlyKeyword(isWriteOnly));

			return schema;
		}
		public static JsonSchema ExclusiveMinimum(this JsonSchema schema, double minimum)
		{
			schema.Add(new ExclusiveMinimumKeyword(minimum));

			return schema;
		}
		public static JsonSchema ExclusiveMaximumDraft04(this JsonSchema schema, bool isExclusive)
		{
			schema.Add(new ExclusiveMaximumDraft04Keyword(isExclusive));

			return schema;
		}
		public static JsonSchema ExclusiveMaximum(this JsonSchema schema, double maximum)
		{
			schema.Add(new ExclusiveMaximumKeyword(maximum));

			return schema;
		}
		public static JsonSchema MultipleOf(this JsonSchema schema, double divisor)
		{
			schema.Add(new MultipleOfKeyword(divisor));

			return schema;
		}
		public static JsonSchema Comment(this JsonSchema schema, string comment)
		{
			schema.Add(new CommentKeyword(comment));

			return schema;
		}
		public static JsonSchema ContentMediaType(this JsonSchema schema, string comment)
		{
			schema.Add(new ContentMediaTypeKeyword(comment));

			return schema;
		}
		public static JsonSchema ContentEncoding(this JsonSchema schema, string comment)
		{
			schema.Add(new ContentEncodingKeyword(comment));

			return schema;
		}
	}
}