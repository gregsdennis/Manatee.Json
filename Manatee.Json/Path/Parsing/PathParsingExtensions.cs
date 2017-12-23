using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Manatee.Json.Internal;

namespace Manatee.Json.Path.Parsing
{
	internal static class PathParsingExtensions
	{
		#region GetKey

		public static string GetKey(this string source, ref int index, out string key)
		{
			if (source[index] == '"' || source[index] == '\'')
				return _GetQuotedKey(source, ref index, out key);

			return _GetBasicKey(source, ref index, out key);
		}

		private static string _GetBasicKey(string source, ref int index, out string key)
		{
			key = null;

			var originalIndex = index;
			var complete = false;
			while (index < source.Length)
			{
				if (char.IsLetterOrDigit(source[index]) || source[index] == '_')
					index++;
				else
				{
					complete = true;
					break;
				}
			}

			if (!complete && index + 1 < source.Length)
				return $"The character {source[index]} is not supported for unquoted names.";

			key = source.Substring(originalIndex, index - originalIndex);
			return null;
		}

		private static string _GetQuotedKey(string source, ref int index, out string key)
		{
			key = null;
			Debug.Assert(source[index] == '\'' || source[index] == '"');
			var quoteChar = source[index];
			index++;

			var originalIndex = index;
			bool complete = false, foundEscape = false;
			while (index < source.Length)
			{
				if (source[index] == '\\')
				{
					foundEscape = true;
					break;
				}

				if (source[index] == quoteChar)
				{
					complete = true;
					break;
				}

				index++;
			}

			if (foundEscape)
				return _GetQuotedKeyWithEscape(source, quoteChar, originalIndex, ref index, out key);

			if (!complete) return "Could not find end of string value.";

			key = source.Substring(originalIndex, index - originalIndex);
			index++; // swallow quote character
			return null;
		}

		private static string _GetQuotedKeyWithEscape(string source, char quoteChar, int originalIndex, ref int index, out string key)
		{
			key = null;
			string errorMessage = null;
			var builder = StringBuilderCache.Acquire();
			builder.Append(source.Substring(originalIndex, index - originalIndex));

			var complete = false;
			while (index < source.Length)
			{
				var c = source[index++];
				if (c != '\\')
				{
					if (c == quoteChar)
					{
						complete = true;
						break;
					}

					builder.Append(c);
					continue;
				}

				if (index >= source.Length) return "Could not find end of string value.";

				string append = null;
				c = source[index++];
				switch (c)
				{
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
						if (index + length >= source.Length)
						{
							errorMessage = $"Invalid escape sequence: '\\{c}{source.Substring(index)}'.";
							break;
						}

						if (!_IsValidHex(source, index, 4) ||
							!int.TryParse(source.Substring(index, 4), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var hex))
						{
							errorMessage = $"Invalid escape sequence: '\\{c}{source.Substring(index, length)}'.";
							break;
						}

						if (index + length + 2 < source.Length &&
						    source.IndexOf("\\u", index + length, 2) == index + length)
						{
							// +2 from \u
							// +4 from the next four hex chars
							length += 6;

							if (index + length >= source.Length)
							{
								errorMessage = $"Invalid escape sequence: '\\{c}{source.Substring(index)}'.";
								break;
							}

							if (!_IsValidHex(source, index + 6, 4) ||
							    !int.TryParse(source.Substring(index + 6, 4), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int hex2))
							{
								errorMessage = $"Invalid escape sequence: '\\{c}{source.Substring(index, length)}'.";
								break;
							}

							hex = StringExtensions.CalculateUtf32(hex, hex2);
						}

						if (hex.IsValidUtf32CodePoint())
							append = char.ConvertFromUtf32(hex);
						else
						{
							errorMessage = "Invalid UTF-32 code point.";
							break;
						}

						index += length;
						break;
					case '\'':
						append = "'";
						break;
					case '"':
						append = "\"";
						break;
					case '\\':
						append = "\\";
						break;
					// Is this correct?
					case '/':
						append = "/";
						break;
					default:
						complete = true;
						errorMessage = $"Invalid escape sequence: '\\{c}'.";
						break;
				}

				if (append == null) break;

				builder.Append(append);
			}

			if (!complete || errorMessage != null)
			{
				StringBuilderCache.Release(builder);
				return errorMessage ?? "Could not find end of string value.";
			}
			key = StringBuilderCache.GetStringAndRelease(builder);
			return null;
		}

