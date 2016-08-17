/***************************************************************************************

	Copyright 2016 Greg Dennis

	   Licensed under the Apache License, Version 2.0 (the "License");
	   you may not use this file except in compliance with the License.
	   You may obtain a copy of the License at

		 http://www.apache.org/licenses/LICENSE-2.0

	   Unless required by applicable law or agreed to in writing, software
	   distributed under the License is distributed on an "AS IS" BASIS,
	   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	   See the License for the specific language governing permissions and
	   limitations under the License.
 
	File Name:		SerializerCache.cs
	Namespace:		Manatee.Json.Serialization.Internal
	Class Name:		SerializerCache
	Purpose:		Maintains a cache of JsonSerializers methods organized by type.

***************************************************************************************/

using System;
#if NET4 || NET4C || NET45
using System.Collections.Concurrent;
#else
using System.Collections.Generic;
#endif
using System.Reflection;

namespace Manatee.Json.Serialization.Internal
{
	internal static class SerializerCache
	{
#if NET4 || NET4C || NET45
		private static readonly ConcurrentDictionary<Type, SerializerMethodPair> _cache;
#else
		private static readonly Dictionary<Type, SerializerMethodPair> _cache;
#endif
		static SerializerCache()
		{
#if NET4 || NET4C || NET45
			_cache = new ConcurrentDictionary<Type, SerializerMethodPair>();
#else
			_cache = new Dictionary<Type, SerializerMethodPair>();
#endif
		}

		public static MethodInfo GetSerializeMethod(Type type)
		{
			if (!_cache.ContainsKey(type))
#if NET4 || NET4C || NET45
				_cache.TryAdd(type, new SerializerMethodPair(type));
#else
				_cache.Add(type, new SerializerMethodPair(type));
#endif
			return _cache[type].Serializer;
		}

		public static MethodInfo GetDeserializeMethod(Type type)
		{
			if (!_cache.ContainsKey(type))
#if NET4 || NET4C || NET45
				_cache.TryAdd(type, new SerializerMethodPair(type));
#else
				_cache.Add(type, new SerializerMethodPair(type));
#endif
			return _cache[type].Deserializer;
		}
	}
}
