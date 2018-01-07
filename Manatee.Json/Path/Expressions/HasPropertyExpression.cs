using System;

namespace Manatee.Json.Path.Expressions
{
	internal class HasPropertyExpression<T> : PathExpression<T>, IEquatable<HasPropertyExpression<T>>
	{
		public string Name { get; set; }

		public override object Evaluate(T json, JsonValue root)
		{
			var value = json as JsonValue;
			if (value == null)
				throw new NotSupportedException("HasProperty requires an array to evaluate.");
			var result = value.Type == JsonValueType.Object && value.Object.ContainsKey(Name);
			if (result && value.Object[Name].Type == JsonValueType.Boolean)
				result = value.Object[Name].Boolean;
			return result;
		}
		public override string ToString()
		{
			return $"@.{Name}";
		}
		public bool Equals(HasPropertyExpression<T> other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return base.Equals(other) && string.Equals(Name, other.Name);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as HasPropertyExpression<T>);
		}
		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = base.GetHashCode();
				hashCode = (hashCode * 397) ^ (Name?.GetHashCode() ?? 0);
				return hashCode;
			}
		}
	}
}