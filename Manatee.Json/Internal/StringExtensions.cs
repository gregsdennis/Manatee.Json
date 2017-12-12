using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Manatee.Json.Internal
{
	internal static class StringExtensions
	{
		private static readonly int[] _availableChars = Enumerable.Range(ushort.MinValue, ushort.MaxValue)
		                                                          .Select(n =>
			                                                          {
				                                                          var asChar = (char) n;
				                                                          return char.IsControl(asChar) ||
				                                                                 asChar == '\\' || asChar == '\"'
					                                                                 ? 0
					                                                                 : n;
			                                                          })
		                                                          .ToArray();
		private static readonly Regex _generalEscapePattern = new Regex("%(?<Value>[0-9A-F]{2})", RegexOptions.IgnoreCase);

		public static string EvaluateEscapeSequences(this string source, out string result)
		{
			var i = 0;
			while (i < source.Length)
			{
				var length = 1;
				if (source[i] == '\\')
					switch (source[i + 1])
					{
						case '"':
						case '/':
						case '\\':
							source = source.Remove(i, 1);
							break;
						case 'b':
							source = source.Substring(0, i) + '\b' + source.Substring(i + length + 1);
							break;
						case 'f':
							source = source.Substring(0, i) + '\f' + source.Substring(i + length + 1);
							break;
						case 'n':
							source = source.Substring(0, i) + '\n' + source.Substring(i + length + 1);
							break;
						case 'r':
							source = source.Substring(0, i) + '\r' + source.Substring(i + length + 1);
							break;
						case 't':
							source = source.Substring(0, i) + '\t' + source.Substring(i + length + 1);
							break;
						case 'u':
							length = 6;
							var hex = int.Parse(source.Substring(i + 2, 4), NumberStyles.HexNumber);
							if (source.Substring(i + 6, 2) == "\\u")
							{
								var hex2 = int.Parse(source.Substring(i + 8, 4), NumberStyles.HexNumber);
								hex = CalculateUtf32(hex, hex2);
								length += 6;
							}
							if (hex.IsValidUtf32CodePoint())
								source = source.Substring(0, i) + char.ConvertFromUtf32(hex) + source.Substring(i + length);
							else
							{
								result = null;
								return "Invalid UTF-32 code point.";
							}
							length = 2; // unicode pairs are 2 chars in .Net strings.
							break;
						default:
							result = source;
							return $"Invalid escape sequence: '\\{source[i + 1]}'.";
					}
				i += length;
			}
			result = source;
			return null;
		}

		public static int CalculateUtf32(int hex, int hex2)
		{
			return (hex - 0xD800) * 0x400 + (hex2 - 0xDC00) % 0x400 + 0x10000;
		}

		public static bool IsValidUtf32CodePoint(this int hex)
		{
			return 0 <= hex && hex <= 0x10FFFF && !(0xD800 <= hex && hex <= 0xDFFF);
		}

		public static string InsertEscapeSequences(this string source)
		{
			for (int i = 0; i < source.Length; i++)
			{
				if (_availableChars[source[i]] == 0)
				{
					var builder = StringBuilderCache.Acquire();

					builder.Append(source, 0, i);
					_InsertEscapeSequencesSlow(source, builder, i);

					return StringBuilderCache.GetStringAndRelease(builder);
				}
			}

			return source;
		}

		public static void InsertEscapeSequences(this string source, StringBuilder builder)
		{
			for (int i = 0; i < source.Length; i++)
			{
				if (_availableChars[source[i]] == 0)
				{
					builder.Append(source, 0, i);

					_InsertEscapeSequencesSlow(source, builder, i);
					return;
				}
			}

			builder.Append(source);
		}

		private static void _InsertEscapeSequencesSlow(string source, StringBuilder builder, int index)
		{ 
			for (int i = index; i < source.Length; i++)
			{
				switch (source[i])
				{
					case '"':
						builder.Append(@"\""");
						break;
					case '\\':
						builder.Append(@"\\");
						break;
					case '\b':
						builder.Append(@"\b");
						break;
					case '\f':
						builder.Append(@"\f");
						break;
					case '\n':
						builder.Append(@"\n");
						break;
					case '\r':
						builder.Append(@"\r");
						break;
					case '\t':
						builder.Append(@"\t");
						break;
					default:
						if (_availableChars[source[i]] != 0)
						{
							builder.Append(source[i]);
						}
						else
						{
							builder.Append(@"\u");
							builder.Append(((int) source[i]).ToString("X4"));
						}
						break;
				}
			}
		}

		public static string SkipWhiteSpace(this string source, ref int index, int length, out char ch)
		{
			ch = default(char);
			while (index < length)
			{
				ch = source[index];
				if (!char.IsWhiteSpace(ch)) break;
				index++;
			}

			if (index >= length)
			{
				ch = default(char);
				return "Unexpected end of input.";
			}

			return null;
		}

		public static string SkipWhiteSpace(this TextReader stream, out char ch)
		{
			ch = default(char);

			int c = stream.Peek();
			while (c != -1)
			{
				ch = (char)c;
				if (!char.IsWhiteSpace(ch)) break;
				stream.Read();
				c = stream.Peek();
			}

			if (c == -1)
			{
				ch = default(char);
				return "Unexpected end of input.";
			}

			return null;
		}

		public static async Task<(string, char)> SkipWhiteSpaceAsync(this TextReader stream, char[] scratch)
		{
			System.Diagnostics.Debug.Assert(scratch.Length >= 1);
			char ch = default(char);
			int c = stream.Peek();
			while (c != -1)
			{
				ch = (char)c;
				if (!char.IsWhiteSpace(ch)) break;
				await stream.ReadAsync(scratch, 0, 1);
				c = stream.Peek();
			}

			if (c == -1)
			{
				ch = default(char);
				return ("Unexpected end of input.", ch);
			}
			return (null, ch);
		}

		public static string UnescapePointer(this string reference)
		{
			var unescaped = reference.Replace("~1", "/")
									 .Replace("~0", "~");
			var matches = _generalEscapePattern.Matches(unescaped);
			foreach (Match match in matches)
			{
				var value = int.Parse(match.Groups["Value"].Value, NumberStyles.HexNumber);
				var ch = (char)value;
				unescaped = Regex.Replace(unescaped, match.Value, new string(ch, 1));
			}
			return unescaped;
		}

		public static Task<bool> TryRead(this TextReader stream, char[] buffer, int offset, int count)
		{
			return TryRead(stream, buffer, offset, count, CancellationToken.None);
		}

		public static async Task<bool> TryRead(this TextReader stream, char[] buffer, int offset, int count, CancellationToken token)
		{
			if (token.IsCancellationRequested)
				return false;

			var charsRead = await stream.ReadBlockAsync(buffer, offset, count);

			return count == charsRead;
		}
	}
}
