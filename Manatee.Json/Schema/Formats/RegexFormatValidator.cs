using System.Text.RegularExpressions;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Handles `regex` format validation by checking that the value is a well-formed regular expression string.
	/// </summary>
	public class RegexFormatValidator : IFormatValidator
	{
		/// <summary>
		/// A singleton instance of the validator.
		/// </summary>
		public static IFormatValidator Instance { get; } = new RegexFormatValidator();

		/// <summary>
		/// Gets the format this validator handles.
		/// </summary>
		public string Format => "regex";

		/// <summary>
		/// Gets the JSON Schema draft versions supported by this format.
		/// </summary>
		public JsonSchemaVersion SupportedBy => JsonSchemaVersion.All;

		private RegexFormatValidator() { }

		/// <summary>
		/// Validates a value.
		/// </summary>
		/// <param name="value">The value to validate.</param>
		/// <returns>True if <paramref name="value"/> matches the format; otherwise false.</returns>
		public bool Validate(JsonValue value)
		{
			try
			{
				if (value.Type != JsonValueType.String) return true;
				// ReSharper disable once ObjectCreationAsStatement
				new Regex(value.String);
				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}