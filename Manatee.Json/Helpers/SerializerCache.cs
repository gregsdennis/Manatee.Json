/***************************************************************************************

	Copyright 2012 Greg Dennis

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
	Namespace:		Manatee.Json.Cache
	Class Name:		SerializerCache
	Purpose:		Maintains a cache of JsonSerializers methods organized by type.

***************************************************************************************/
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Manatee.Json.Helpers
{
	internal class SerializerCache : ISerializerCache
	{
		private readonly Dictionary<Type, SerializerMethodPair> _cache;

		public static ISerializerCache Default { get; private set; }

		static SerializerCache()
		{
			Default = new SerializerCache();
		}
		public SerializerCache()
		{
			_cache = new Dictionary<Type, SerializerMethodPair>();
		}

		public MethodInfo GetSerializeMethod(Type type)
		{
			if (!_cache.ContainsKey(type))
				_cache.Add(type, new SerializerMethodPair(type));
			return _cache[type].Serializer;
		}

		public MethodInfo GetDeserializeMethod(Type type)
		{
			if (!_cache.ContainsKey(type))
				_cache.Add(type, new SerializerMethodPair(type));
			return _cache[type].Deserializer;
		}
	}
}
