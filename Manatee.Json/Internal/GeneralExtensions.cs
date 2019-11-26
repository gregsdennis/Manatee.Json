using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Manatee.Json.Internal
{
	internal static class GeneralExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool In<T>(this T value, params T[] collection)
		{
			return collection.Contains(value);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsInt(this double value)
		{
			return Math.Ceiling(value) == Math.Floor(value);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int GetCollectionHashCode<T>(this IEnumerable<T> collection)
		{
			return collection.Aggregate(0, (current, obj) => unchecked(current * 397) ^ obj.GetHashCode());
		}
		public static int GetCollectionHashCode<T>(this IEnumerable<KeyValuePair<string, T>> collection)
		{
			return collection.OrderBy(kvp => kvp.Key, StringComparer.Ordinal)
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
		public static JsonValue? AsJsonValue(this object value)
		{
			if (ReferenceEquals(value, null)) return null;
			if (value is JsonValue jsonValue) return jsonValue;
			if (value is JsonArray jsonArray) return jsonArray;
			if (value is JsonObject jsonObject) return jsonObject;
			if (value is string s) return s;
			if (value is bool) return (bool) value;
			if (value.IsNumber()) return Convert.ToDouble(value);

			return null;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
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