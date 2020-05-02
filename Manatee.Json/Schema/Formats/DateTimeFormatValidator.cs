using System;
using System.Globalization;

namespace Manatee.Json.Schema
{
	/// <summary>
	/// Handles `date-time` format validation by attempting to parse against several formats (ISO-8601 compatible).
	/// </summary>
	/// <remarks>
	/// The expected formats are:
	/// 
	///   - `yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffffK`
	///   - `yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'ffffffK`
	///   - `yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffK`
	///   - `yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'ffffK`
	///   - `yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffK`
	///   - `yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'ffK`
	///   - `yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fK`
	///   - `yyyy'-'MM'-'dd'T'HH':'mm':'ssK`
	///   - `yyyy'-'MM'-'dd'T'HH':'mm':'ss`
	/// </remarks>
	public class DateTimeFormatValidator : IFormatValidator
	{
		private static readonly string[] _dateTimeFormats =
			{
				"yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffffK",
				"yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'ffffffK",
				"yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffK",
				"yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'ffffK",
				"yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffK",
				"yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'ffK",
				"yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fK",
				"yyyy'-'MM'-'dd'T'HH':'mm':'ssK",
				"yyyy'-'MM'-'dd'T'HH':'mm':'ss"
			};

		/// <summary>
		/// A singleton instance of the validator.
		/// </summary>
		public static IFormatValidator Instance { get; } = new DateTimeFormatValidator();

		/// <summary>
		/// Gets the format this validator handles.
		/// </summary>
		public string Format => "date-time";

		/// <summary>
		/// Gets the JSON Schema draft versions supported by this format.
		/// </summary>
		public JsonSchemaVersion SupportedBy => JsonSchemaVersion.All;

		/// <summary>
		/// Validates a value.
		/// </summary>
		/// <param name="value">The value to validate.</param>
		/// <returns>True if <paramref name="value"/> matches the format; otherwise false.</returns>
		public virtual bool Validate(JsonValue value)
		{
			return value.Type != JsonValueType.String ||
			       DateTimeOffset.TryParseExact(value.String, _dateTimeFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
		}
	}
}