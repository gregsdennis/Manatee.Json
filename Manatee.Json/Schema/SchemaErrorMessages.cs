using System;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Defines tokenized error messages for schema validations.
	/// </summary>
	/// <remarks>
	/// Tokens are denoted using a set of double curly braces (e.g. `{{value}}`. All messages support a "value" token, which is the validated value.  Others are declared in the notes for each message.
	/// Properties which have no error message listed here simply forward error messages from children schemata.
	/// </remarks>
	[Obsolete("Customizing error messages can now be performed directly on their respective keyword classes.")]
	public static class SchemaErrorMessages
	{
		/// <summary>
		/// The error message used for the `const` property.
		/// </summary>
		/// <remarks>
		/// Supports the following tokens:
		/// - expected
		/// </remarks>
		public static string Const { get; set; } = "Expected: {{expected}}; Actual: {{value}}";
		/// <summary>
		/// The error message used for the `contains` property.
		/// </summary>
		/// <remarks>
		/// Supports the following tokens:
		/// - expected
		/// </remarks>
		public static string Contains { get; set; } = "Expected an item that matched '{{expected}}' but no items were found";
		/// <summary>
		/// The error message used for the `definitions` property.
		/// </summary>
		/// <remarks>
		/// Supports the following tokens:
		/// Supports no other tokens.
		/// </remarks>
		public static string Definitions { get; set; } = "Property 'definitions' must contain an object";
		/// <summary>
		/// The error message used for the `enum` property.
		/// </summary>
		/// <remarks>
		/// Supports the following tokens:
		/// Supports no other tokens.
		/// </remarks>
		public static string Enum { get; set; } = "'{{value}}' does not match any of the required values";
		/// <summary>
		/// The error message used for the `format` property.
		/// </summary>
		/// <remarks>
		/// Supports the following tokens:
		/// - format
		/// </remarks>
		public static string Format { get; set; } = "Value '{{value}}' is not in an acceptable {{format}} format";
		/// <summary>
		/// The error message used for the `then` property (only used when the `if` property is present and succeeds).
		/// </summary>
		/// <remarks>
		/// Supports the following tokens:
		/// Supports no other tokens.
		/// </remarks>
		public static string Then { get; set; } = "Validation of `if` succeeded, but validation of `then` failed";
		/// <summary>
		/// The error message used for the `else` property (only used when the `if` property is present and fails).
		/// </summary>
		/// <remarks>
		/// Supports the following tokens:
		/// Supports no other tokens.
		/// </remarks>
		public static string Else { get; set; } = "Validation of `if` failed, but validation of `else` also failed";
		/// <summary>
		/// The error message used for the `items` property.
		/// </summary>
		/// <remarks>
		/// Supports no other tokens.
		/// </remarks>
		public static string Items { get; set; } = "Schema indicates no additional items are allowed";
		/// <summary>
		/// The error message used for the `exclusiveMaximum` property.
		/// </summary>
		/// <remarks>
		/// Supports the following tokens:
		/// - expected
		/// </remarks>
		public static string ExclusiveMaximum { get; set; } = "Expected: < {{expected}}; Actual: {{value}}";
		/// <summary>
		/// The error message used for the `maximum` property.
		/// </summary>
		/// <remarks>
		/// Supports the following tokens:
		/// - expected
		/// </remarks>
		public static string Maximum { get; set; } = "Expected: <= {{expected}}; Actual: {{value}}";
		/// <summary>
		/// The error message used for the `exclusiveMinimum` property.
		/// </summary>
		/// <remarks>
		/// Supports the following tokens:
		/// - expected
		/// </remarks>
		public static string ExclusiveMinimum { get; set; } = "Expected: > {{expected}}; Actual: {{value}}";
		/// <summary>
		/// The error message used for the `minimum` property.
		/// </summary>
		/// <remarks>
		/// Supports the following tokens:
		/// - expected
		/// </remarks>
		public static string Minimum { get; set; } = "Expected: >= {{expected}}; Actual: {{value}}";
		/// <summary>
		/// The error message used for the `maxItems` property.
		/// </summary>
		/// <remarks>
		/// Supports the following tokens:
		/// - expected
		/// - actual
		/// </remarks>
		public static string MaxItems { get; set; } = "Expected: <= {{expected}} items; Actual: {{actual}} items";
		/// <summary>
		/// The error message used for the `minItems` property.
		/// </summary>
		/// <remarks>
		/// Supports the following tokens:
		/// - expected
		/// - actual
		/// </remarks>
		public static string MinItems { get; set; } = "Expected: >= {{expected}} items; Actual: {{actual}} items";
		/// <summary>
		/// The error message used for the `maxLength` property.
		/// </summary>
		/// <remarks>
		/// Supports the following tokens:
		/// - expected
		/// - actual
		/// </remarks>
		public static string MaxLength { get; set; } = "Expected: <= {{expected}} characters; Actual: {{actual}} characters";
		/// <summary>
		/// The error message used for the `minLength` property.
		/// </summary>
		/// <remarks>
		/// Supports the following tokens:
		/// - expected
		/// - actual
		/// </remarks>
		public static string MinLength { get; set; } = "Expected: <= {{expected}} characters; Actual: {{actual}} characters";
		/// <summary>
		/// The error message used for the `maxProperties` property.
		/// </summary>
		/// <remarks>
		/// Supports the following tokens:
		/// - expected
		/// - actual
		/// </remarks>
		public static string MaxProperties { get; set; } = "Expected: <= {{expected}} properties; Actual: {{actual}} properties";
		/// <summary>
		/// The error message used for the `minProperties` property.
		/// </summary>
		/// <remarks>
		/// Supports the following tokens:
		/// - expected
		/// - actual
		/// </remarks>
		public static string MinProperties { get; set; } = "Expected: >= {{expected}} properties; Actual: {{actual}} properties";
		/// <summary>
		/// The error message used for the `multipleOf` property.
		/// </summary>
		/// <remarks>
		/// Supports the following tokens:
		/// - multipleOf
		/// - actual
		/// </remarks>
		public static string MultipleOf { get; set; } = "Expected: {{value}}%{{multipleOf}}=0; Actual: {{actual}}";
		/// <summary>
		/// The error message used for the `not` property.
		/// </summary>
		/// <remarks>
		/// Supports no other tokens.
		/// </remarks>
		public static string Not { get; set; } = "Expected schema to be invalid, but was valid";
		/// <summary>
		/// The error message used for the `oneOf` property.
		/// </summary>
		/// <remarks>
		/// Supports no other tokens.
		/// </remarks>
		public static string OneOf { get; set; } = "More than one option was valid";
		/// <summary>
		/// The error message used for the `pattern` property.
		/// </summary>
		/// <remarks>
		/// Supports the following tokens:
		/// - pattern
		/// </remarks>
		public static string Pattern { get; set; } = "Value '{{value}}' does not match required Regex pattern '{{pattern}}'";
		/// <summary>
		/// The error message used for the `additionalProperties` property when the property's value is false.
		/// </summary>
		/// <remarks>
		/// Supports no other tokens.
		/// </remarks>
		// ReSharper disable once InconsistentNaming
		public static string AdditionalProperties_False { get; set; } = "Additional properties are not allowed";
		/// <summary>
		/// The error message used for the `required` property.
		/// </summary>
		/// <remarks>
		/// Supports no other tokens.
		/// </remarks>
		public static string Required { get; set; } = "Required property not found";
		/// <summary>
		/// The error message used for the `type` property.
		/// </summary>
		/// <remarks>
		/// Supports the following tokens:
		/// - expected
		/// - actual
		/// </remarks>
		public static string Type { get; set; } = "Expected Type: {{expected}}; Actual Type: {{actual}}";
		/// <summary>
		/// The error message used for the `uniqueItems` property.
		/// </summary>
		/// <remarks>
		/// Supports no other tokens.
		/// </remarks>
		public static string UniqueItems { get; set; } = "Expected unique items; Duplicates were found.";
	}
}
