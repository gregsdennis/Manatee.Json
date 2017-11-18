using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Manatee.Json.Internal;

namespace Manatee.Json.Parsing
{
	internal class StringParser : IJsonParser
	{
		public bool Handles(char c)
		{
			return c == '\"';
		}
		public string TryParse(string source, ref int index, out JsonValue value, bool allowExtraChars)
		{
			string errorMessage = null;
			var builder = new StringBuilder();
			var sourceLength = source.Length;
			var foundEscape = false;
			var complete = false;
			index++;
			while (index < sourceLength)
			{
				var c = source[index];
				var append = new string(c, 1);
				index++;
				if (foundEscape)
				{
					switch (c)
					{
						case '"':
						case '/':
						case '\\':
							break;
						case 'b':
							append = "\b";
							break;
						case 'f':
							append = "\f";
							break;
						case 'n':
							append = "\n";
							break;
						case 'r':
							append = "\r";
							break;
						case 't':
							append = "\t";
							break;
						case 'u':
							var length = 4;
							var hex = int.Parse(source.Substring(index, 4), NumberStyles.HexNumber);
							if (source.Substring(index + 4, 2) == "\\u")
							{
								var hex2 = int.Parse(source.Substring(index + 6, 4), NumberStyles.HexNumber);
								hex = (hex - 0xD800) * 0x400 + (hex2 - 0xDC00) % 0x400 + 0x10000;
								length += 6;
							}
							append = char.ConvertFromUtf32(hex);
							index += length;
							break;
						default:
							errorMessage = $"Invalid escape sequence: '\\{c}'.";
							break;
					}
				}
				else if (c == '"')
				{
					complete = true;
					break;
				}
				foundEscape = !foundEscape && c == '\\';
				if (!foundEscape)
					builder.Append(append);
			}
			if (!complete)
			{
				value = null;
				return "Could not find end of string value.";
			}
			value = builder.ToString();
			return errorMessage;
		}
		public string TryParse(StreamReader stream, out JsonValue value)
		{
			string errorMessage = null;
			var builder = new StringBuilder();
			stream.Read(); // waste the '"'
			var foundEscape = false;
			var complete = false;
			string backup = null;
			while (!stream.EndOfStream)
			{
				char c;
				if (!string.IsNullOrEmpty(backup))
				{
					c = backup[0];
					backup = backup.Substring(1);
				}
				else c = (char) stream.Read();
				var append = new string(c, 1);
				if (foundEscape)
				{
					switch (c)
					{
						case '"':
						case '/':
						case '\\':
							break;
						case 'b':
							append = "\b";
							break;
						case 'f':
							append = "\f";
							break;
						case 'n':
							append = "\n";
							break;
						case 'r':
							append = "\r";
							break;
						case 't':
							append = "\t";
							break;
						case 'u':
							var buffer = new char[4];
							stream.Read(buffer, 0, 4);
							var hexString = new string(buffer);
							var hex = int.Parse(hexString, NumberStyles.HexNumber);
							stream.Read(buffer, 0, 2);
							backup = new string(buffer, 0, 2);
							if (backup == "\\u")
							{
								stream.Read(buffer, 0, 4);
								hexString = new string(buffer);
								backup = null;
								var hex2 = int.Parse(hexString, NumberStyles.HexNumber);
								hex = (hex - 0xD800) * 0x400 + (hex2 - 0xDC00) % 0x400 + 0x10000;
							}
							append = char.ConvertFromUtf32(hex);
							break;
						default:
							errorMessage = $"Invalid escape sequence: '\\{c}'.";
							break;
					}
				}
				else if (c == '"')
				{
					complete = true;
					break;
				}
				foundEscape = !foundEscape && c == '\\';
				if (!foundEscape)
					builder.Append(append);
			}
			if (!complete)
			{
				value = null;
				return "Could not find end of string value.";
			}
			value = builder.ToString();
			return errorMessage;
		}
		public async Task<(string errorMessage, JsonValue value)> TryParseAsync(StreamReader stream)
		{
			string errorMessage = null;
			var builder = new StringBuilder();
			stream.Read(); // waste the '"'
			var foundEscape = false;
			var complete = false;
			string backup = null;
			while (!stream.EndOfStream)
			{
				char c;
				if (!string.IsNullOrEmpty(backup))
				{
					c = backup[0];
					backup = backup.Substring(1);
				}
				else
				{
					var result = await stream.TryRead();
					if (result.success) break;
					c = result.c;
				}
				var append = new string(c, 1);
				if (foundEscape)
				{
					switch (c)
					{
						case '"':
						case '/':
						case '\\':
							break;
						case 'b':
							append = "\b";
							break;
						case 'f':
							append = "\f";
							break;
						case 'n':
							append = "\n";
							break;
						case 'r':
							append = "\r";
							break;
						case 't':
							append = "\t";
							break;
						case 'u':
							var buffer = new char[4];
							stream.Read(buffer, 0, 4);
							var hexString = new string(buffer);
							var hex = int.Parse(hexString, NumberStyles.HexNumber);
							stream.Read(buffer, 0, 2);
							backup = new string(buffer, 0, 2);
							if (backup == "\\u")
							{
								stream.Read(buffer, 0, 4);
								hexString = new string(buffer);
								backup = null;
								var hex2 = int.Parse(hexString, NumberStyles.HexNumber);
								hex = (hex - 0xD800) * 0x400 + (hex2 - 0xDC00) % 0x400 + 0x10000;
							}
							append = char.ConvertFromUtf32(hex);
							break;
						default:
							errorMessage = $"Invalid escape sequence: '\\{c}'.";
							break;
					}
				}
				else if (c == '"')
				{
					complete = true;
					break;
				}
				foundEscape = !foundEscape && c == '\\';
				if (!foundEscape)
					builder.Append(append);
			}
			if (!complete)
			{
				return ("Could not find end of string value.", null);
			}
			var value = builder.ToString();
			return (errorMessage, value);
		}
	}
}