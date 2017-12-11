using System;
using System.Collections.Generic;

namespace Manatee.Json.Serialization.Internal
{
	internal class SerializationPairCache
	{
		private IDictionary<Guid, SerializationPair> map = new Dictionary<Guid, SerializationPair>();
		private IDictionary<SerializationPair, Guid> reverse = new Dictionary<SerializationPair, Guid>();

		public SerializationPair this[Guid key]
		{
			get
			{
				return map[key];
			}
			set
			{
				map[key] = value;
				reverse[value] = key;
			}
		}

		public int Count => map.Count;

		public void Add(Guid key, SerializationPair value)
		{
			map.Add(key, value);
			reverse.Add(value, key);
		}

		public void Clear()
		{
			map.Clear();
			reverse.Clear();
		}

		public bool TryGetPair(object obj, out Guid key, out SerializationPair pair)
		{
			foreach (var value in map.Values)
			{
				if (ReferenceEquals(value.Object, obj))
				{
					key = reverse[value];
					pair = value;
					return true;
				}
			}

			key = default(Guid);
			pair = null;
			return false;
		}
	}
}