using System;
using System.Linq;

namespace Manatee.Json.Path.Operators
{
	internal class NameOperator : IJsonPathOperator, IEquatable<NameOperator>
	{
		public string Name { get; }

		public NameOperator(string name)
		{
			Name = name;
		}

		public JsonArray Evaluate(JsonArray json, JsonValue root)
		{
			var results = new JsonArray();
			foreach (var value in json)
			{
				if (value.Type == JsonValueType.Object && value.Object.TryGetValue(Name, out var match))
					results.Add(match);
			}
			return results;
		}

		public override string ToString()
		{
			return Name.Any(c => !char.IsLetterOrDigit(c)) || string.IsNullOrWhiteSpace(Name)
				       ? $".'{Name}'"
				       : $".{Name}";
		}

		public bool Equals(NameOperator other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return string.Equals(Name, other.Name);
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as NameOperator);
		}

		public override int GetHashCode()
		{
			return Name?.GetHashCode() ?? 0;
		}
	}
}