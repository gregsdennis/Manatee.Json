namespace Manatee.Json.Schema
{
	public static class SchemaErrorMessages
	{
		public static string Const { get; set; } = "Expected: {{expected}}; Actual: {{value}}";
		public static string Contains { get; set; } = "Expected an item that matched '{{expected}}' but no items were found";
		public static string Definitions { get; set; } = "Property 'definitions' must contain an object";
		public static string Enum { get; set; } = "'{{actual}}' does not match the required value";
		public static string Format { get; set; } = "Value '{{actual}}' is not in an acceptable {{format}} format";
		public static string Then { get; set; } = "Validation of `if` succeeded, but validation of `then` failed";
		public static string Else { get; set; } = "Validation of `if` failed, but validation of `else` also failed";
		public static string Items { get; set; } = "Schema indicates no additional items are allowed";
		public static string ExclusiveMaximum { get; set; } = "Expected: < {{expected}}; Actual: {{value}}";
		public static string Maximum { get; set; } = "Expected: <= {{expected}}; Actual: {{value}}";
		public static string ExclusiveMinimum { get; set; } = "Expected: > {{expected}}; Actual: {{value}}";
		public static string Minimum { get; set; } = "Expected: >= {{expected}}; Actual: {{value}}";
		public static string MaxItems { get; set; } = "Expected: <= {{expected}} items; Actual: {{actual}} items";
		public static string MinItems { get; set; } = "Expected: >= {{expected}} items; Actual: {{actual}} items";
		public static string MaxLength { get; set; } = "Expected: <= {{expected}} characters; Actual: {{actual}} characters";
		public static string MinLength { get; set; } = "Expected: <= {{expected}} characters; Actual: {{actual}} characters";
		public static string MaxProperties { get; set; } = "Expected: <= {{expected}} properties; Actual: {{actual}} properties";
		public static string MinProperties { get; set; } = "Expected: >= {{expected}} properties; Actual: {{actual}} properties";
		public static string MultipleOf { get; set; } = "Expected: {{value}}%{{multipleOf}}=0; Actual: {{actual}}";
		public static string Not { get; set; } = "Expected schema to be invalid, but was valid";
		public static string OneOf { get; set; } = "More than one option was valid";
		public static string Pattern { get; set; } = "Value '{{value}}' does not match required Regex pattern '{{pattern}}'";
		public static string AdditionalProperties_False { get; set; } = "Additional properties are not allowed";
		public static string Required { get; set; } = "Required property not found";
		public static string Type { get; set; } = "Expected Type: {{expected}}; Actual Type: {{actual}}";
		public static string UniqueItems { get; set; } = "Expected unique items; Duplicates were found.";
	}
}
