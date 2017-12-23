using System;

namespace Manatee.Json.Path.Expressions
{
	internal class ValueExpression<T> : ExpressionTreeNode<T>, IEquatable<ValueExpression<T>>
	{
		public object Value { get; set; }

		public override object Evaluate(T json, JsonValue root)
		{
			return Value;
		}
		public override string ToString()
		{
			return Value is string
				       ? $"\"{Value}\""
				       : Value is bool
					       ? Value.ToString().ToLower()
					       : Value.ToString();
		}
		public bool Equals(ValueExpression<T> other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(Value, other.Value);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as ValueExpression<T>);
		}
		public override int GetHashCode()
		{
			return Value?.GetHashCode() ?? 0;
		}
	}
}