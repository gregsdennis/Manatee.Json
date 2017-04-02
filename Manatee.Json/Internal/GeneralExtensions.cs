using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Manatee.Json.Internal;

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
								hex = (hex - 0xD800)*0x400 + (hex2 - 0xDC00)%0x400 + 0x10000;
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
		// Note: These methods assume that if a generic type is passed, the type is open.
		public static bool InheritsFrom(this Type tDerived, Type tBase)
		{
			if (tDerived.IsSubtypeOf(tBase)) return true;
			var interfaces = tDerived.TypeInfo().GetInterfaces().Select(i => i.TypeInfo().IsGenericType ? i.GetGenericTypeDefinition() : i);
			return interfaces.Contains(tBase);
		}
		private static bool IsSubtypeOf(this Type tDerived, Type tBase)
		{
			var currentType = tDerived.TypeInfo().BaseType;
			while (currentType != null)
			{
				if (currentType.TypeInfo().IsGenericType)
					currentType = currentType.GetGenericTypeDefinition();
				if (currentType == tBase) return true;
				currentType = currentType.TypeInfo().BaseType;
			}
			return false;
		}
		public static int GetCollectionHashCode<T>(this IEnumerable<T> collection)
		{
			return collection.Aggregate(0, (current, obj) => unchecked(current * 397) ^ obj.GetHashCode());
		}
		public static int GetCollectionHashCode<T>(this IEnumerable<KeyValuePair<string, T>> collection)
		{
#if IOS || CORE
			return collection.OrderBy(kvp => kvp.Key, StringComparer.Ordinal)
#else
			return collection.OrderBy(kvp => kvp.Key, StringComparer.InvariantCulture)
#endif
							 .Aggregate(0, (current, kvp) =>
				                            {
					                            unchecked
					                            {
						                            var code = (current*397) ^ kvp.Key.GetHashCode();
						                            code = (code*397) ^ (kvp.Value?.GetHashCode() ?? 0);
						                            return code;
					                            }
				                            });
		}
		public static bool ContentsEqual<T>(this IEnumerable<T> a, IEnumerable<T> b)
		{
			if (a == null && b != null) return false;
			if (a != null && b == null) return false;
			if (a == null) return true;

			var listA = a.ToList();
			var listB = b.ToList();
			return listA.Count == listB.Count && listA.All(item => listB.Contains(item));
		}
		public static JsonValue AsJsonValue(this object value)
		{
			if (value is JsonValue) return (JsonValue) value;
			if (value is JsonArray) return (JsonArray) value;
			if (value is JsonObject) return (JsonObject) value;
			if (value is string) return (string) value;
			if (value is bool) return (bool) value;
			if (value.IsNumber()) return Convert.ToDouble(value);

			return null;
		}
		public static bool IsNumber(this object value)
		{
			return value is double ||
			       value is float ||
			       value is int ||
			       value is uint ||
			       value is short ||
			       value is ushort ||
				   value is byte ||
				   value is long ||
				   value is ulong;
		}
	}
}