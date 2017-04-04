using System;
using Manatee.Json.Internal;

namespace Manatee.Json.Path.Expressions
{
	internal class ConversionExpression<T> : ExpressionTreeNode<T>, IEquatable<ConversionExpression<T>>
	{
		public ExpressionTreeNode<T> Root { get; set; }
		public Type TargetType { get; set; }

		protected override int BasePriority => 6;

		public override object Evaluate(T json, JsonValue root)
		{
			var value = Root.Evaluate(json, root);
			var result = CastValue(value);
			return result;
		}
		public override string ToString()
		{
			return Root.ToString();
		}
		public bool Equals(ConversionExpression<T> other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(Root, other.Root) && TargetType == other.TargetType;
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as ConversionExpression<T>);
		}
		public override int GetHashCode()
		{
			unchecked { return ((Root?.GetHashCode() ?? 0)*397) ^ (TargetType?.GetHashCode() ?? 0); }
		}

		private object CastValue(object value)
		{
			if (TargetType != typeof (JsonValue)) return value;
			if (value is bool)
				return new JsonValue((bool)value);
			if (value is string)
				return new JsonValue((string)value);
			if (value.IsNumber())
				return new JsonValue(Convert.ToDouble(value));
			return value;
		}
	}
}