using System.Text.RegularExpressions;

namespace Manatee.Json.Internal
{
	/// <summary>
	/// Defines a URI per RFC-3986.
	/// </summary>
	public class Uri3986
	{
		private static readonly Regex _pattern = new Regex(@"^(([^:/?#]+):)?(//([^/?#]*))?([^?#]*)(\?([^#]*))?(#(.*))?");

		/// <summary>
		/// Determines whether a string represents a valid URI.
		/// </summary>
		/// <param name="source">The URI string.</param>
		/// <returns>true if valid; otherwise false.</returns>
		public static bool IsValid(string source)
		{
			return _pattern.IsMatch(source);
		}
	}
}