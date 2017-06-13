using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Manatee.Json.Internal
{
	[Obsolete]
	internal static class CompatibilityExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsNullOrWhiteSpace(this string value)
		{
			return string.IsNullOrWhiteSpace(value);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Join<T>(this IEnumerable<T> segments, string separator)
		{
			return string.Join(separator, segments);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TypeInfo TypeInfo(this Type type)
		{
			return type.GetTypeInfo();
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Type[] GetTypeArguments(this Type type)
		{
			return type.GetTypeInfo().GenericTypeArguments;
		}
	}
}
