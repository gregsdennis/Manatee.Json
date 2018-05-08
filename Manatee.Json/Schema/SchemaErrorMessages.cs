namespace Manatee.Json.Schema
{
	public static class SchemaErrorMessages
	{
		public static string ConstNotEqual { get; set; } = "Expected: {{expected}}; Actual: {{actual}}.";
		public static string ContainsFoundNoMatch { get; set; } = "Expected an item that matched '{{expected}}' but no items were found.";
		public static string DefinitionsShouldBeObject { get; set; } = "Property 'definitions' must contain an object.";
		public static string EnumValueMismatch { get; set; } = "'{{actual}}' does not match the required value.";
		public static string FormatMismatch { get; set; } = "'Value [{{actual}}] is not in an acceptable {{format}} format.";
	}
}
