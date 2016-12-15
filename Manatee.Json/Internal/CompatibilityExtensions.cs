using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Manatee.Json.Internal
{
	internal static class CompatibilityExtensions
	{
		//[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsNullOrWhiteSpace(this string value)
		{
#if NET35
			return string.IsNullOrEmpty(value) || string.IsNullOrEmpty(value.Trim());
#else
			return string.IsNullOrWhiteSpace(value);
#endif
		}
		//[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string Join<T>(this IEnumerable<T> segments, string separator)
		{
#if NET35
			return string.Join(separator, segments.Select(s => s.ToString()).ToArray());
#else
			return string.Join(separator, segments);
#endif
		}
#if IOS || CORE
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
#else
		//[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Type TypeInfo(this Type type)
		{
			return type;
		}
		//[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Type[] GetTypeArguments(this Type type)
		{
			return type.GetGenericArguments();
		}
#endif
#if IOS
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Any(this string value, Func<char, bool> predicate)
		{
			return value.ToCharArray().Any(predicate);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<char> TakeWhile(this string value, Func<char, bool> predicate)
		{
			return value.ToCharArray().TakeWhile(predicate);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Contains(this string value, char c)
		{
			return value.ToCharArray().Contains(c);
		}
		public static IEnumerable<Type> GetInterfaces(this TypeInfo type)
		{
			return type.ImplementedInterfaces;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsAssignableFrom(this Type derivedType, Type baseType)
		{
			return derivedType.TypeInfo().IsAssignableFrom(baseType.TypeInfo());
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MethodInfo GetMethod(this Type type, string name)
		{
			return type.TypeInfo().GetDeclaredMethod(name);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MemberInfo[] GetMember(this TypeInfo typeInfo, string name)
		{
			return typeInfo.DeclaredMembers.Where(m => m.Name == name).ToArray();
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MethodInfo GetSetMethod(this PropertyInfo property)
		{
			return property.SetMethod;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MethodInfo GetGetMethod(this PropertyInfo property)
		{
			return property.GetMethod;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<Type> GetTypes(this Assembly assembly)
		{
			return assembly.ExportedTypes;
		}
#endif
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
