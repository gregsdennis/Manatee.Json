using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Manatee.Json.Internal
{
	internal static class StringExtensions
	{
		private static readonly IEnumerable<char> AvailableChars = Enumerable.Range(ushort.MinValue, ushort.MaxValue)
		                                                                     .Select(n => (char) n)
		                                                                     .Where(c => !char.IsControl(c));
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
								hex = (hex - 0xD800) * 0x400 + (hex2 - 0xDC00) % 0x400 + 0x10000;
								length += 6;
							}
							source = source.Substring(0, i) + char.ConvertFromUtf32(hex) + source.Substring(i + length);
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
		public static string InsertEscapeSequences(this string source)
		{
			var index = 0;
			while (index < source.Length)
			{
				var count = 0;
				string replace = null;
				switch (source[index])
				{
					case '"':
					case '\\':
						source = source.Insert(index, "\\");
						index++;
						break;
					case '\b':
						count = 1;
						replace = "\\b";
						break;
					case '\f':
						count = 1;
						replace = "\\f";
						break;
					case '\n':
						count = 1;
						replace = "\\n";
						break;
					case '\r':
						count = 1;
						replace = "\\r";
						break;
					case '\t':
						count = 1;
						replace = "\\t";
						break;
					default:
						if (!AvailableChars.Contains(source[index]))
						{
							var hex = Convert.ToInt16(source[index]).ToString("X4");
							source = source.Substring(0, index) + "\\u" + hex + source.Substring(index + 1);
							index += 5;
						}
						break;
				}
				if (replace != null)
				{
					source = _Replace(source, index, count, replace);
					index++;
				}
				index++;
			}
			return source;
		}
		private static string _Replace(string source, int index, int count, string content)
		{
			// I've checked both of these methods with ILSpy.  They occur in external methods, so
			// we're not going to do much better than this.
			return source.Remove(index, count).Insert(index, content);
		}
		public static string SkipWhiteSpace(this string source, ref int index, int length, out char ch)
		{
			if (index >= length)
			{
				ch = default(char);
				return "Unexpected end of input.";
			}
			var c = source[index];
			while (index < length)
			{
				if (!char.IsWhiteSpace(c)) break;
				index++;
				if (index >= length)
				{
					ch = default(char);
					return "Unexpected end of input.";
				}
				c = source[index];
			}
			if (index >= length)
			{
				ch = default(char);
				return "Unexpected end of input.";
			}
			ch = c;
			return null;
		}
		public static string SkipWhiteSpace(this StreamReader stream, out char ch)
		{
			if (stream.EndOfStream)
			{
				ch = default(char);
				return "Unexpected end of input.";
			}
			ch = (char) stream.Peek();
			while (!stream.EndOfStream)
			{
				if (!char.IsWhiteSpace(ch)) break;
				stream.Read();
				ch = (char) stream.Peek();
			}
			if (stream.EndOfStream)
			{
				ch = default(char);
				return "Unexpected end of input.";
			}
			return null;
		}
		public static async Task<(string, char)> SkipWhiteSpaceAsync(this StreamReader stream)
		{
			char ch;
			if (stream.EndOfStream)
			{
				ch = default(char);
				return ("Unexpected end of input.", ch);
			}
			ch = (char) stream.Peek();
			while (!stream.EndOfStream)
			{
				if (!char.IsWhiteSpace(ch)) break;
				await stream.TryRead();
				ch = (char) stream.Peek();
			}
			if (stream.EndOfStream)
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
				var ch = (char) value;
				unescaped = Regex.Replace(unescaped, match.Value, new string(ch, 1));
			}
			return unescaped;
		}

		public static async Task<(bool success, char c)> TryRead(this StreamReader stream)
		{
			var buffer = new char[1];
			var count = await stream.ReadAsync(buffer, 0, 1);
			return (count != 0, buffer[0]);
		}
	}
}