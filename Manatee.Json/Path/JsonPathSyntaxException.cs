using System;

namespace Manatee.Json.Path
{
	/// <summary>
	/// Thrown when an input string contains a syntax error while parsing a <see cref="JsonPath"/>.
	/// </summary>
	public class JsonPathSyntaxException : Exception
	{
		private readonly bool _isExpression;
		
		/// <summary>
		/// Gets the path up to the point at which the error was found.
		/// </summary>
		public string Path { get; }

		/// <summary>
		/// Gets a message that describes the current exception.
		/// </summary>
		/// <returns>
		/// The error message that explains the reason for the exception, or an empty string("").
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public override string Message => string.Format(_isExpression ? "{0} Expression up to error: '{1}'" : "{0} Path up to error: '{1}'", base.Message, Path);

		internal JsonPathSyntaxException(JsonPath path, string message)
			: base(message)
		{
			Path = path.ToString();
		}
		internal JsonPathSyntaxException(string expression, string format, params object[] parameters)
			: base(string.Format(format, parameters))
		{
			_isExpression = true;
			Path = expression;
		}
	}
}