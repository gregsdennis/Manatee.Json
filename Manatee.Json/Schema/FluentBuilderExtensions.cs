using System;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Extends <see cref="JsonSchema"/> to aid in construction.
	/// </summary>
	public static class FluentBuilderExtensions
	{
		/// <summary>
		/// Add an `additionalItems` keyword to the schema.
		/// </summary>
		public static JsonSchema AdditionalItems(this JsonSchema schema, JsonSchema additionalItems)
		{
			schema.Add(new AdditionalItemsKeyword(additionalItems));

			return schema;
		}
		/// <summary>
		/// Add an `additionalProperties` keyword to the schema.
		/// </summary>
		public static JsonSchema AdditionalProperties(this JsonSchema schema, JsonSchema otherSchema)
		{
			schema.Add(new AdditionalPropertiesKeyword(otherSchema));

			return schema;
		}
		/// <summary>
		/// Add an `allOf` keyword to the schema.
		/// </summary>
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
		/// <summary>
		/// Add an `anyOf` keyword to the schema.
		/// </summary>
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
		/// <summary>
		/// Add a `$comment` keyword to the schema.
		/// </summary>
		public static JsonSchema Comment(this JsonSchema schema, string comment)
		{
			schema.Add(new CommentKeyword(comment));

			return schema;
		}
		/// <summary>
		/// Add a `const` keyword to the schema.
		/// </summary>
		public static JsonSchema Const(this JsonSchema schema, JsonValue value)
		{
			schema.Add(new ConstKeyword(value));

			return schema;
		}
		/// <summary>
		/// Add a `contains` keyword to the schema.
		/// </summary>
		public static JsonSchema Contains(this JsonSchema schema, JsonSchema match)
		{
			schema.Add(new ContainsKeyword(match));

			return schema;
		}
		/// <summary>
		/// Add a `contentEncoding` keyword to the schema.
		/// </summary>
		public static JsonSchema ContentEncoding(this JsonSchema schema, string encoding)
		{
			schema.Add(new ContentEncodingKeyword(encoding));

			return schema;
		}
		/// <summary>
		/// Add a `contentMediaType` keyword to the schema.
		/// </summary>
		public static JsonSchema ContentMediaType(this JsonSchema schema, string mediaType)
		{
			schema.Add(new ContentMediaTypeKeyword(mediaType));

			return schema;
		}
		/// <summary>
		/// Add a `contentSchema` keyword to the schema.
		/// </summary>
		public static JsonSchema ContentSchema(this JsonSchema schema, JsonSchema match)
		{
			schema.Add(new ContentSchemaKeyword(match));

			return schema;
		}
		/// <summary>
		/// Add a `default` keyword to the schema.
		/// </summary>
		public static JsonSchema Default(this JsonSchema schema, JsonValue value)
		{
			schema.Add(new DefaultKeyword(value));

			return schema;
		}
		/// <summary>
		/// Add a single definition to the `$def` keyword.
		/// </summary>
		public static JsonSchema Def(this JsonSchema schema, string name, JsonSchema definition)
		{
			var keyword = schema.OfType<DefsKeyword>().FirstOrDefault();

			if (keyword == null)
			{
				keyword = new DefsKeyword();
				schema.Add(keyword);
			}

			keyword.Add(name, definition);

			return schema;
		}
		/// <summary>
		/// Add a single definition to the `definitions` keyword.
		/// </summary>
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
		/// <summary>
		/// Add a property-based dependency to the `dependencies` keyword.
		/// </summary>
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
		/// <summary>
		/// Add a property-based dependency to the `dependencies` keyword.
		/// </summary>
		public static JsonSchema DependentRequired(this JsonSchema schema, string name, params string[] dependencies)
		{
			var keyword = schema.OfType<DependentRequiredKeyword>().FirstOrDefault();

			if (keyword == null)
			{
				keyword = new DependentRequiredKeyword();
				schema.Add(keyword);
			}

			keyword.Add(new PropertyDependency(name, dependencies));

			return schema;
		}
		/// <summary>
		/// Add a schema-based dependency to the `dependencies` keyword.
		/// </summary>
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
		/// <summary>
		/// Add a schema-based dependency to the `dependencies` keyword.
		/// </summary>
		public static JsonSchema DependentSchema(this JsonSchema schema, string name, JsonSchema dependency)
		{
			var keyword = schema.OfType<DependentSchemasKeyword>().FirstOrDefault();

			if (keyword == null)
			{
				keyword = new DependentSchemasKeyword();
				schema.Add(keyword);
			}

			keyword.Add(new SchemaDependency(name, dependency));

			return schema;
		}
		/// <summary>
		/// Add a `descriptions` keyword to the schema.
		/// </summary>
		public static JsonSchema Description(this JsonSchema schema, string description)
		{
			schema.Add(new DescriptionKeyword(description));

			return schema;
		}
		/// <summary>
		/// Add an `else` keyword to the schema.
		/// </summary>
		public static JsonSchema Else(this JsonSchema schema, JsonSchema elseSchema)
		{
			schema.Add(new ElseKeyword(elseSchema));

			return schema;
		}
		/// <summary>
		/// Add an `enum` keyword to the schema.
		/// </summary>
		public static JsonSchema Enum(this JsonSchema schema, params JsonValue[] values)
		{
			schema.Add(new EnumKeyword(values));

			return schema;
		}
		/// <summary>
		/// Add an `examples` keyword to the schema.
		/// </summary>
		public static JsonSchema Examples(this JsonSchema schema, params JsonValue[] value)
		{
			schema.Add(new ExamplesKeyword(value));

			return schema;
		}
		/// <summary>
		/// Add an `exclusiveMaximum` keyword to the schema.
		/// </summary>
		public static JsonSchema ExclusiveMaximum(this JsonSchema schema, double maximum)
		{
			schema.Add(new ExclusiveMaximumKeyword(maximum));

			return schema;
		}
		/// <summary>
		/// Add an `exclusiveMaximum` keyword for draft-04 to the schema.
		/// </summary>
		public static JsonSchema ExclusiveMaximumDraft04(this JsonSchema schema, bool isExclusive)
		{
			schema.Add(new ExclusiveMaximumDraft04Keyword(isExclusive));

			return schema;
		}
		/// <summary>
		/// Add an `exclusiveMinimum` keyword to the schema.
		/// </summary>
		public static JsonSchema ExclusiveMinimum(this JsonSchema schema, double minimum)
		{
			schema.Add(new ExclusiveMinimumKeyword(minimum));

			return schema;
		}
		/// <summary>
		/// Add an `exclusiveMinimum` keyword for draft-04 to the schema.
		/// </summary>
		public static JsonSchema ExclusiveMinimumDraft04(this JsonSchema schema, bool isExclusive)
		{
			schema.Add(new ExclusiveMinimumDraft04Keyword(isExclusive));

			return schema;
		}
		/// <summary>
		/// Add a `format` keyword to the schema.
		/// </summary>
		public static JsonSchema Format(this JsonSchema schema, StringFormat format)
		{
			schema.Add(new FormatKeyword(format));

			return schema;
		}
		/// <summary>
		/// Add an `$id` keyword to the schema.
		/// </summary>
		public static JsonSchema Id(this JsonSchema schema, string id)
		{
			schema.Add(new IdKeyword(id));

			return schema;
		}
		/// <summary>
		/// Add an `id` keyword for draft-04 to the schema.
		/// </summary>
		public static JsonSchema IdDraft04(this JsonSchema schema, string id)
		{
			schema.Add(new IdKeywordDraft04(id));

			return schema;
		}
		/// <summary>
		/// Add an `if` keyword to the schema.
		/// </summary>
		public static JsonSchema If(this JsonSchema schema, JsonSchema ifSchema)
		{
			schema.Add(new IfKeyword(ifSchema));

			return schema;
		}
		/// <summary>
		/// Add an `items` keyword to the schema.
		/// </summary>
		public static JsonSchema Item(this JsonSchema schema, JsonSchema definition)
		{
			var keyword = schema.OfType<ItemsKeyword>().FirstOrDefault();

			if (keyword == null)
			{
				keyword = new ItemsKeyword {IsArray = true};
				schema.Add(keyword);
			}

			keyword.Add(definition);

			return schema;
		}
		/// <summary>
		/// Add an `items` keyword to the schema.
		/// </summary>
		public static JsonSchema Items(this JsonSchema schema, JsonSchema definition)
		{
			var keyword = new ItemsKeyword {definition};
			schema.Add(keyword);

			return schema;
		}
		/// <summary>
		/// (Obsolete) Add an `items` keyword to the schema.
		/// </summary>
		[Obsolete("This method cannot consider a single-item array value.  Use multiple calls to Item() instead.")]
		public static JsonSchema Items(this JsonSchema schema, params JsonSchema[] definitions)
		{
			var keyword = schema.OfType<ItemsKeyword>().FirstOrDefault();

			if (keyword == null)
			{
				keyword = new ItemsKeyword {IsArray = true};
				schema.Add(keyword);
			}

			keyword.AddRange(definitions);

			return schema;
		}
		/// <summary>
		/// Add a `maxContains` keyword to the schema.
		/// </summary>
		public static JsonSchema MaxContains(this JsonSchema schema, uint count)
		{
			schema.Add(new MaxContainsKeyword(count));

			return schema;
		}
		/// <summary>
		/// Add a `maximum` keyword to the schema.
		/// </summary>
		public static JsonSchema Maximum(this JsonSchema schema, double maximum)
		{
			schema.Add(new MaximumKeyword(maximum));

			return schema;
		}
		/// <summary>
		/// Add a `maxItems` keyword to the schema.
		/// </summary>
		public static JsonSchema MaxItems(this JsonSchema schema, uint count)
		{
			schema.Add(new MaxItemsKeyword(count));

			return schema;
		}
		/// <summary>
		/// Add a `maxLength` keyword to the schema.
		/// </summary>
		public static JsonSchema MaxLength(this JsonSchema schema, uint length)
		{
			schema.Add(new MaxLengthKeyword(length));

			return schema;
		}
		/// <summary>
		/// Add a `maxProperties` keyword to the schema.
		/// </summary>
		public static JsonSchema MaxProperties(this JsonSchema schema, uint count)
		{
			schema.Add(new MaxPropertiesKeyword(count));

			return schema;
		}
		/// <summary>
		/// Add a `minContains` keyword to the schema.
		/// </summary>
		public static JsonSchema MinContains(this JsonSchema schema, uint count)
		{
			schema.Add(new MinContainsKeyword(count));

			return schema;
		}
		/// <summary>
		/// Add a `minimum` keyword to the schema.
		/// </summary>
		public static JsonSchema Minimum(this JsonSchema schema, double minimum)
		{
			schema.Add(new MinimumKeyword(minimum));

			return schema;
		}
		/// <summary>
		/// Add a `minItems` keyword to the schema.
		/// </summary>
		public static JsonSchema MinItems(this JsonSchema schema, uint count)
		{
			schema.Add(new MinItemsKeyword(count));

			return schema;
		}
		/// <summary>
		/// Add a `minLength` keyword to the schema.
		/// </summary>
		public static JsonSchema MinLength(this JsonSchema schema, uint length)
		{
			schema.Add(new MinLengthKeyword(length));

			return schema;
		}
		/// <summary>
		/// Add a `minProperties` keyword to the schema.
		/// </summary>
		public static JsonSchema MinProperties(this JsonSchema schema, uint count)
		{
			schema.Add(new MinPropertiesKeyword(count));

			return schema;
		}
		/// <summary>
		/// Add a `multipleOf` keyword to the schema.
		/// </summary>
		public static JsonSchema MultipleOf(this JsonSchema schema, double divisor)
		{
			schema.Add(new MultipleOfKeyword(divisor));

			return schema;
		}
		/// <summary>
		/// Add a `not` keyword to the schema.
		/// </summary>
		public static JsonSchema Not(this JsonSchema schema, JsonSchema notSchema)
		{
			schema.Add(new NotKeyword(notSchema));

			return schema;
		}
		/// <summary>
		/// Add a `oneOf` keyword to the schema.
		/// </summary>
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
		/// <summary>
		/// Add a `pattern` keyword to the schema.
		/// </summary>
		public static JsonSchema Pattern(this JsonSchema schema, [RegexPattern] string pattern)
		{
			schema.Add(new PatternKeyword(pattern));

			return schema;
		}
		/// <summary>
		/// Add a `pattern` keyword to the schema.
		/// </summary>
		public static JsonSchema Pattern(this JsonSchema schema, Regex pattern)
		{
			schema.Add(new PatternKeyword(pattern));

			return schema;
		}
		/// <summary>
		/// Add a single pattern-based property requirement to the `patterProperties` keyword.
		/// </summary>
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
		/// <summary>
		/// Add a single property requirement to the `properties` keyword.
		/// </summary>
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
		/// <summary>
		/// Add a `propertyNames` keyword to the schema.
		/// </summary>
		public static JsonSchema PropertyNames(this JsonSchema schema, JsonSchema otherSchema)
		{
			schema.Add(new PropertyNamesKeyword(otherSchema));

			return schema;
		}
		/// <summary>
		/// Add a `readOnly` keyword to the schema.
		/// </summary>
		public static JsonSchema ReadOnly(this JsonSchema schema, bool isReadOnly)
		{
			schema.Add(new ReadOnlyKeyword(isReadOnly));

			return schema;
		}
		/// <summary>
		/// Add a `$recursiveAnchor` keyword to the schema.  The only supported value is `true`.
		/// </summary>
		public static JsonSchema RecursiveAnchor(this JsonSchema schema, bool value)
		{
			schema.Add(new RecursiveAnchorKeyword(value));

			return schema;
		}
		/// <summary>
		/// Add a `$recursiveRef` keyword to the schema.
		/// </summary>
		public static JsonSchema RecursiveRef(this JsonSchema schema, string reference)
		{
			schema.Add(new RecursiveRefKeyword(reference));

			return schema;
		}
		/// <summary>
		/// Add a `$recursiveRef` that points to the root (`#`) keyword to the schema.
		/// </summary>
		public static JsonSchema RecursiveRefRoot(this JsonSchema schema)
		{
			schema.Add(new RecursiveRefKeyword("#"));

			return schema;
		}
		/// <summary>
		/// Add a `$ref` keyword to the schema.
		/// </summary>
		public static JsonSchema Ref(this JsonSchema schema, string reference)
		{
			schema.Add(new RefKeyword(reference));

			return schema;
		}
		/// <summary>
		/// Add a `$ref` keyword that points to the root (`#`) to the schema.
		/// </summary>
		public static JsonSchema RefRoot(this JsonSchema schema)
		{
			schema.Add(new RefKeyword("#"));

			return schema;
		}
		/// <summary>
		/// Add a `required` keyword to the schema.
		/// </summary>
		public static JsonSchema Required(this JsonSchema schema, params string[] values)
		{
			schema.Add(new RequiredKeyword(values));

			return schema;
		}
		/// <summary>
		/// Add a `$schema` keyword to the schema.
		/// </summary>
		public static JsonSchema Schema(this JsonSchema schema, string schemaCallout)
		{
			schema.Add(new SchemaKeyword(schemaCallout));

			return schema;
		}
		/// <summary>
		/// Add a `then` keyword to the schema.
		/// </summary>
		public static JsonSchema Then(this JsonSchema schema, JsonSchema thenSchema)
		{
			schema.Add(new ThenKeyword(thenSchema));

			return schema;
		}
		/// <summary>
		/// Add a `title` keyword to the schema.
		/// </summary>
		public static JsonSchema Title(this JsonSchema schema, string title)
		{
			schema.Add(new TitleKeyword(title));

			return schema;
		}
		/// <summary>
		/// Add a `type` keyword to the schema.
		/// </summary>
		public static JsonSchema Type(this JsonSchema schema, JsonSchemaType type)
		{
			schema.Add(new TypeKeyword(type));

			return schema;
		}
		/// <summary>
		/// Add an `unevaluatedItems` keyword to the schema.
		/// </summary>
		public static JsonSchema UnevaluatedItems(this JsonSchema schema, JsonSchema otherSchema)
		{
			schema.Add(new UnevaluatedItemsKeyword(otherSchema));

			return schema;
		}
		/// <summary>
		/// Add an `unevaluatedProperties` keyword to the schema.
		/// </summary>
		public static JsonSchema UnevaluatedProperties(this JsonSchema schema, JsonSchema otherSchema)
		{
			schema.Add(new UnevaluatedPropertiesKeyword(otherSchema));

			return schema;
		}
		/// <summary>
		/// Add a `uniqueItems` keyword to the schema.
		/// </summary>
		public static JsonSchema UniqueItems(this JsonSchema schema, bool unique)
		{
			schema.Add(new UniqueItemsKeyword(unique));

			return schema;
		}
		/// <summary>
		/// Add a single property requirement to the `properties` keyword.
		/// </summary>
		public static JsonSchema Vocabulary(this JsonSchema schema, string id, bool required)
		{
			var keyword = schema.OfType<VocabularyKeyword>().FirstOrDefault();

			if (keyword == null)
			{
				keyword = new VocabularyKeyword();
				schema.Add(keyword);
			}

			var vocabulary = SchemaKeywordCatalog.GetVocabulary(id) ?? new SchemaVocabulary(id);
			keyword[vocabulary] = required;

			return schema;
		}
		/// <summary>
		/// Add a single property requirement to the `properties` keyword.
		/// </summary>
		public static JsonSchema Vocabulary(this JsonSchema schema, SchemaVocabulary vocabulary, bool required)
		{
			var keyword = schema.OfType<VocabularyKeyword>().FirstOrDefault();

			if (keyword == null)
			{
				keyword = new VocabularyKeyword();
				schema.Add(keyword);
			}

			keyword[vocabulary] = required;

			return schema;
		}
		/// <summary>
		/// Add a `writeOnly` keyword to the schema.
		/// </summary>
		public static JsonSchema WriteOnly(this JsonSchema schema, bool isWriteOnly)
		{
			schema.Add(new WriteOnlyKeyword(isWriteOnly));

			return schema;
		}
	}
}