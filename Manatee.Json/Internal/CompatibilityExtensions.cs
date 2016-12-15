using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Manatee.Json.Internal
{
	internal static class CompatibilityExtensions
	{
		public static bool IsNullOrWhiteSpace(this string value)
		{
#if NET35
			return string.IsNullOrEmpty(value) || string.IsNullOrEmpty(value.Trim());
#else
			return string.IsNullOrWhiteSpace(value);
#endif
		}
		public static string Join<T>(this IEnumerable<T> segments, string separator)
		{
#if NET35
			return string.Join(separator, segments.Select(s => s.ToString()).ToArray());
#else
			return string.Join(separator, segments);
#endif
		}
#if IOS || CORE
		public static TypeInfo TypeInfo(this Type type)
		{
			return type.GetTypeInfo();
		}
#else
		public static Type TypeInfo(this Type type)
		{
			return type;
		}
#endif
#if IOS
		public static bool Any(this string value, Func<char, bool> predicate)
		{
			return value.ToCharArray().Any(predicate);
		}
		public static IEnumerable<char> TakeWhile(this string value, Func<char, bool> predicate)
		{
			return value.ToCharArray().TakeWhile(predicate);
		}
		public static bool Contains(this string value, char c)
		{
			return value.ToCharArray().Contains(c);
		}
#endif
#if CORE
		public static bool IsAssignableFrom(this Type derivedType, Type baseType)
		{
			return derivedType.TypeInfo().IsAssignableFrom(baseType);
		}
		public static MethodInfo GetMethod(this Type type, string name, BindingFlags flags = BindingFlags.Default)
		{
			return type.TypeInfo().GetMethod(name, flags);
		}
		public static MethodInfo GetMethod(this Type type, string name, Type[] typeParams)
		{
			return type.TypeInfo().GetMethod(name, typeParams);
		}
#endif
	}
}
