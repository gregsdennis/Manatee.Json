using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Manatee.Json.Internal
{
	internal static class ReflectionExtensions
	{
		// Note: These methods assume that if a generic type is passed, the type is open.
		public static bool InheritsFrom(this Type tDerived, Type tBase)
		{
			if (tDerived._IsSubtypeOf(tBase)) return true;
			var interfaces = tDerived.GetTypeInfo().ImplementedInterfaces.Select(i => IntrospectionExtensions.GetTypeInfo(i).IsGenericType ? i.GetGenericTypeDefinition() : i);
			return interfaces.Contains(tBase);
		}
		private static bool _IsSubtypeOf(this Type tDerived, Type tBase)
		{
			var currentType = tDerived.GetTypeInfo().BaseType;
			while (currentType != null)
			{
				if (currentType.GetTypeInfo().IsGenericType)
					currentType = currentType.GetGenericTypeDefinition();
				if (currentType == tBase) return true;
				currentType = currentType.GetTypeInfo().BaseType;
			}
			return false;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Type[] GetTypeArguments(this Type type)
		{
			return type.GetTypeInfo().GenericTypeArguments;
		}
		public static IEnumerable<PropertyInfo> GetAllProperties(this TypeInfo type)
		{
			var properties = new List<PropertyInfo>();
			while (type != null)
			{
				properties.AddRange(type.DeclaredProperties);
				type = type.BaseType?.GetTypeInfo();
			}
			return properties;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsNumericType(this Type value)
		{
			return value == typeof(double) ||
			       value == typeof(float) ||
			       value == typeof(int) ||
			       value == typeof(uint) ||
			       value == typeof(short) ||
			       value == typeof(ushort) ||
			       value == typeof(byte) ||
			       value == typeof(long) ||
			       value == typeof(ulong);
		}
	}
}