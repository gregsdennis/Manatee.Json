using System.IO;
using System.Threading.Tasks;
using Manatee.Json.Internal;

namespace Manatee.Json.Parsing
{
	internal class NullParser : IJsonParser
	{
		public bool Handles(char c)
		{
			return c.In('n', 'N');
		}
		public string TryParse(string source, ref int index, out JsonValue value, bool allowExtraChars)
		{
			var buffer = new char[4];
			for (var i = 0; i < 4 && index + i < source.Length; i++)
			{
				buffer[i] = source[index + i];
			}
			var result = new string(buffer).ToLower();
			if (result != "null")
			{
				value = null;
				return $"Value not recognized: '{result}'.";
			}
			index += 4;
			value = JsonValue.Null;
			return null;
		}
		public string TryParse(StreamReader stream, out JsonValue value)
		{
			var buffer = new char[4];
			for (var i = 0; i < 4 && !stream.EndOfStream; i++)
			{
				buffer[i] = (char) stream.Read();
			}
			if (buffer[3] == (char) 0 && stream.EndOfStream)
			{
				value = null;
				return "Unexpected end of input.";
			}
			var result = new string(buffer).ToLower();
			if (result != "null")
			{
				value = null;
				return $"Value not recognized: '{result}'.";
			}
			value = JsonValue.Null;
			return null;
		}
		public async Task<(string errorMessage, JsonValue value)> TryParseAsync(StreamReader stream)
		{
			var buffer = new char[4];
			var count = await stream.ReadAsync(buffer, 0, 4);
			if (count < 4)
				return ("Unexpected end of input.", null);
			var result = new string(buffer).ToLower();
			if (result != "null")
				return ($"Value not recognized: '{result}'.", null);
			return (null, JsonValue.Null);
		}
	}
}