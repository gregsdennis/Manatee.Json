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

namespace Manatee.Json.Internal
{
	internal static class GeneralExtensions
	{
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
								hex = (hex2 - 0xDC00) + ((hex - 0xD800) << 10);
								length += 6;
							}
							source = source.Substring(0, i) + char.ConvertFromUtf32(hex) + source.Substring(i + length);
							break;
						default:
							result = source;
							return string.Format("Invalid escape sequence: '\\{0}'.", source[i + 1]);
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
					source = Replace(source, index, count, replace);
					index++;
				}
				index++;
			}
			return source;
		}
		private static string Replace(string source, int index, int count, string content)
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
		// Note: These methods assume that if a generic type is passed, the type is open.
		public static bool InheritsFrom(this Type tDerived, Type tBase)
		{
			if (tDerived.IsSubtypeOf(tBase)) return true;
			var interfaces = tDerived.GetInterfaces().Select(i => i.IsGenericType ? i.GetGenericTypeDefinition() : i);
			return interfaces.Contains(tBase);
		}
		private static bool IsSubtypeOf(this Type tDerived, Type tBase)
		{
			var currentType = tDerived.BaseType;
			while (currentType != null)
			{
				if (currentType.IsGenericType)
					currentType = currentType.GetGenericTypeDefinition();
				if (currentType == tBase) return true;
				currentType = currentType.BaseType;
			}
			return false;
		}
	}
}