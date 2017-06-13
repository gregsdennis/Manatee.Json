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
				var read = GetInstanceProperties(type).Where(p => !p.SetMethod.IsPublic)
				                                      .Where(p => p.GetMethod.IsPublic)
				                                      .Where(p => !p.GetCustomAttributes(typeof(JsonIgnoreAttribute), true).Any())
				                                      .Select(BuildSerializationInfo);
				var readWrite = GetInstanceProperties(type).Where(p => p.SetMethod.IsPublic)
				                                           .Where(p => p.GetMethod.IsPublic)
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
				var read = GetStaticProperties(type).Where(p => !p.SetMethod.IsPublic)
				                                    .Where(p => p.GetMethod.IsPublic)
				                                    .Where(p => !p.GetCustomAttributes(typeof(JsonIgnoreAttribute), true).Any())
				                                    .Select(BuildSerializationInfo);
				var readWrite = GetStaticProperties(type).Where(p => p.SetMethod.IsPublic)
				                                         .Where(p => p.GetMethod.IsPublic)
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
			return type.TypeInfo().DeclaredProperties.Where(p => (!p.GetMethod?.IsStatic ?? false) && (p.GetMethod?.IsPublic ?? false));
		}
		private static IEnumerable<PropertyInfo> GetStaticProperties(Type type)
		{
			return type.TypeInfo().DeclaredProperties.Where(p => (p.GetMethod?.IsStatic ?? false) && (p.GetMethod?.IsPublic ?? false));
		}
		private static IEnumerable<FieldInfo> GetInstanceFields(Type type)
		{
			return type.TypeInfo().DeclaredFields.Where(f => !f.IsStatic && f.IsPublic);
		}
		private static IEnumerable<FieldInfo> GetStaticFields(Type type)
		{
			return type.TypeInfo().DeclaredFields.Where(f => f.IsStatic && f.IsPublic);
		}
	}
}