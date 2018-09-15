using System.Collections.Generic;

namespace Manatee.Json.Serialization.Internal
{
	internal class SerializationPairCache
	{
		private readonly IDictionary<object, SerializationReference> _objMap = new Dictionary<object, SerializationReference>();

		public int Count => _objMap.Count;

		public void Add(SerializationReference value)
		{
			if (value.Object != null)
				_objMap.Add(value.Object, value);
		}

		public void Clear()
		{
			_objMap.Clear();
		}

		public bool TryGetPair(object obj, out SerializationReference pair)
		{
			return _objMap.TryGetValue(obj, out pair);
		}

		public void Update(SerializationReference pair, object obj)
		{
			pair.Object = obj;
			_objMap.Add(obj, pair);
		}
	}
}