using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Manatee.Json.Internal;
using Manatee.Json.Pointer;

namespace Manatee.Json.Serialization.Internal
{
	internal class SerializationReferenceCache
	{
		private readonly IDictionary<object, SerializationReference> _objMap = new Dictionary<object, SerializationReference>();
		private readonly IDictionary<string, SerializationReference> _refMap = new Dictionary<string, SerializationReference>();

		public int Count => _objMap.Count;

		public void Add(SerializationReference value)
		{
			if (value.Object != null)
				_objMap.Add(value.Object, value);
			if (value.Source != null)
				_refMap.Add(value.Source.ToString(), value);
		}

		public void AddReference(JsonPointer source, JsonPointer target)
		{
			var sourceAsString = source.ToString();
			if (!_refMap.TryGetValue(sourceAsString, out var map))
			{
				map = new SerializationReference
					{
						Source = source
					};
				_refMap[sourceAsString] = map;
			}

			map.Targets.Add(target);
		}

		public void Clear()
		{
			_objMap.Clear();
			_refMap.Clear();
		}

		public void Complete(object root)
		{
			foreach (var map in _refMap.Values.Where(r => r.Targets.Count > 0))
			{
				if (!map.DeserializationIsComplete) continue;

				foreach (var target in map.Targets)
				{
					root.SetMember(target, map.Object);
				}

			}

			Clear();
		}

		public bool TryGetPair(object obj, [NotNullWhen(true)] out SerializationReference? pair)
		{
			return _objMap.TryGetValue(obj, out pair);
		}
	}
}