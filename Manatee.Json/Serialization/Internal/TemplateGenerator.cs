using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Manatee.Json.Internal;

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
			_buildMethod = typeof(TemplateGenerator).GetTypeInfo().GetDeclaredMethod("BuildInstance");
			_buildMethods = new Dictionary<Type, MethodInfo>();
			_defaultInstances = new Dictionary<Type, object>
				{
					{typeof (string), string.Empty},
					{typeof (Guid), Guid.Empty},
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

			if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				var valueType = type.GetTypeArguments().First();
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

			var properties = type.GetTypeInfo().DeclaredProperties
								 .Where(p => p.SetMethod != null)
								 .Where(p => p.GetMethod != null)
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
			var fields = typeof (T).GetTypeInfo().DeclaredFields
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
			MethodInfo methodInfo;
			if (!_buildMethods.TryGetValue(type, out methodInfo))
			{
				methodInfo = _buildMethod.MakeGenericMethod(type);
				_buildMethods[type] = methodInfo;
			}

			return methodInfo;
		}
	}
}