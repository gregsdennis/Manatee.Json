using System;
#if !NET35 && !IOS
using Dictionary = System.Collections.Concurrent.ConcurrentDictionary<System.Type, Manatee.Json.Serialization.Internal.SerializerMethodPair>;
#else
using Dictionary = System.Collections.Generic.Dictionary<System.Type, Manatee.Json.Serialization.Internal.SerializerMethodPair>;
#endif
using System.Reflection;

namespace Manatee.Json.Serialization.Internal
{
	internal static class SerializerCache
	{
		private static readonly Dictionary _cache;

		static SerializerCache()
		{
			_cache = new Dictionary();
		}

		public static MethodInfo GetSerializeMethod(Type type)
		{
			var pair = EnsureMethodPair(type);
			return pair.Serializer;
		}

		public static MethodInfo GetDeserializeMethod(Type type)
		{
			var pair = EnsureMethodPair(type);
			return pair.Deserializer;
		}

		private static SerializerMethodPair EnsureMethodPair(Type type)
		{
			SerializerMethodPair pair;
			if (!_cache.TryGetValue(type, out pair))
			{
				pair = new SerializerMethodPair(type);
				_cache[type] = pair;
			}

			return pair;
		}
	}
}
