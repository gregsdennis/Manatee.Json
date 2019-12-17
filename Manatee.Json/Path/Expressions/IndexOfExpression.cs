using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Manatee.Json.Internal;

namespace Manatee.Json.Path.Expressions
{
	internal class IndexOfExpression<T> : PathExpression<T>, IEquatable<IndexOfExpression<T>>
	{
		public JsonValue Parameter { get; }
		public ExpressionTreeNode<JsonArray> ParameterExpression { get; }

		public IndexOfExpression(JsonPath path, bool isLocal, JsonValue parameter)
			: base(path, isLocal)
		{
			Parameter = parameter;
			ParameterExpression = null!;
		}

		public IndexOfExpression(JsonPath path, bool isLocal, ExpressionTreeNode<JsonArray> parameterExpression)
			: base(path, isLocal)
		{
			Parameter = null!;
			ParameterExpression = parameterExpression;
		}

		public override object? Evaluate([MaybeNull] T json, JsonValue? root)
		{
			var value = IsLocal ? json?.AsJsonValue() : root;
			if (value == null)
				throw new NotSupportedException("IndexOf requires a JsonValue to evaluate.");
			var results = Path.Evaluate(value);
			if (results.Count > 1)
				throw new InvalidOperationException($"Path '{Path}' returned more than one result on value '{value}'");
			var result = results.FirstOrDefault();
			var parameter = _GetParameter();

			if (result == null || result.Type != JsonValueType.Array || parameter == null) return null;
			return result.Array.IndexOf(parameter);
		}
		public override string? ToString()
		{
			var path = Path == null ? string.Empty : Path.GetRawString();
			var parameter = ParameterExpression?.ToString() ?? Parameter.ToString();
			return string.Format(IsLocal ? "@{0}.indexOf({1})" : "${0}.indexOf({1})", path, parameter);
		}
		public bool Equals(IndexOfExpression<T>? other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return base.Equals(other) && Equals(ParameterExpression, other.ParameterExpression);
		}
		public override bool Equals(object? obj)
		{
			return Equals(obj as IndexOfExpression<T>);
		}
		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = base.GetHashCode();
				hashCode = (hashCode*397) ^ (Parameter != null ? Parameter.GetHashCode() : 0);
				hashCode = (hashCode*397) ^ (ParameterExpression?.GetHashCode() ?? 0);
				return hashCode;
			}
		}

		private JsonValue? _GetParameter()
		{
			var value = ParameterExpression?.Evaluate(null!, null);
			return value switch
				{
					double d => new JsonValue(d),
					bool b => new JsonValue(b),
					string s => new JsonValue(s),
					JsonArray array => new JsonValue(array),
					JsonObject jsonObject => new JsonValue(jsonObject),
					JsonValue jsonValue => jsonValue,
					_ => Parameter
				};
		}
	}
}
