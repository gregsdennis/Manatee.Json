using System;

namespace Manatee.Json.Path
{
	/// <summary>
	/// Thrown when an input string contains a syntax error while parsing a <see cref="JsonPath"/>.
	/// </summary>
	public class JsonPathSyntaxException : Exception
	{
		/// <summary>
		/// Gets the path up to the point at which the error was found.
		/// </summary>
		public string? Path { get; }

		/// <summary>
		/// Gets a message that describes the current exception.
		/// </summary>
		/// <returns>
		/// The error message that explains the reason for the exception, or an empty string("").
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public override string Message => $"{base.Message} Path up to error: '{Path}'";

		internal JsonPathSyntaxException(JsonPath? path, string message)
			: base(message)
		{
			Path = path?.ToString();
		}
	}
}