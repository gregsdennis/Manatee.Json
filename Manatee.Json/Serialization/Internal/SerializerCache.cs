using System;
using System.Collections.Concurrent;

namespace Manatee.Json.Serialization.Internal
{
	internal static class SerializerCache
	{
		private static readonly ConcurrentDictionary<Type, SerializerMethodPair> _cache;

		static SerializerCache()
		{
			_cache = new ConcurrentDictionary<Type, SerializerMethodPair>();
		}

		public static Func<JsonSerializer, object, object> GetSerializeMethod(Type type)
		{
			var pair = _EnsureMethodPair(type);
			return pair.Serializer;
		}

		public static Func<JsonSerializer, object, object> GetDeserializeMethod(Type type)
		{
			var pair = _EnsureMethodPair(type);
			return pair.Deserializer;
		}

		private static SerializerMethodPair _EnsureMethodPair(Type type)
		{
			if (!_cache.TryGetValue(type, out var pair))
			{
				pair = new SerializerMethodPair(type);
				_cache[type] = pair;
			}

			return pair;
		}
	}
}
