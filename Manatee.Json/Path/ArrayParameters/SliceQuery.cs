using System;
using Manatee.Json.Internal;
using System.Collections.Generic;
using System.Linq;

namespace Manatee.Json.Path.ArrayParameters
{
	internal class SliceQuery : IJsonPathArrayQuery, IEquatable<SliceQuery>
	{
		internal IEnumerable<Slice> Slices { get; }

		public SliceQuery(params Slice[] slices)
		{
			Slices = slices.ToList();
		}
		public IEnumerable<JsonValue> Find(JsonArray json, JsonValue root)
		{
			var results = new HashSet<JsonValue>();

			foreach (var slice in Slices)
			{
				results.UnionWith(slice.Find(json, root));
			}

			return results;
		}
		public override string ToString()
		{
			return string.Join(",", Slices);
		}
		public bool Equals(SliceQuery other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Slices.ContentsEqual(other.Slices);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as SliceQuery);
		}
		public override int GetHashCode()
		{
			return Slices?.GetCollectionHashCode() ?? 0;
		}
	}
}