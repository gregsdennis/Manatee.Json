using System;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Handles `iri` format validation by checking that the value is a well-formed relative or absolute URI string.
	/// </summary>
	public class IriFormatValidator : IFormatValidator
	{
		/// <summary>
		/// A singleton instance of the validator.
		/// </summary>
		public static IFormatValidator Instance { get; } = new IriFormatValidator();

		/// <summary>
		/// Gets the format this validator handles.
		/// </summary>
		public string Format => "iri";

		/// <summary>
		/// Gets the JSON Schema draft versions supported by this format.
		/// </summary>
		public JsonSchemaVersion SupportedBy => JsonSchemaVersion.Draft2019_09;

		private IriFormatValidator() { }

		/// <summary>
		/// Validates a value.
		/// </summary>
		/// <param name="value">The value to validate.</param>
		/// <returns>True if <paramref name="value"/> matches the format; otherwise false.</returns>
		public bool Validate(JsonValue value)
		{
			return value.Type != JsonValueType.String || Uri.IsWellFormedUriString(value.String, UriKind.RelativeOrAbsolute);
		}
	}
}