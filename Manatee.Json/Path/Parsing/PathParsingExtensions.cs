using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Manatee.Json.Internal;

namespace Manatee.Json.Path.Parsing
{
	internal static class PathParsingExtensions
	{
		#region GetKey

		public static bool TryGetKey(this string source, ref int index, [NotNullWhen(true)] out string key, [NotNullWhen(false)] out string errorMessage)
		{
			if (source[index] == '"' || source[index] == '\'')
				return _TryGetQuotedKey(source, ref index, out key, out errorMessage);

			return _TryGetBasicKey(source, ref index, out key, out errorMessage);
		}

		private static bool _TryGetBasicKey(string source, ref int index, [NotNullWhen(true)] out string key, [NotNullWhen(false)] out string errorMessage)
		{

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
			{
				key = null!;
				errorMessage = $"The character {source[index]} is not supported for unquoted names.";
				return false;
			}

			key = source.Substring(originalIndex, index - originalIndex);
			errorMessage = null!;
			return true;
		}

		private static bool _TryGetQuotedKey(string source, ref int index, [NotNullWhen(true)] out string key, [NotNullWhen(false)] out string errorMessage)
		{
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
				return _TryGetQuotedKeyWithEscape(source, quoteChar, originalIndex, ref index, out key, out errorMessage);

			if (!complete)
			{
				key = null!;
				errorMessage = "Could not find end of string value.";
				return false;
			}

			key = source.Substring(originalIndex, index - originalIndex);
			errorMessage = null!;
			index++; // swallow quote character
			return true;
		}

		private static bool _TryGetQuotedKeyWithEscape(string source, char quoteChar, int originalIndex, ref int index, [NotNullWhen(true)] out string key, [NotNullWhen(false)] out string errorMessage)
		{
			var builder = StringBuilderCache.Acquire();
			builder.Append(source.Substring(originalIndex, index - originalIndex));

			var complete = false;
			errorMessage = null!;
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

				if (index >= source.Length)
				{
					key = null!;
					errorMessage = "Could not find end of string value.";
					return false;
				}

				string? append = null;
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
				key = null!;
				errorMessage ??= "Could not find end of string value.";
				return false;
			}

			key = StringBuilderCache.GetStringAndRelease(builder);
			return true;
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

		public static bool TryGetSlices(this string source, ref int index, out IList<Slice> slices, [NotNullWhen(false)] out string errorMessage)
		{
			Slice? lastSlice;
			slices = new List<Slice>();
			do
			{
				index++;
				if (source._TryGetSlice(ref index, out lastSlice, out errorMessage) && lastSlice != null)
					slices.Add(lastSlice);
			} while (errorMessage == null && lastSlice != null);

			if (errorMessage != null) return false;
			if (slices.Any()) return true;

			errorMessage = "Index required inside '[]'";
			return false;

		}

		private static bool _TryGetSlice(this string source, ref int index, out Slice? slice, [NotNullWhen(false)] out string errorMessage)
		{
			slice = null;
			if (source[index - 1] == ']')
			{
				errorMessage = null!;
				return true;
			}

			if (!_TryGetInt(source, ref index, out var n1, out errorMessage)) return false;
			if (index >= source.Length)
			{
				errorMessage = "Expected ':', ',', or ']'.";
				return false;
			}

			if (n1.HasValue && (source[index] == ',' || source[index] == ']'))
			{
				slice = new Slice(n1.Value);
				return true;
			}

			if (source[index] != ':')
			{
				errorMessage = "Expected ':', ',', or ']'.";
				return false;
			}

			index++;
			if (!_TryGetInt(source, ref index, out var n2, out errorMessage)) return false;
			if (source[index] == ',' || source[index] == ']')
			{
				slice = new Slice(n1, n2);
				return true;
			}

			if (source[index] != ':')
			{
				errorMessage = "Expected ':', ',', or ']'.";
				return false;
			}

			index++;
			if (!_TryGetInt(source, ref index, out var n3, out errorMessage)) return false;
			if (source[index] == ',' || source[index] == ']')
			{
				slice = new Slice(n1, n2, n3);
				return true;
			}

			errorMessage = "Expected ',' or ']'.";
			return false;
		}

		private static bool _TryGetInt(string source, ref int index, out int? number, [NotNullWhen(false)] out string errorMessage)
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
			{
				errorMessage = null!;
				return true;
			}

			string text = source.Substring(originalIndex, index - originalIndex);
			if (!int.TryParse(text, out var value))
			{
				errorMessage = "Expected number.";
				return false;
			}

			number = value;
			errorMessage = null!;
			return true;
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

		public static bool TryGetNumber(this string source, ref int index, out double number, [NotNullWhen(false)] out string errorMessage)
		{
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
			{
				number = 0;
				errorMessage = null!;
				return true;
			}

			var text = source.Substring(originalIndex, index - originalIndex);
			if (!double.TryParse(text, out var value))
			{
				number = default;
				errorMessage = "Expected number.";
				return false;
			}

			number = value;
			errorMessage = null!;
			return true;
		}

		#endregion
	}
}
