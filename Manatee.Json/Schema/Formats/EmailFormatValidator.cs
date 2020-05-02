namespace Manatee.Json.Schema
{
	/// <summary>
	/// Handles `email` validation by using a regular expression.
	/// </summary>
	/// <remarks>
	/// The regular expression is sourced from http://www.regular-expressions.info/email.html.
	/// </remarks>
	public class EmailFormatValidator : RegexBasedFormatValidator
	{
		/// <summary>
		/// A singleton instance of the validator.
		/// </summary>
		public static IFormatValidator Instance { get; } = new EmailFormatValidator();

		/// <summary>
		/// Gets the format this validator handles.
		/// </summary>
		public override string Format => "email";

		/// <summary>
		/// Gets the JSON Schema draft versions supported by this format.
		/// </summary>
		public override JsonSchemaVersion SupportedBy => JsonSchemaVersion.All;

		private EmailFormatValidator()
			: base(@"^(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|""(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*"")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])$") { }
	}
}