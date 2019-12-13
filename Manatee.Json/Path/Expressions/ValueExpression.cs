using System;
using System.Diagnostics.CodeAnalysis;

namespace Manatee.Json.Path.Expressions
{
	internal class ValueExpression<T> : ExpressionTreeNode<T>, IEquatable<ValueExpression<T>>
	{
		public object Value { get; }

		public ValueExpression(object value)
		{
			Value = value;
		}

		public override object? Evaluate([MaybeNull] T json, JsonValue? root)
		{
			return Value;
		}
		public override string? ToString()
		{
			return Value switch
				{
					string s => $"\"{s}\"",
					bool b => b.ToString().ToLower(),
					_ => Value.ToString()
				};
		}
		public bool Equals(ValueExpression<T>? other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(Value, other.Value);
		}
		public override bool Equals(object? obj)
		{
			return Equals(obj as ValueExpression<T>);
		}
		public override int GetHashCode()
		{
			return Value?.GetHashCode() ?? 0;
		}
	}
}