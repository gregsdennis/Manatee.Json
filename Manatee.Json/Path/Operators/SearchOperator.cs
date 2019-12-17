using System;

namespace Manatee.Json.Path.Operators
{
	internal class SearchOperator : IJsonPathOperator, IEquatable<SearchOperator>
	{
		private readonly IJsonPathSearchParameter _parameter;

		public SearchOperator(IJsonPathSearchParameter parameter)
		{
			_parameter = parameter;
		}

		public JsonArray Evaluate(JsonArray json, JsonValue root)
		{
			return new JsonArray(_parameter.Find(json, root));
		}

		public override string? ToString()
		{
			return $"..{_parameter}";
		}

		public bool Equals(SearchOperator? other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(_parameter, other._parameter);
		}

		public override bool Equals(object? obj)
		{
			return Equals(obj as SearchOperator);
		}

		public override int GetHashCode()
		{
			return _parameter?.GetHashCode() ?? 0;
		}
	}
}