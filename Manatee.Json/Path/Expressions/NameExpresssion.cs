using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Manatee.Json.Internal;

namespace Manatee.Json.Path.Expressions
{
	internal class NameExpression<T> : PathExpression<T>, IEquatable<NameExpression<T>>
	{
		public string Name { get; }
		public ExpressionTreeNode<T> NameExp { get; }

		public NameExpression(JsonPath path, bool isLocal, string name)
			: base(path, isLocal)
		{
			Name = name;
			NameExp = null!;
		}

		public NameExpression(JsonPath path, bool isLocal, ExpressionTreeNode<T> nameExp)
			: base(path, isLocal)
		{
			Name = null!;
			NameExp = nameExp;
		}

		public override object? Evaluate([MaybeNull] T json, JsonValue? root)
		{
			var value = IsLocal ? json?.AsJsonValue() : root;
			if (value == null)
				throw new NotSupportedException("Name requires a JsonValue to evaluate.");
			var results = Path.Evaluate(value);
			if (results.Count > 1)
				throw new InvalidOperationException($"Path '{Path}' returned more than one result on value '{value}'");
			var result = results.FirstOrDefault();
			var name = _GetName();
			return result != null && result.Type == JsonValueType.Object && result.Object.ContainsKey(name)
				       ? result.Object[name].GetValue()
				       : null;
		}
		public override string? ToString()
		{
			var path = Path == null ? string.Empty : Path.GetRawString();
			return string.Format(IsLocal ? "@{0}.{1}" : "${0}.{1}", path, _GetName());
		}

		private string _GetName()
		{
			var value = NameExp?.Evaluate(default!, null);
			if (value != null)
				return (string)value;
			return Name;
		}
		public bool Equals(NameExpression<T>? other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return base.Equals(other) && string.Equals(Name, other.Name) && Equals(NameExp, other.NameExp);
		}
		public override bool Equals(object? obj)
		{
			return Equals(obj as NameExpression<T>);
		}
		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = base.GetHashCode();
				hashCode = (hashCode*397) ^ (Name?.GetHashCode() ?? 0);
				hashCode = (hashCode*397) ^ (NameExp?.GetHashCode() ?? 0);
				return hashCode;
			}
		}
	}
}