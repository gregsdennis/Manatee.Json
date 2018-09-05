using System.Collections.Generic;
using System.Linq;

namespace Manatee.Json.Schema
{
	public static class GetterExtensions
	{
		public static string Description(this JsonSchema schema)
		{
			return schema.OfType<DescriptionKeyword>().FirstOrDefault()?.Value;
		}
		public static JsonSchema AdditionalItems(this JsonSchema schema)
		{
			return schema.OfType<AdditionalItemsKeyword>().FirstOrDefault()?.Value;
		}
		public static bool ExclusiveMinimumDraft04(this JsonSchema schema)
		{
			return schema.OfType<ExclusiveMinimumDraft04Keyword>().FirstOrDefault()?.Value ?? false;
		}
		public static bool ExclusiveMaximumDraft04(this JsonSchema schema)
		{
			return schema.OfType<ExclusiveMaximumDraft04Keyword>().FirstOrDefault()?.Value ?? false;
		}
		public static Dictionary<string, JsonSchema> Properties(this JsonSchema schema)
		{
			return schema.OfType<PropertiesKeyword>().FirstOrDefault();
		}
		public static Dictionary<string, JsonSchema> PatternProperties(this JsonSchema schema)
		{
			return schema.OfType<PatternPropertiesKeyword>().FirstOrDefault();
		}
		public static JsonSchema AdditionalProperties(this JsonSchema schema)
		{
			return schema.OfType<AdditionalPropertiesKeyword>().FirstOrDefault()?.Value;
		}
		public static List<string> Required(this JsonSchema schema)
		{
			return schema.OfType<RequiredKeyword>().FirstOrDefault();
		}
	}
}