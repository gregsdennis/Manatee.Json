using System.Linq;

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
		public static JsonSchema Description(this JsonSchema schema, string description)
		{
			schema.Add(new DescriptionKeyword(description));

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
		public static JsonSchema Enum(this JsonSchema schema, params JsonValue[] values)
		{
			schema.Add(new EnumKeyword(values));

			return schema;
		}
		public static JsonSchema Default(this JsonSchema schema, JsonValue value)
		{
			schema.Add(new DefaultKeyword(value));

			return schema;
		}
		public static JsonSchema Minimum(this JsonSchema schema, int minimum)
		{
			schema.Add(new MinimumKeyword(minimum));

			return schema;
		}
		public static JsonSchema Maximum(this JsonSchema schema, int maximum)
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
		public static JsonSchema UniqueItems(this JsonSchema schema, bool unique = true)
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
	}
}