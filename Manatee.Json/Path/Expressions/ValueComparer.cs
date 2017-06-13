using System;
using Manatee.Json.Internal;

namespace Manatee.Json.Path.Expressions
{
	internal static class ValueComparer
	{
		public static bool Equal(object a, object b)
		{
			var sleft = TryGetString(a);
			var sright = TryGetString(b);
			if (sleft != null && sright != null)
				return string.Compare(sleft, sright, StringComparison.Ordinal) == 0;
			var bleft = TryGetBoolean(a);
			var bright = TryGetBoolean(b);
			if (bleft != null && bright != null)
				return bleft == bright;
			var dleft = TryGetNumber(a);
			var dright = TryGetNumber(b);
			if (dleft != null && dright != null)
				return dleft == dright;
			return false;
		}
		public static bool LessThan(object a, object b)
		{
			var sleft = TryGetString(a);
			var sright = TryGetString(b);
			if (sleft != null && sright != null)
				return string.Compare(sleft, sright, StringComparison.Ordinal) < 0;
			var dleft = TryGetNumber(a);
			var dright = TryGetNumber(b);
			if (dleft != null && dright != null)
				return dleft < dright;
			return false;
		}
		public static bool GreaterThan(object a, object b)
		{
			var sleft = TryGetString(a);
			var sright = TryGetString(b);
			if (sleft != null && sright != null)
				return string.Compare(sleft, sright, StringComparison.Ordinal) > 0;
			var dleft = TryGetNumber(a);
			var dright = TryGetNumber(b);
			if (dleft != null && dright != null)
				return dleft > dright;
			return false;
		}

		public static bool LessThanEqual(object a, object b)
		{
			var sleft = TryGetString(a);
			var sright = TryGetString(b);
			if (sleft != null && sright != null)
				return string.Compare(sleft, sright, StringComparison.Ordinal) <= 0;
			var dleft = TryGetNumber(a);
			var dright = TryGetNumber(b);
			if (dleft != null && dright != null)
				return dleft <= dright;
			return false;
		}
		public static bool GreaterThanEqual(object a, object b)
		{
			var sleft = TryGetString(a);
			var sright = TryGetString(b);
			if (sleft != null && sright != null)
				return string.Compare(sleft, sright, StringComparison.Ordinal) >= 0;
			var dleft = TryGetNumber(a);
			var dright = TryGetNumber(b);
			if (dleft != null && dright != null)
				return dleft >= dright;
			return false;
		}

		private static string TryGetString(object value)
		{
			var jv = value as JsonValue;
			return jv != null && jv.Type == JsonValueType.String ? jv.String : value as string;
		}
		private static double? TryGetNumber(object value)
		{
			var jv = value as JsonValue;
			if (jv != null && jv.Type == JsonValueType.Number) return jv.Number;
			if (value.IsNumber()) return Convert.ToDouble(value);
			// at this point, we have no idea what this is.
			return null;
		}
		private static bool? TryGetBoolean(object value)
		{
			var jv = value as JsonValue;
			return jv != null && jv.Type == JsonValueType.Boolean ? jv.Boolean : value as bool?;
		}
	}
}