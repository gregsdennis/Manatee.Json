using System;

namespace Manatee.Json.Path.Expressions
{
	internal abstract class ComparisonExpression<T> : ExpressionTreeBranch<T>
	{
		protected static double? GetDouble(object value)
		{
			if (value == null) return null;
			var json = value as JsonValue;
			if (json == null) return Convert.ToDouble(value);
			return json.Type == JsonValueType.Number ? json.Number : (double?) null;
		}
	}
}