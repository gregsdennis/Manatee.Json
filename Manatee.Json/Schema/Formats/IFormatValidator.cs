namespace Manatee.Json.Schema
{
	/// <summary>
	/// Defines properties and methods for `format` keyword validation.
	/// </summary>
	public interface IFormatValidator
	{
		/// <summary>
		/// Gets the format this validator handles.
		/// </summary>
		string Format { get; }

		/// <summary>
		/// Gets the JSON Schema draft versions supported by this format.
		/// </summary>
		JsonSchemaVersion SupportedBy { get; }

		/// <summary>
		/// Validates a value.
		/// </summary>
		/// <param name="value">The value to validate.</param>
		/// <returns>True if <paramref name="value"/> matches the format; otherwise false.</returns>
		bool Validate(JsonValue value);
	}
}