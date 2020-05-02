using System.Text.RegularExpressions;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Handles `uri-template` format validation by checking that the value is a well-formed regular expression string that can also validate a URI.
	/// </summary>
	public class UriTemplateFormatValidator : IFormatValidator
	{
		/// <summary>
		/// A singleton instance of the validator.
		/// </summary>
		public static IFormatValidator Instance { get; } = new UriTemplateFormatValidator();

		/// <summary>
		/// Gets the format this validator handles.
		/// </summary>
		public string Format => "uri-template";

		/// <summary>
		/// Gets the JSON Schema draft versions supported by this format.
		/// </summary>
		public JsonSchemaVersion SupportedBy => JsonSchemaVersion.All;

		private UriTemplateFormatValidator() { }

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
				var regex = new Regex(value.String);
				return regex.IsMatch("https://www.manateeopensource.com");
			}
			catch
			{
				return false;
			}
		}
	}
}