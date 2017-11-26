using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Manatee.Json.Parsing
{
	internal interface IJsonParser
	{
		bool Handles(char c);
		// returns error message, if any.  Null return implies success.
		string TryParse(string source, ref int index, out JsonValue value, bool allowExtraChars);
		string TryParse(StreamReader stream, out JsonValue value);
		Task<(string errorMessage, JsonValue value)> TryParseAsync(StreamReader stream, CancellationToken token);
	}
}
