using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Manatee.Json.Internal
{
	internal static class StringExtensions
	{
		private static readonly IEnumerable<char> AvailableChars = Enumerable.Range(UInt16.MinValue, UInt16.MaxValue)
		                                                                     .Select(n => (char) n)
		                                                                     .Where(c => !Char.IsControl(c));

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
							var hex = Int32.Parse(source.Substring(i + 2, 4), NumberStyles.HexNumber);
							if (source.Substring(i + 6, 2) == "\\u")
							{
								var hex2 = Int32.Parse(source.Substring(i + 8, 4), NumberStyles.HexNumber);
								hex = (hex - 0xD800) * 0x400 + (hex2 - 0xDC00) % 0x400 + 0x10000;
								length += 6;
							}
							source = source.Substring(0, i) + Char.ConvertFromUtf32(hex) + source.Substring(i + length);
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
						if (!Enumerable.Contains(AvailableChars, source[index]))
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
				if (!Char.IsWhiteSpace(c)) break;
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
				if (!Char.IsWhiteSpace(ch)) break;
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
	}
}