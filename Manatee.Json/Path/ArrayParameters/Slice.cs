using System;
using System.Collections.Generic;
using System.Linq;

namespace Manatee.Json.Path.ArrayParameters
{
	public class Slice
	{
		private int? _index;
		private int? _start;
		private int? _end;
		private int? _step;

		public Slice(int index)
		{
			_index = index;
		}
		public Slice(int? start, int? end, int? step = null)
		{
			_start = start;
			_end = end;
			_step = step;
		}

		public IEnumerable<JsonValue> Find(JsonArray json, JsonValue root)
		{
			if (_index.HasValue)
			{
				return json.Count > _index.Value
					       ? Enumerable.Empty<JsonValue>()
					       : new[] {json[_index.Value]};
			}

			var start = ResolveIndex(_start ?? 0, json.Count);
			var end = ResolveIndex(_end ?? json.Count, json.Count);
			var step = Math.Max(_step ?? 1, 1);

			var index = start;
			var list = new List<JsonValue>();
			while (index < end)
			{
				list.Add(json[index]);
				index += step;
			}
			return list;
		}
		public override string ToString()
		{
			return _index.HasValue
				       ? _index.ToString()
				       : _step.HasValue
					       ? $"{(_start.HasValue ? _start.ToString() : string.Empty)}:{(_end.HasValue ? _end.ToString() : string.Empty)}:{_step}"
					       : $"{(_start.HasValue ? _start.ToString() : string.Empty)}:{(_end.HasValue ? _end.ToString() : string.Empty)}";
		}

		public static implicit operator Slice(int i)
		{
			return new Slice(i);
		}

		private static int ResolveIndex(int index, int count)
		{
			return index < 0 ? count + index : index;
		}
	}
}