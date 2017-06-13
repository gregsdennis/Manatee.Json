using System;
using System.Collections.Generic;
using System.Reflection;

namespace Manatee.Json.Serialization.Internal
{
	internal static class SerializerCache
	{
		private static readonly Dictionary<Type, SerializerMethodPair> _cache;

		static SerializerCache()
		{
			_cache = new Dictionary<Type, SerializerMethodPair>();
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
