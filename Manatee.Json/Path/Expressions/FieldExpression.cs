using System;
using System.Reflection;

namespace Manatee.Json.Path.Expressions
{
	internal class FieldExpression<T> : ExpressionTreeNode<T>, IEquatable<FieldExpression<T>>
	{
		public FieldInfo Field { get; set; }
		public object Source { get; set; }

		protected override int BasePriority => 6;

		public override object Evaluate(T json, JsonValue root)
		{
			if (Field.FieldType == typeof(string) ||
				Field.FieldType == typeof(JsonArray) ||
				Field.FieldType == typeof(JsonObject) ||
				Field.FieldType == typeof(JsonValue))
				return Field.GetValue(Source);
			return Convert.ToDouble(Field.GetValue(Source));
		}
		public override string ToString()
		{
			var value = Evaluate(default(T), null);
			return value is string
				       ? $"\"{value}\""
				       : value?.ToString() ?? "null";
		}
		public bool Equals(FieldExpression<T> other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(Field, other.Field) && Equals(Source, other.Source);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as FieldExpression<T>);
		}
		public override int GetHashCode()
		{
			unchecked { return ((Field != null ? Field.GetHashCode() : 0)*397) ^ (Source?.GetHashCode() ?? 0); }
		}
	}
}