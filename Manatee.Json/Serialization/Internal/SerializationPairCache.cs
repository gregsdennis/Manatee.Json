using System;
using System.Collections.Generic;
using System.Linq;

namespace Manatee.Json.Serialization.Internal
{
	internal class SerializationPairCache : Dictionary<Guid, SerializationPair>
	{
		public bool Contains(object obj)
		{
			return Values.Any(v => ReferenceEquals(v.Object, obj));
		}
		public Guid GetKey(object obj)
		{
			return this.First(v => ReferenceEquals(v.Value.Object, obj)).Key;
		}
	}
}