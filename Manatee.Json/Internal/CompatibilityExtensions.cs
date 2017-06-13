using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Manatee.Json.Internal
{
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
#if CORE
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsAssignableFrom(this Type derivedType, Type baseType)
		{
			return derivedType.TypeInfo().IsAssignableFrom(baseType);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MethodInfo GetMethod(this Type type, string name, BindingFlags flags = BindingFlags.Default)
		{
			return type.TypeInfo().GetMethod(name, flags);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MethodInfo GetMethod(this Type type, string name, Type[] typeParams)
		{
			return type.TypeInfo().GetMethod(name, typeParams);
		}
#endif
	}
}
