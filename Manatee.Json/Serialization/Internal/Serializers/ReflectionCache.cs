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
 
	File Name:		ReflectionCache.cs
	Namespace:		Manatee.Json.Serialization.Internal.Serializers
	Class Name:		ReflectionCache
	Purpose:		Maintains reflection results.

***************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Manatee.Json.Internal;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal static class ReflectionCache
	{
		private class ReflectionInfo
		{
			public IEnumerable<SerializationInfo> ReadOnlyProperties { get; }
			public IEnumerable<SerializationInfo> ReadWriteProperties { get; }
			public IEnumerable<SerializationInfo> Fields { get; }

			public ReflectionInfo(IEnumerable<SerializationInfo> readOnlyProperties, IEnumerable<SerializationInfo> readWriteProperties, IEnumerable<SerializationInfo> fields)
			{
				ReadOnlyProperties = readOnlyProperties;
				ReadWriteProperties = readWriteProperties;
				Fields = fields;
			}
		}

		private static readonly Dictionary<Type, ReflectionInfo> _instanceCache;
		private static readonly Dictionary<Type, ReflectionInfo> _staticCache;

		static ReflectionCache()
		{
			_instanceCache = new Dictionary<Type, ReflectionInfo>();
			_staticCache = new Dictionary<Type, ReflectionInfo>();
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
			return properties;
		}
		private static IEnumerable<SerializationInfo> GetFields(ReflectionInfo info)
		{
			var fields = new List<SerializationInfo>();
			fields.AddRange(info.Fields);
			return fields;
		}
		private static ReflectionInfo InitializeInstanceCache(Type type)
		{
			ReflectionInfo info;
			if (!_instanceCache.TryGetValue(type, out info))
			{
				var read = GetInstanceProperties(type).Where(p => p.GetSetMethod() == null)
				                                      .Where(p => p.GetGetMethod() != null)
				                                      .Where(p => !p.GetCustomAttributes(typeof(JsonIgnoreAttribute), true).Any())
				                                      .Select(BuildSerializationInfo);
				var readWrite = GetInstanceProperties(type).Where(p => p.GetSetMethod() != null)
				                                           .Where(p => p.GetGetMethod() != null)
				                                           .Where(p => !p.GetCustomAttributes(typeof(JsonIgnoreAttribute), true).Any())
				                                           .Select(BuildSerializationInfo);
				var fields = GetInstanceFields(type).Where(p => !p.IsInitOnly)
				                                    .Where(p => !p.GetCustomAttributes(typeof(JsonIgnoreAttribute), true).Any())
				                                    .Select(BuildSerializationInfo);
				_instanceCache[type] = info = new ReflectionInfo(read, readWrite, fields);
			}
			return info;
		}
		private static ReflectionInfo InitializeStaticCache(Type type)
		{
			ReflectionInfo info;
			if (!_staticCache.TryGetValue(type, out info))
			{
				var read = GetStaticProperties(type).Where(p => p.GetSetMethod() == null)
				                                    .Where(p => p.GetGetMethod() != null)
				                                    .Where(p => !p.GetCustomAttributes(typeof(JsonIgnoreAttribute), true).Any())
				                                    .Select(BuildSerializationInfo);
				var readWrite = GetStaticProperties(type).Where(p => p.GetSetMethod() != null)
				                                         .Where(p => p.GetGetMethod() != null)
				                                         .Where(p => !p.GetCustomAttributes(typeof(JsonIgnoreAttribute), true).Any())
				                                         .Select(BuildSerializationInfo);
				var fields = GetStaticFields(type).Where(p => !p.IsInitOnly)
				                                  .Where(p => !p.GetCustomAttributes(typeof(JsonIgnoreAttribute), true).Any())
				                                  .Select(BuildSerializationInfo);
				_staticCache[type] = info = new ReflectionInfo(read, readWrite, fields);
			}
			return info;
		}
		private static SerializationInfo BuildSerializationInfo(MemberInfo info)
		{
			var mapper = (JsonMapToAttribute) info.GetCustomAttributes(typeof(JsonMapToAttribute), false).FirstOrDefault();
			var name = mapper == null ? info.Name : mapper.MapToKey;
			return new SerializationInfo(info, name);
		}
		private static IEnumerable<PropertyInfo> GetInstanceProperties(Type type)
		{
#if IOS
			return type.TypeInfo().DeclaredProperties.Where(p => (!p.GetMethod?.IsStatic ?? false) && (p.GetMethod?.IsPublic ?? false));
#else
			return type.TypeInfo().GetProperties(BindingFlags.Instance | BindingFlags.Public);
#endif
		}
		private static IEnumerable<PropertyInfo> GetStaticProperties(Type type)
		{
#if IOS
			return type.TypeInfo().DeclaredProperties.Where(p => (p.GetMethod?.IsStatic ?? false) && (p.GetMethod?.IsPublic ?? false));
#else
			return type.TypeInfo().GetProperties(BindingFlags.Static | BindingFlags.Public);
#endif
		}
		private static IEnumerable<FieldInfo> GetInstanceFields(Type type)
		{
#if IOS
			return type.TypeInfo().DeclaredFields.Where(f => !f.IsStatic && f.IsPublic);
#else
			return type.TypeInfo().GetFields(BindingFlags.Instance | BindingFlags.Public);
#endif
		}
		private static IEnumerable<FieldInfo> GetStaticFields(Type type)
		{
#if IOS
			return type.TypeInfo().DeclaredFields.Where(f => f.IsStatic && f.IsPublic);
#else
			return type.TypeInfo().GetFields(BindingFlags.Static | BindingFlags.Public);
#endif
		}
	}
}