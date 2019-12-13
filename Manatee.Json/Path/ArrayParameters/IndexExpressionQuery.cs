using System;
using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Path.Expressions;

namespace Manatee.Json.Path.ArrayParameters
{
	internal class IndexExpressionQuery : IJsonPathArrayQuery, IEquatable<IndexExpressionQuery>
	{
		private readonly Expression<int, JsonArray> _expression;

		public IndexExpressionQuery(Expression<int, JsonArray> expression)
		{
			_expression = expression;
		}

		public IEnumerable<JsonValue> Find(JsonArray json, JsonValue root)
		{
			var index = _expression.Evaluate(json, root);
			return index >= 0 && index < json.Count ? new[] {json[index]} : Enumerable.Empty<JsonValue>();
		}
		public override string ToString()
		{
			return $"({_expression})";
		}
		public bool Equals(IndexExpressionQuery? other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(_expression, other._expression);
		}
		public override bool Equals(object? obj)
		{
			return Equals(obj as IndexExpressionQuery);
		}
		public override int GetHashCode()
		{
			return _expression?.GetHashCode() ?? 0;
		}
	}
}