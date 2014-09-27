/***************************************************************************************

	Copyright 2012 Greg Dennis

	   Licensed under the Apache License, Version 2.0 (the "License");
	   you may not use this file except in compliance with the License.
	   You may obtain a copy of the License at

		 http://www.apache.org/licenses/LICENSE-2.0

	   Unless required by applicable law or agreed to in writing, software
	   distributed under the License is distributed on an "AS IS" BASIS,
	   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	   See the License for the specific language governing permissions and
	   limitations under the License.
 
	File Name:		GeneralExtensions.cs
	Namespace:		Manatee.Json.Internal
	Class Name:		GeneralExtensions
	Purpose:		General-use extension methods for the library.

***************************************************************************************/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Manatee.Json.Internal
{
	internal static class GeneralExtensions
	{
		public const string EscapeChars = @"\""/bfnrtu";

		private static readonly IEnumerable<char> AvailableChars = Enumerable.Range(ushort.MinValue, ushort.MaxValue)
																			 .Select(n => (char)n)
																			 .Where(c => !char.IsControl(c));

		public static bool In<T>(this T value, params T[] collection)
		{
			return collection.Contains(value);
		}
		public static bool IsInt(this double value)
		{
			return Math.Ceiling(value) == Math.Floor(value);
		}
		public static bool IsNullOrWhiteSpace(this string value)
		{
#if NET35 || NET35C
			return string.IsNullOrEmpty(value) || string.IsNullOrEmpty(value.Trim());
#elif NET4 || NET4C || NET45
			return string.IsNullOrWhiteSpace(value);
#endif
		}
		public static string Join<T>(this IEnumerable<T> segments, string separator)
		{
#if NET35 || NET35C
			return string.Join(separator, segments.Select(s => s.ToString()).ToArray());
#elif NET4 || NET4C || NET45
			return string.Join(separator, segments);
#endif
		}
		public static IEnumerable<T> NotNull<T>(this IEnumerable<T> items)
			where T : class
		{
			return items.Where(i => i != null);
		}
		public static string StripExternalSpaces(this string s)
		{
			var getNonDelimitedQuote = new Regex("((([^\"]*\\\\\\\")*)|([^\"]*))[^\"]*(\\\"|$)");
			var whitespace = new Regex("\\s+");
			var match = getNonDelimitedQuote.Match(s);
			var remove = true;
			var sb = new StringBuilder();
			while (match.Success)
			{
				sb.Append(remove ? whitespace.Replace(match.Value, string.Empty) : match.Value);
				remove = !remove;
				match = match.NextMatch();
			}
			return sb.ToString();
		}
		public static string EvaluateEscapeSequences(this string s)
		{
			var i = 0;
			int length;
			while (i < s.Length)
			{
				length = 1;
				if (s[i] == '\\')
					switch (s[i + 1])
					{
						case '"':
						case '/':
						case '\\':
							s = s.Remove(i, 1);
							break;
						case 'b':
							s = s.Substring(0, i) + '\b' + s.Substring(i + length + 1);
							break;
						case 'f':
							s = s.Substring(0, i) + '\f' + s.Substring(i + length + 1);
							break;
						case 'n':
							s = s.Substring(0, i) + '\n' + s.Substring(i + length + 1);
							break;
						case 'r':
							s = s.Substring(0, i) + '\r' + s.Substring(i + length + 1);
							break;
						case 't':
							s = s.Substring(0, i) + '\t' + s.Substring(i + length + 1);
							break;
						case 'u':
							length = 6;
							var hex = int.Parse(s.Substring(i + 2, 4), NumberStyles.HexNumber);
							if (s.Substring(i + 6, 2) == "\\u")
							{
								var hex2 = int.Parse(s.Substring(i + 8, 4), NumberStyles.HexNumber);
								hex = (hex2 - 0xDC00) + ((hex - 0xD800) << 10);
								length += 6;
							}
							s = s.Substring(0, i) + char.ConvertFromUtf32(hex) + s.Substring(i + length);
							break;
					}
				i += length;
			}
			return s;
		}
		public static string InsertEscapeSequences(this string s)
		{
			var i = 0;
			while (i < s.Length)
			{
				switch (s[i])
				{
					case '"':
					case '\\':
						s = s.Insert(i, "\\");
						i++;
						break;
					case '\b':
						s = s.Substring(0, i) + "\\b" + s.Substring(i + 1);
						i++;
						break;
					case '\f':
						s = s.Substring(0, i) + "\\f" + s.Substring(i + 1);
						i++;
						break;
					case '\n':
						s = s.Substring(0, i) + "\\n" + s.Substring(i + 1);
						i++;
						break;
					case '\r':
						s = s.Substring(0, i) + "\\r" + s.Substring(i + 1);
						i++;
						break;
					case '\t':
						s = s.Substring(0, i) + "\\t" + s.Substring(i + 1);
						i++;
						break;
					default:
						if (!AvailableChars.Contains(s[i]))
						{
							var hex = Convert.ToInt16(s[i]).ToString("X4");
							s = s.Substring(0, i) + "\\u" + hex + s.Substring(i + 1);
							i += 5;
						}
						break;
				}
				i++;
			}
			return s;
		}
	}
}