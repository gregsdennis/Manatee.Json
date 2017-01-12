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
