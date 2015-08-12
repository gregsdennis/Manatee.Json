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
	Namespace:		Manatee.Json.Serialization.Internal
	Class Name:		ReflectionCache
	Purpose:		Caches reflection results for the AutoSerializer.

***************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Manatee.Json.Serialization.Internal
{
	internal class TypeDescription
	{
		public IEnumerable<PropertyInfo> Properties { get; private set; }
		public IEnumerable<FieldInfo> Fields { get; private set; }

		public TypeDescription(IEnumerable<PropertyInfo> properties, IEnumerable<FieldInfo> methods)
		{
			Properties = properties;
			Fields = methods;
		}

		public IEnumerable<MemberInfo> GetSerializableMembers(bool serializeFields)
		{
			return serializeFields ? Properties.Cast<MemberInfo>().Concat(Fields) : Properties;
		}
	}

	internal static class ReflectionCache
	{
		private static readonly Dictionary<Tuple<Type, PropertySelectionStrategy>, TypeDescription> Cache;

		static ReflectionCache()
		{
			Cache = new Dictionary<Tuple<Type, PropertySelectionStrategy>, TypeDescription>();
		}

		public static TypeDescription GetTypeDescription(Type type, PropertySelectionStrategy propertyTypes)
		{
			TypeDescription description;
			var key = new Tuple<Type, PropertySelectionStrategy>(type, propertyTypes);
			if (!Cache.TryGetValue(key, out description))
			{
				description = GetDescription(key);
				Cache[key] = description;
			}
			return description;
		}

		private static TypeDescription GetDescription(Tuple<Type, PropertySelectionStrategy> key)
		{
			var properties = GetProperties(key.Item1, key.Item2);
			var fields = GetFields(key.Item1);
			return new TypeDescription(properties, fields);
		}
		private static IEnumerable<PropertyInfo> GetProperties(Type type, PropertySelectionStrategy propertyTypes)
		{
			var properties = new List<PropertyInfo>();
			if ((propertyTypes & PropertySelectionStrategy.ReadWriteOnly) != 0)
				properties.AddRange(type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
										.Where(p => p.GetSetMethod() != null)
										.Where(p => p.GetGetMethod() != null)
										.Where(p => !p.GetCustomAttributes(typeof(JsonIgnoreAttribute), true).Any()));
			if ((propertyTypes & PropertySelectionStrategy.ReadOnly) != 0)
				properties.AddRange(type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
										.Where(p => p.GetSetMethod() == null)
										.Where(p => p.GetGetMethod() != null)
										.Where(p => !p.GetCustomAttributes(typeof(JsonIgnoreAttribute), true).Any()));
			return properties;
		}
		private static IEnumerable<FieldInfo> GetFields(Type type)
		{
			var fields = new List<FieldInfo>();
			fields.AddRange(type.GetFields(BindingFlags.Instance | BindingFlags.Public)
								.Where(p => !p.IsInitOnly)
								.Where(p => !p.GetCustomAttributes(typeof(JsonIgnoreAttribute), true).Any()));
			return fields;
		}
	}
}