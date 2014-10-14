/***************************************************************************************

	Copyright 2014 Greg Dennis

	   Licensed under the Apache License, Version 2.0 (the "License");
	   you may not use this file except in compliance with the License.
	   You may obtain a copy of the License at

		 http://www.apache.org/licenses/LICENSE-2.0

	   Unless required by applicable law or agreed to in writing, software
	   distributed under the License is distributed on an "AS IS" BASIS,
	   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	   See the License for the specific language governing permissions and
	   limitations under the License.
 
	File Name:		TemplateGenerator.cs
	Namespace:		Manatee.Json.Serialization.Internal
	Class Name:		TemplateGenerator
	Purpose:		Creates JSON templates to represent types.

***************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Manatee.Json.Serialization.Internal
{

	internal static class TemplateGenerator
	{
		private static readonly MethodInfo _buildMethod;
		private static readonly Dictionary<Type, MethodInfo> _buildMethods;
		private static readonly Dictionary<Type, object> _defaultInstances;

		[ThreadStatic]
		private static List<Type> _generatedTypes;

		static TemplateGenerator()
		{
			_buildMethod = typeof (TemplateGenerator).GetMethod("BuildInstance", BindingFlags.Static | BindingFlags.NonPublic);
			_buildMethods = new Dictionary<Type, MethodInfo>();
			_defaultInstances = new Dictionary<Type, object>
				{
					{typeof (string), string.Empty},
				};
		}

		public static JsonValue FromType<T>(JsonSerializer serializer)
		{
			var encodeDefaultValues = serializer.Options.EncodeDefaultValues;
			serializer.Options.EncodeDefaultValues = true;
			serializer.Options.IncludeContentSample = true;
			_generatedTypes = new List<Type>();

			var instance = BuildInstance<T>(serializer.Options);

			var json = serializer.Serialize(instance);

			serializer.Options.IncludeContentSample = false;
			serializer.Options.EncodeDefaultValues = encodeDefaultValues;
			return json;
		}

		private static T BuildInstance<T>(JsonSerializerOptions options)
		{
			var type = typeof (T);

			if (_defaultInstances.ContainsKey(type))
				return (T) _defaultInstances[type];

			if (_generatedTypes.Contains(type)) return default(T);

			_generatedTypes.Add(type);
			T instance;

			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof (Nullable<>))
			{
				var valueType = type.GetGenericArguments().First();
				var buildMethod = GetBuildMethod(valueType);
				var value = buildMethod.Invoke(null, new object[] {options});
				instance = (T) value;
			}
			else
			{
				instance = JsonSerializationAbstractionMap.CreateInstance<T>(null, options.Resolver);
				FillProperties(instance, options);
				if (options.AutoSerializeFields)
					FillFields(instance, options);
			}

			_defaultInstances[type] = instance;

			return instance;
		}
		private static void FillProperties<T>(T instance, JsonSerializerOptions options)
		{
			var type = typeof (T);

			var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
								 .Where(p => p.GetSetMethod() != null)
								 .Where(p => p.GetGetMethod() != null)
								 .Where(p => !p.GetCustomAttributes(typeof (JsonIgnoreAttribute), true).Any());
			foreach (var propertyInfo in properties)
			{
				var propertyType = propertyInfo.PropertyType;
				var indexParameters = propertyInfo.GetIndexParameters().ToList();
				if (indexParameters.Any()) continue;
				var value = GetValue(options, propertyType);
				propertyInfo.SetValue(instance, value, null);
			}
		}
		private static void FillFields<T>(T instance, JsonSerializerOptions options)
		{
			var fields = typeof (T).GetFields(BindingFlags.Instance | BindingFlags.Public)
								   .Where(p => !p.IsInitOnly)
								   .Where(p => !p.GetCustomAttributes(typeof (JsonIgnoreAttribute), true).Any());
			foreach (var fieldInfo in fields)
			{
				var propertyType = fieldInfo.FieldType;
				var buildMethod = GetBuildMethod(propertyType);
				var value = buildMethod.Invoke(null, new object[] {options});
				fieldInfo.SetValue(instance, value);
			}
		}
		private static object GetValue(JsonSerializerOptions options, Type propertyType)
		{
			var buildMethod = GetBuildMethod(propertyType);
			var value = buildMethod.Invoke(null, new object[] {options});
			return value;
		}
		internal static MethodInfo GetBuildMethod(Type type)
		{
			if (_buildMethods.ContainsKey(type))
				return _buildMethods[type];

			var method = _buildMethod.MakeGenericMethod(type);
			_buildMethods[type] = method;
			return method;
		}
	}
}