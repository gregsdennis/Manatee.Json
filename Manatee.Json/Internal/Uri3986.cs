using System.Text.RegularExpressions;

namespace Manatee.Json.Internal
{
    public class Uri3986
    {
        private static readonly Regex _pattern = new Regex(@"^(([^:/?#]+):)?(//([^/?#]*))?([^?#]*)(\?([^#]*))?(#(.*))?");

        public static bool IsValid(string source)
        {
            return _pattern.IsMatch(source);
        }
    }
}