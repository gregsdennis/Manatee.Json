using System.Collections.Generic;

namespace Manatee.Json.Serialization.Internal
{
	internal class SerializationPairCache
	{
		private readonly IDictionary<object, SerializationReference> _objMap = new Dictionary<object, SerializationReference>();
		private readonly IDictionary<string, SerializationReference> _refMap = new Dictionary<string, SerializationReference>();

		public int Count => _objMap.Count;

		public void Add(SerializationReference value)
		{
			if (value.Object != null)
				_objMap.Add(value.Object, value);
			if (value.Reference != null)
				_refMap.Add(value.Reference.ToString(), value);
		}

		public void Clear()
		{
			_objMap.Clear();
		}

		public bool TryGetPair(object obj, out SerializationReference pair)
		{
			return _objMap.TryGetValue(obj, out pair);
		}

		public bool TryGetPair(string location, out SerializationReference pair)
		{
			return _refMap.TryGetValue(location, out pair);
		}
	}
}