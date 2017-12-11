using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Manatee.Json.Serialization.Internal
{
	internal class SerializationPairCache
	{
		private IDictionary<object, SerializationPair> _objMap = new Dictionary<object, SerializationPair>();
		private IDictionary<Guid, SerializationPair> _guidMap = new Dictionary<Guid, SerializationPair>();
		private IDictionary<SerializationPair, Guid> _reverse = new Dictionary<SerializationPair, Guid>();

		public SerializationPair this[Guid key]
		{
			get
			{
				return _guidMap[key];
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public int Count => _guidMap.Count;

		public void Add(Guid key, SerializationPair value)
		{
			Debug.Assert(value.Object != null);
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
			pair = null;
			return false;
		}
	}
}