using System;
using System.Globalization;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Handles `date` format validation by attempting to parse against the `yyyy-MM-dd` format (ISO-8601 compatible).
	/// </summary>
	public class DateFormatValidator : IFormatValidator
	{
		/// <summary>
		/// A singleton instance of the validator.
		/// </summary>
		public static IFormatValidator Instance { get; } = new DateFormatValidator();

		/// <summary>
		/// Gets the format this validator handles.
		/// </summary>
		public string Format => "date";

		/// <summary>
		/// Gets the JSON Schema draft versions supported by this format.
		/// </summary>
		public JsonSchemaVersion SupportedBy => JsonSchemaVersion.Draft2019_09;

		private DateFormatValidator(){}

		/// <summary>
		/// Validates a value.
		/// </summary>
		/// <param name="value">The value to validate.</param>
		/// <returns>True if <paramref name="value"/> matches the format; otherwise false.</returns>
		public virtual bool Validate(JsonValue value)
		{
			return value.Type != JsonValueType.String ||
			       DateTime.TryParseExact(value.String, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
		}
	}
}