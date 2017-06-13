using System;
using System.Linq;
using Manatee.Json.Path.Expressions;

namespace Manatee.Json.Path.Operators
{
	internal class IndexOfOperator : IJsonPathOperator, IEquatable<IndexOfOperator>
	{
		public Expression<JsonValue, JsonArray> Parameter { get; }

		public IndexOfOperator(Expression<JsonValue, JsonArray> parameter)
		{
			Parameter = parameter;
		}

		public JsonArray Evaluate(JsonArray json, JsonValue root)
		{
			var parameter = Parameter.Evaluate(json, root);
			return json.Where(v => v.Type == JsonValueType.Array)
			           .Select(v => (JsonValue) v.Array.IndexOf(parameter))
			           .ToJson();
		}
		public override string ToString()
		{
			return $".indexOf({Parameter})";
		}
		public bool Equals(IndexOfOperator other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(Parameter, other.Parameter);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as IndexOfOperator);
		}
		public override int GetHashCode()
		{
			return Parameter?.GetHashCode() ?? 0;
		}
	}
}