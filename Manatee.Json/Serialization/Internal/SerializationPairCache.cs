using System;
using System.Collections.Generic;

namespace Manatee.Json.Serialization.Internal
{
	internal class SerializationPairCache
	{
		private readonly IDictionary<object, SerializationPair> _objMap = new Dictionary<object, SerializationPair>();
		private readonly IDictionary<Guid, SerializationPair> _guidMap = new Dictionary<Guid, SerializationPair>();
		private readonly IDictionary<SerializationPair, Guid> _reverse = new Dictionary<SerializationPair, Guid>();

		public SerializationPair this[Guid key] => _guidMap[key];

		public int Count => _guidMap.Count;

		public void Add(Guid key, SerializationPair value)
		{
			if (value.Object != null)
				_objMap.Add(value.Object, value);
			_guidMap.Add(key, value);
			_reverse.Add(value, key);
		}

		public void Clear()
		{
			_objMap.Clear();
			_guidMap.Clear();
			_reverse.Clear();
		}

		public bool TryGetPair(object obj, out Guid key, out SerializationPair pair)
		{
			if (_objMap.TryGetValue(obj, out pair))
			{
				key = _reverse[pair];
				return true;
			}

			key = default(Guid);
			return false;
		}

		public void Update(SerializationPair pair, object obj)
		{
			pair.Object = obj;
			_objMap.Add(obj, pair);
		}
	}
}