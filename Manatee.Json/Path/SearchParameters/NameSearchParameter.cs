using System;
using System.Collections.Generic;
using System.Linq;

namespace Manatee.Json.Path.SearchParameters
{
	internal class NameSearchParameter : IJsonPathSearchParameter, IEquatable<NameSearchParameter>
	{
		private readonly string _name;

		public NameSearchParameter(string name)
		{
			_name = name;
		}

		public IEnumerable<JsonValue> Find(IEnumerable<JsonValue> json, JsonValue root)
		{
			var results = new JsonArray();

			foreach (var value in json)
			{
				_Find(value, results);
			}

			return results;
		}

		public override string? ToString()
		{
			return _name.Any(c => !char.IsLetterOrDigit(c)) || string.IsNullOrWhiteSpace(_name)
					   ? $"'{_name}'"
					   : _name;
		}

		public bool Equals(NameSearchParameter? other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return string.Equals(_name, other._name);
		}

		public override bool Equals(object? obj)
		{
			return Equals(obj as NameSearchParameter);
		}

		public override int GetHashCode()
		{
			return _name?.GetHashCode() ?? 0;
		}

		private void _Find(JsonValue value, JsonArray results)
		{
			switch (value.Type)
			{
				case JsonValueType.Object:
					if (value.Object.TryGetValue(_name, out var match))
						results.Add(match);
					foreach (var subValue in value.Object.Values)
					{
						_Find(subValue, results);
					}
					break;
				case JsonValueType.Array:
					foreach (var subValue in value.Array)
					{
						_Find(subValue, results);
					}
					break;
			}
		}
	}
}