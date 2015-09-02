/***************************************************************************************

	Copyright 2015 Greg Dennis

	   Licensed under the Apache License, Version 2.0 (the "License");
	   you may not use this file except in compliance with the License.
	   You may obtain a copy of the License at

		 http://www.apache.org/licenses/LICENSE-2.0

	   Unless required by applicable law or agreed to in writing, software
	   distributed under the License is distributed on an "AS IS" BASIS,
	   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	   See the License for the specific language governing permissions and
	   limitations under the License.
 
	File Name:		ReflectionCache.cs
	Namespace:		Manatee.Json.Serialization.Internal.Serializers
	Class Name:		ReflectionCache
	Purpose:		Maintains reflection results.

***************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal static class ReflectionCache
	{
		private class ReflectionInfo
		{
			public IEnumerable<SerializationInfo> ReadOnlyProperties { get; private set; }
			public IEnumerable<SerializationInfo> ReadWriteProperties { get; private set; }
			public IEnumerable<SerializationInfo> Fields { get; private set; }

			public ReflectionInfo(IEnumerable<SerializationInfo> readOnlyProperties, IEnumerable<SerializationInfo> readWriteProperties, IEnumerable<SerializationInfo> fields)
			{
				ReadOnlyProperties = readOnlyProperties;
				ReadWriteProperties = readWriteProperties;
				Fields = fields;
			}
		}

		private static readonly Dictionary<Type, ReflectionInfo> InstanceCache;
		private static readonly Dictionary<Type, ReflectionInfo> StaticCache;

		static ReflectionCache()
		{
			InstanceCache = new Dictionary<Type, ReflectionInfo>();
			StaticCache = new Dictionary<Type, ReflectionInfo>();
		}

		public static IEnumerable<SerializationInfo> GetMembers(Type type, PropertySelectionStrategy propertyTypes, bool includeFields)
		{
			var info = InitializeInstanceCache(type);
			var members = GetProperties(info, propertyTypes);
			if (includeFields)
				members = members.Concat(GetFields(info));
			return members;
		}
		public static IEnumerable<SerializationInfo> GetTypeMembers(Type type, PropertySelectionStrategy propertyTypes, bool includeFields)
		{
			var info = InitializeStaticCache(type);
			var members = GetProperties(info, propertyTypes);
			if (includeFields)
				members = members.Concat(GetFields(info));
			return members;
		}

		private static IEnumerable<SerializationInfo> GetProperties(ReflectionInfo info, PropertySelectionStrategy propertyTypes)
		{
			var properties = new List<SerializationInfo>();
			if ((propertyTypes & PropertySelectionStrategy.ReadWriteOnly) != 0)
				properties.AddRange(info.ReadWriteProperties);
			if ((propertyTypes & PropertySelectionStrategy.ReadOnly) != 0)
				properties.AddRange(info.ReadOnlyProperties);
#if NET35 || NET35C
			return properties;
#elif NET4 || NET4C || NET45
			return properties;
#endif
		}
		private static IEnumerable<SerializationInfo> GetFields(ReflectionInfo info)
		{
			var fields = new List<SerializationInfo>();
			fields.AddRange(info.Fields);
#if NET35 || NET35C
			return fields;
#elif NET4 || NET4C || NET45
			return fields;
#endif
		}
		private static ReflectionInfo InitializeInstanceCache(Type type)
		{
			ReflectionInfo info;
			if (!InstanceCache.TryGetValue(type, out info))
			{
				var read = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
							   .Where(p => p.GetSetMethod() == null)
							   .Where(p => p.GetGetMethod() != null)
							   .Where(p => !p.GetCustomAttributes(typeof(JsonIgnoreAttribute), true).Any())
							   .Select(BuildSerializationInfo);
				var readWrite = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
									.Where(p => p.GetSetMethod() != null)
									.Where(p => p.GetGetMethod() != null)
									.Where(p => !p.GetCustomAttributes(typeof(JsonIgnoreAttribute), true).Any())
									.Select(BuildSerializationInfo);
				var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public)
								 .Where(p => !p.IsInitOnly)
								 .Where(p => !p.GetCustomAttributes(typeof(JsonIgnoreAttribute), true).Any())
								 .Select(BuildSerializationInfo);
				InstanceCache[type] = info = new ReflectionInfo(read, readWrite, fields);
			}
			return info;
		}
		private static ReflectionInfo InitializeStaticCache(Type type)
		{
			ReflectionInfo info;
			if (!StaticCache.TryGetValue(type, out info))
			{
				var read = type.GetProperties(BindingFlags.Static | BindingFlags.Public)
							   .Where(p => p.GetSetMethod() == null)
							   .Where(p => p.GetGetMethod() != null)
							   .Where(p => !p.GetCustomAttributes(typeof(JsonIgnoreAttribute), true).Any())
							   .Select(BuildSerializationInfo);
				var readWrite = type.GetProperties(BindingFlags.Static | BindingFlags.Public)
									.Where(p => p.GetSetMethod() != null)
									.Where(p => p.GetGetMethod() != null)
									.Where(p => !p.GetCustomAttributes(typeof(JsonIgnoreAttribute), true).Any())
									.Select(BuildSerializationInfo);
				var fields = type.GetFields(BindingFlags.Static | BindingFlags.Public)
								 .Where(p => !p.IsInitOnly)
								 .Where(p => !p.GetCustomAttributes(typeof(JsonIgnoreAttribute), true).Any())
								 .Select(BuildSerializationInfo);
				StaticCache[type] = info = new ReflectionInfo(read, readWrite, fields);
			}
			return info;
		}
		private static SerializationInfo BuildSerializationInfo(MemberInfo info)
		{
			var mapper = (JsonMapToAttribute)info.GetCustomAttributes(typeof(JsonMapToAttribute), false).FirstOrDefault();
			var name = mapper == null ? info.Name : mapper.MapToKey;
			return new SerializationInfo(info, name);
		}
	}
}