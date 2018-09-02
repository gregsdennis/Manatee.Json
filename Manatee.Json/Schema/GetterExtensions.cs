using System.Linq;

namespace Manatee.Json.Schema
{
	public static class GetterExtensions
	{
		public static string Description(this JsonSchema schema)
		{
			return (schema as JsonSchema)?.OfType<DescriptionKeyword>().FirstOrDefault()?.Value;
		}
		public static JsonSchema AdditionalItems(this JsonSchema schema)
		{
			return (schema as JsonSchema)?.OfType<AdditionalItemsKeyword>().FirstOrDefault()?.Value;
		}
		public static bool ExclusiveMinimumDraft04(this JsonSchema schema)
		{
			return (schema as JsonSchema)?.OfType<ExclusiveMinimumDraft04Keyword>().FirstOrDefault()?.Value ?? false;
		}
		public static bool ExclusiveMaximumDraft04(this JsonSchema schema)
		{
			return (schema as JsonSchema)?.OfType<ExclusiveMaximumDraft04Keyword>().FirstOrDefault()?.Value ?? false;
		}
	}
}