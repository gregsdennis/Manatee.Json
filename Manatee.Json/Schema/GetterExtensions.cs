using System.Collections.Generic;
using System.Linq;

namespace Manatee.Json.Schema
{
	public static class GetterExtensions
	{
		public static string Description(this JsonSchema schema)
		{
			return schema.Get<DescriptionKeyword>()?.Value;
		}
		public static JsonSchema AdditionalItems(this JsonSchema schema)
		{
			return schema.Get<AdditionalItemsKeyword>()?.Value;
		}
		public static bool ExclusiveMinimumDraft04(this JsonSchema schema)
		{
			return schema.Get<ExclusiveMinimumDraft04Keyword>()?.Value ?? false;
		}
		public static bool ExclusiveMaximumDraft04(this JsonSchema schema)
		{
			return schema.Get<ExclusiveMaximumDraft04Keyword>()?.Value ?? false;
		}
		public static Dictionary<string, JsonSchema> Properties(this JsonSchema schema)
		{
			return schema.Get<PropertiesKeyword>();
		}
		public static Dictionary<string, JsonSchema> PatternProperties(this JsonSchema schema)
		{
			return schema.Get<PatternPropertiesKeyword>();
		}
		public static JsonSchema AdditionalProperties(this JsonSchema schema)
		{
			return schema.Get<AdditionalPropertiesKeyword>()?.Value;
		}
		public static List<string> Required(this JsonSchema schema)
		{
			return schema.Get<RequiredKeyword>();
		}
		public static string Ref(this JsonSchema schema)
		{
			return schema.Get<RefKeyword>()?.Reference;
		}
		public static JsonSchema RefResolved(this JsonSchema schema)
		{
			return schema.Get<RefKeyword>()?.Resolved;
		}

		public static T Get<T>(this JsonSchema schema)
			where T : IJsonSchemaKeyword
		{
			return schema.OfType<T>().FirstOrDefault();
		}
	}
}