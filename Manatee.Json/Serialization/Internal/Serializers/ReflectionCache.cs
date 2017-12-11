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
				ReadOnlyProperties = readOnlyProperties.ToList();
				ReadWriteProperties = readWriteProperties.ToList();
				Fields = fields.ToList();
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
			var info = _InitializeInstanceCache(type);
			var members = new List<SerializationInfo>();
			_GetProperties(info, propertyTypes, members);
			if (includeFields)
				_GetFields(info, members);
			return members;
		}
		public static IEnumerable<SerializationInfo> GetTypeMembers(Type type, PropertySelectionStrategy propertyTypes, bool includeFields)
		{
			var info = _InitializeStaticCache(type);
			var members = new List<SerializationInfo>();
			_GetProperties(info, propertyTypes, members);
			if (includeFields)
				_GetFields(info, members);
			return members;
		}

		private static IEnumerable<SerializationInfo> _GetProperties(ReflectionInfo info, PropertySelectionStrategy propertyTypes, List<SerializationInfo> properties)
		{
			if (propertyTypes.HasFlag(PropertySelectionStrategy.ReadWriteOnly))
				properties.AddRange(info.ReadWriteProperties);
			if (propertyTypes.HasFlag(PropertySelectionStrategy.ReadOnly))
				properties.AddRange(info.ReadOnlyProperties);
			return properties;
		}
		private static IEnumerable<SerializationInfo> _GetFields(ReflectionInfo info, List<SerializationInfo> fields)
		{
			fields.AddRange(info.Fields);
			return fields;
		}
		private static ReflectionInfo _InitializeInstanceCache(Type type)
		{
			if (!_instanceCache.TryGetValue(type, out ReflectionInfo info))
			{
				var read = _GetInstanceProperties(type).Where(p => !p.SetMethod?.IsPublic ?? true)
				                                       .Where(p => p.GetMethod?.IsPublic ?? false)
				                                       .Where(p => !p.GetCustomAttributes(typeof(JsonIgnoreAttribute), true).Any())
				                                       .Select(_BuildSerializationInfo);
				var readWrite = _GetInstanceProperties(type).Where(p => p.SetMethod?.IsPublic ?? false)
				                                            .Where(p => p.GetMethod?.IsPublic ?? false)
				                                            .Where(p => !p.GetCustomAttributes(typeof(JsonIgnoreAttribute), true).Any())
				                                            .Select(_BuildSerializationInfo);
				var fields = _GetInstanceFields(type).Where(p => !p.IsInitOnly)
				                                     .Where(p => !p.GetCustomAttributes(typeof(JsonIgnoreAttribute), true).Any())
				                                     .Select(_BuildSerializationInfo);
				_instanceCache[type] = info = new ReflectionInfo(read, readWrite, fields);
			}
			return info;
		}
		private static ReflectionInfo _InitializeStaticCache(Type type)
		{
			if (!_staticCache.TryGetValue(type, out ReflectionInfo info))
			{
				var read = _GetStaticProperties(type).Where(p => !p.SetMethod?.IsPublic ?? true)
				                                     .Where(p => p.GetMethod?.IsPublic ?? false)
				                                     .Where(p => !p.GetCustomAttributes(typeof(JsonIgnoreAttribute), true).Any())
				                                     .Select(_BuildSerializationInfo);
				var readWrite = _GetStaticProperties(type).Where(p => p.SetMethod?.IsPublic ?? false)
				                                          .Where(p => p.GetMethod?.IsPublic ?? false)
				                                          .Where(p => !p.GetCustomAttributes(typeof(JsonIgnoreAttribute), true).Any())
				                                          .Select(_BuildSerializationInfo);
				var fields = _GetStaticFields(type).Where(p => !p.IsInitOnly)
				                                   .Where(p => !p.GetCustomAttributes(typeof(JsonIgnoreAttribute), true).Any())
				                                   .Select(_BuildSerializationInfo);
				_staticCache[type] = info = new ReflectionInfo(read, readWrite, fields);
			}
			return info;
		}
		private static SerializationInfo _BuildSerializationInfo(MemberInfo info)
		{
			var mapper = (JsonMapToAttribute) info.GetCustomAttributes(typeof(JsonMapToAttribute), false).FirstOrDefault();
			var name = mapper == null ? info.Name : mapper.MapToKey;
			return new SerializationInfo(info, name, mapper == null);
		}
		private static IEnumerable<PropertyInfo> _GetInstanceProperties(this Type type)
		{
			return type.GetTypeInfo().GetAllProperties().Where(p => (!p.GetMethod?.IsStatic ?? false) && (p.GetMethod?.IsPublic ?? false));
		}
		private static IEnumerable<PropertyInfo> _GetStaticProperties(this Type type)
		{
			return type.GetTypeInfo().GetAllProperties().Where(p => (p.GetMethod?.IsStatic ?? false) && (p.GetMethod?.IsPublic ?? false));
		}
		private static IEnumerable<FieldInfo> _GetInstanceFields(this Type type)
		{
			return type.GetTypeInfo()._GetAllFields().Where(f => !f.IsStatic && f.IsPublic);
		}
		private static IEnumerable<FieldInfo> _GetStaticFields(this Type type)
		{
			return type.GetTypeInfo()._GetAllFields().Where(f => f.IsStatic && f.IsPublic);
		}
		private static IEnumerable<FieldInfo> _GetAllFields(this TypeInfo type)
		{
			var fields = new List<FieldInfo>();
			while (type != null)
			{
				fields.AddRange(type.DeclaredFields);
				type = type.BaseType?.GetTypeInfo();
			}
			return fields;
		}
	}
}