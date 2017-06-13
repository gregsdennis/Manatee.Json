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
			var info = _InitializeInstanceCache(type);
			var members = _GetProperties(info, propertyTypes);
			if (includeFields)
				members = members.Concat(_GetFields(info));
			return members;
		}
		public static IEnumerable<SerializationInfo> GetTypeMembers(Type type, PropertySelectionStrategy propertyTypes, bool includeFields)
		{
			var info = _InitializeStaticCache(type);
			var members = _GetProperties(info, propertyTypes);
			if (includeFields)
				members = members.Concat(_GetFields(info));
			return members;
		}

		private static IEnumerable<SerializationInfo> _GetProperties(ReflectionInfo info, PropertySelectionStrategy propertyTypes)
		{
			var properties = new List<SerializationInfo>();
			if ((propertyTypes & PropertySelectionStrategy.ReadWriteOnly) != 0)
				properties.AddRange(info.ReadWriteProperties);
			if ((propertyTypes & PropertySelectionStrategy.ReadOnly) != 0)
				properties.AddRange(info.ReadOnlyProperties);
			return properties;
		}
		private static IEnumerable<SerializationInfo> _GetFields(ReflectionInfo info)
		{
			var fields = new List<SerializationInfo>();
			fields.AddRange(info.Fields);
			return fields;
		}
		private static ReflectionInfo _InitializeInstanceCache(Type type)
		{
			ReflectionInfo info;
			if (!_instanceCache.TryGetValue(type, out info))
			{
				var read = _GetInstanceProperties(type).Where(p => !p.SetMethod.IsPublic)
				                                      .Where(p => p.GetMethod.IsPublic)
				                                      .Where(p => !p.GetCustomAttributes(typeof(JsonIgnoreAttribute), true).Any())
				                                      .Select(_BuildSerializationInfo);
				var readWrite = _GetInstanceProperties(type).Where(p => p.SetMethod.IsPublic)
				                                           .Where(p => p.GetMethod.IsPublic)
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
			ReflectionInfo info;
			if (!_staticCache.TryGetValue(type, out info))
			{
				var read = _GetStaticProperties(type).Where(p => !p.SetMethod.IsPublic)
				                                    .Where(p => p.GetMethod.IsPublic)
				                                    .Where(p => !p.GetCustomAttributes(typeof(JsonIgnoreAttribute), true).Any())
				                                    .Select(_BuildSerializationInfo);
				var readWrite = _GetStaticProperties(type).Where(p => p.SetMethod.IsPublic)
				                                         .Where(p => p.GetMethod.IsPublic)
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
			return new SerializationInfo(info, name);
		}
		private static IEnumerable<PropertyInfo> _GetInstanceProperties(this Type type)
		{
			return type.GetTypeInfo()._GetAllProperties().Where(p => (!p.GetMethod?.IsStatic ?? false) && (p.GetMethod?.IsPublic ?? false));
		}
		private static IEnumerable<PropertyInfo> _GetStaticProperties(this Type type)
		{
			return type.GetTypeInfo()._GetAllProperties().Where(p => (p.GetMethod?.IsStatic ?? false) && (p.GetMethod?.IsPublic ?? false));
		}
		private static IEnumerable<FieldInfo> _GetInstanceFields(this Type type)
		{
			return type.GetTypeInfo()._GetAllFields().Where(f => !f.IsStatic && f.IsPublic);
		}
		private static IEnumerable<FieldInfo> _GetStaticFields(this Type type)
		{
			return type.GetTypeInfo()._GetAllFields().Where(f => f.IsStatic && f.IsPublic);
		}
		private static IEnumerable<PropertyInfo> _GetAllProperties(this TypeInfo type)
		{
			var properties = new List<PropertyInfo>();
			while (type != null)
			{
				properties.AddRange(type.DeclaredProperties);
				type = type.BaseType?.GetTypeInfo();
			}
			return properties;
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