		private static bool _IsValidHex(string source, int offset, int count)
		{
			for (int i = offset; i < offset + count; ++i)
			{
				// if not a hex digit
				if ((source[i] < '0' || source[i] > '9') &&
				    (source[i] < 'A' || source[i] > 'F') &&
				    (source[i] < 'a' || source[i] > 'f'))
					return false;
			}

			return true;
		}

		#endregion

		#region GetSlice

		public static string GetSlices(this string source, ref int index, out IList<Slice> slices)
		{
			string error;
			Slice lastSlice;
			slices = new List<Slice>();
			do
			{
				index++;
				error = source._GetSlice(ref index, out lastSlice);
				if (lastSlice != null)
					slices.Add(lastSlice);
			} while (error == null && lastSlice != null);

			if (error != null) return error;

			return !slices.Any() ? "Index required inside '[]'" : null;
		}

		private static string _GetSlice(this string source, ref int index, out Slice slice)
		{
			slice = null;
			if (source[index - 1] == ']') return null;

			var error = _GetInt(source, ref index, out var n1);
			if (error != null) return error;
			if (index >= source.Length) return "Expected ':', ',', or ']'.";

			if (n1.HasValue && (source[index] == ',' || source[index] == ']'))
			{
				slice = new Slice(n1.Value);
				return null;
			}

			if (source[index] != ':') return "Expected ':', ',', or ']'.";

			index++;
			error = _GetInt(source, ref index, out var n2);
			if (error != null) return error;
			if (source[index] == ',' || source[index] == ']')
			{
				slice = new Slice(n1, n2);
				return null;
			}

			if (source[index] != ':') return "Expected ':', ',', or ']'.";

			index++;
			error = _GetInt(source, ref index, out var n3);
			if (error != null) return error;

			if (source[index] == ',' || source[index] == ']')
			{
				slice = new Slice(n1, n2, n3);
				return null;
			}

			return "Expected ',' or ']'.";
		}

		private static string _GetInt(string source, ref int index, out int? number)
		{
			number = null;

			var originalIndex = index;

			// check for leading negative sign
			if (source[index] == '-')
				index++;

			while (index < source.Length)
			{
				if (char.IsDigit(source[index]))
					index++;
				else break;
			}

			if (index - originalIndex == 0 &&
			    (source[index] == ':' || source[index] == ',' || source[index] == ']'))
				return null;

			string text = source.Substring(originalIndex, index - originalIndex);
			if (!int.TryParse(text, out var value))
				return "Expected number.";

			number = value;
			return null;
		}

		#endregion

		#region GetNumber

		private enum NumberPart
		{
			LeadingInteger,
			Fraction,
			ExponentDigitsOrSign,
			ExponentDigitsOnly,
		}

		public static string GetNumber(this string source, ref int index, out double? number)
		{
			number = null;

			var originalIndex = index;

			// check for leading negative sign
			if (source[index] == '-')
				index++;

			var complete = false;
			var part = NumberPart.LeadingInteger;
			while (index < source.Length
			    && !complete)
			{
				switch (part)
				{
					case NumberPart.LeadingInteger:
						if (char.IsDigit(source[index]))
							index++;
						else if (source[index] == '.')
						{
							part = NumberPart.Fraction;
							index++;
						}
						else if (source[index] == 'e' || source[index] == 'E')
						{
							part = NumberPart.ExponentDigitsOrSign;
							index++;
						}
						else
							complete = true;
						break;
					case NumberPart.Fraction:
						if (char.IsDigit(source[index]))
							index++;
						else if (source[index] == 'e' || source[index] == 'E')
						{
							part = NumberPart.ExponentDigitsOrSign;
							index++;
						}
						else
							complete = true;
						break;
					case NumberPart.ExponentDigitsOrSign:
						if (source[index] == '-' || source[index] == '+')
						{
							part = NumberPart.ExponentDigitsOnly;
							index++;
						}
						else if (char.IsDigit(source[index]))
						{
							part = NumberPart.ExponentDigitsOnly;
							index++;
						}
						else
							complete = true;
						break;
					case NumberPart.ExponentDigitsOnly:
						if (char.IsDigit(source[index]))
						{
							part = NumberPart.ExponentDigitsOnly;
							index++;
						}
						else
							complete = true;
						break;
				}
			}

			if (index - originalIndex == 0 &&
			    (source[index] == ':' || source[index] == ',' || source[index] == ']'))
				return null;

			var text = source.Substring(originalIndex, index - originalIndex);
			if (!double.TryParse(text, out var value))
				return "Expected number.";

			number = value;
			return null;
		}

		#endregion
	}
}
