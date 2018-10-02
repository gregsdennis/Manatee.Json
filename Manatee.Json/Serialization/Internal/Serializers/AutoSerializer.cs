using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Manatee.Json.Internal;

namespace Manatee.Json.Serialization.Internal.Serializers
{
	internal class AutoSerializer : IPrioritizedSerializer, ITypeSerializer
	{
		public int Priority => int.MaxValue;

		public bool ShouldMaintainReferences => true;

		public bool Handles(SerializationContext context)
		{
			return true;
		}
		public JsonValue Serialize(SerializationContext context)
		{
			var json = new JsonObject();
			var type = context.RequestedType;
			var objectType = context.Source.GetType();
			bool typeKeyAdded = false;
			if (context.RootSerializer.Options.TypeNameSerializationBehavior != TypeNameSerializationBehavior.Never &&
				(context.RootSerializer.Options.TypeNameSerializationBehavior == TypeNameSerializationBehavior.Always ||
				 type.GetTypeInfo().IsAbstract || type.GetTypeInfo().IsInterface ||
				 (type != objectType && context.RootSerializer.Options.TypeNameSerializationBehavior != TypeNameSerializationBehavior.OnlyForAbstractions)))
			{
				typeKeyAdded = true;
				json.Add(Constants.TypeKey, objectType.AssemblyQualifiedName);
			}
			if (typeKeyAdded || !context.RootSerializer.Options.OnlyExplicitProperties)
				type = context.Source.GetType();
			var propertyList = ReflectionCache.GetMembers(type, context.RootSerializer.Options.PropertySelectionStrategy, context.RootSerializer.Options.AutoSerializeFields);
			var map = _SerializeValues(context, propertyList);
			_ConstructJsonObject(json, map, context.RootSerializer.Options);
			return json;
			//return json.Count == 0 ? JsonValue.Null : json;
		}
		public JsonValue SerializeType<T>(JsonSerializer serializer)
		{
			var json = new JsonObject();
			var type = typeof (T);
			var propertyList = ReflectionCache.GetTypeMembers(type, serializer.Options.PropertySelectionStrategy, serializer.Options.AutoSerializeFields);
			var map = _SerializeTypeValues(serializer, propertyList);
			_ConstructJsonObject(json, map, serializer.Options);
			return json.Count == 0 ? JsonValue.Null : json;
		}
		public object Deserialize(SerializationContext context)
		{
			var json = context.LocalValue;
			var obj = context.RootSerializer.AbstractionMap.CreateInstance(context);
			var type = obj.GetType();
			var propertyList = ReflectionCache.GetMembers(type, context.RootSerializer.Options.PropertySelectionStrategy, context.RootSerializer.Options.AutoSerializeFields);
			var map = _DeserializeValues(context, propertyList, !context.RootSerializer.Options.CaseSensitiveDeserialization);
			if ((json.Object.Count > 0) && (context.RootSerializer.Options.InvalidPropertyKeyBehavior == InvalidPropertyKeyBehavior.ThrowException))
				throw new TypeDoesNotContainPropertyException(type, json);
			_AssignObjectProperties(ref obj, map);
			return obj;
		}
		public void DeserializeType<T>(JsonValue json, JsonSerializer serializer)
		{
			var type = typeof (T);
			var propertyList = ReflectionCache.GetTypeMembers(type, serializer.Options.PropertySelectionStrategy, serializer.Options.AutoSerializeFields);
			var map = _DeserializeTypeValues(json, serializer, propertyList, !serializer.Options.CaseSensitiveDeserialization);
			if ((json.Object.Count > 0) && (serializer.Options.InvalidPropertyKeyBehavior == InvalidPropertyKeyBehavior.ThrowException))
				throw new TypeDoesNotContainPropertyException(type, json);
			object obj = null;
			_AssignObjectProperties(ref obj, map);
		}
		private static Dictionary<SerializationInfo, JsonValue> _SerializeValues(SerializationContext context,
		                                                                         IEnumerable<SerializationInfo> properties)
		{
			var serializer = context.RootSerializer;
			var obj = context.Source;

			var dict = new Dictionary<SerializationInfo, JsonValue>();
			foreach (var property in properties)
			{
				object value;
				Type type;
				if (property.MemberInfo is PropertyInfo propertyInfo)
				{
					if (propertyInfo.GetIndexParameters().Length > 0) continue;
					value = propertyInfo.GetValue(obj, null);
					if (value == null && !serializer.Options.EncodeDefaultValues) continue;
					type = propertyInfo.PropertyType;
				}
				else
				{
					var fieldInfo = (FieldInfo) property.MemberInfo;
					value = fieldInfo.GetValue(obj);
					if (value == null && !serializer.Options.EncodeDefaultValues) continue;
					type = fieldInfo.FieldType;
				}

				var json = JsonValue.Null;
				if (value != null)
				{
					var newLocation = context.CurrentLocation.CloneAndAppend(property.SerializationName);
					var newContext = new SerializationContext(context)
					{
							CurrentLocation = newLocation,
							InferredType = type,
							RequestedType = type,
							Source = value
						};
					json = serializer.Serialize(newContext);
				}

				if (json == JsonValue.Null && !serializer.Options.EncodeDefaultValues) continue;
				if (serializer.Options.IncludeContentSample && json.Type == JsonValueType.Array)
					_AddSample(type, json.Array, serializer);

				dict.Add(property, json);
			}

			return dict;
		}
		private static Dictionary<SerializationInfo, JsonValue> _SerializeTypeValues(JsonSerializer serializer, 
		                                                                             IEnumerable<SerializationInfo> properties)
		{
			var dict = new Dictionary<SerializationInfo, JsonValue>();
			foreach (var memberInfo in properties)
			{
				object value;
				Type type;
				if (memberInfo.MemberInfo is PropertyInfo propertyInfo)
				{
					if (propertyInfo.GetIndexParameters().Any()) continue;
					value = propertyInfo.GetValue(null, null);
					if (value == null) continue;
					type = propertyInfo.PropertyType;
				}
				else
				{
					var fieldInfo = (FieldInfo)memberInfo.MemberInfo;
					value = fieldInfo.GetValue(null);
					if (value == null) continue;
					type = fieldInfo.FieldType;
				}
				var json = serializer.Serialize(type, value);
				if (json == JsonValue.Null && !serializer.Options.EncodeDefaultValues) continue;
				dict.Add(memberInfo, json);
			}
			return dict;
		}
	    private static void _ConstructJsonObject(JsonObject json, Dictionary<SerializationInfo, JsonValue> memberMap, JsonSerializerOptions options)
	    {
	        foreach (var memberInfo in memberMap.Keys)
	        {
	            var name = memberInfo.SerializationName;
	            if (memberInfo.ShouldTransform)
	                name = options.SerializationNameTransform(name);
	            json.Add(name, memberMap[memberInfo]);
	        }
	    }
		private static Dictionary<SerializationInfo, object> _DeserializeValues(SerializationContext context,
		                                                                        IEnumerable<SerializationInfo> members,
		                                                                        bool ignoreCase)
		{
			var dict = new Dictionary<SerializationInfo, object>();
			foreach (var memberInfo in members)
			{
				var name = memberInfo.SerializationName;
				if (memberInfo.ShouldTransform)
					name = context.RootSerializer.Options.DeserializationNameTransform(name);

				var visited = new HashSet<string>(ignoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);
				if (_TryGetKeyValue(context.LocalValue, name, ignoreCase, out var value) &&
				    visited.Add(name))
				{
					Type type;
					if (memberInfo.MemberInfo is PropertyInfo info)
						type = info.PropertyType;
					else
						type = ((FieldInfo) memberInfo.MemberInfo).FieldType;
					var newLocation = context.CurrentLocation.CloneAndAppend(memberInfo.SerializationName);
					var newContext = new SerializationContext(context)
						{
							CurrentLocation = newLocation,
							InferredType = type,
							RequestedType = type,
							LocalValue = value,
						};
					var valueObj = context.RootSerializer.Deserialize(newContext);
					dict.Add(memberInfo, valueObj);
				}
			}

			return dict;
		}
		private static Dictionary<SerializationInfo, object> _DeserializeTypeValues(JsonValue json, JsonSerializer serializer,
		                                                                            IEnumerable<SerializationInfo> members,
		                                                                            bool ignoreCase)
		{
			var dict = new Dictionary<SerializationInfo, object>();
			foreach (var memberInfo in members)
			{
				var name = memberInfo.SerializationName;

				var visited = new HashSet<string>(ignoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);
				if (_TryGetKeyValue(json, name, ignoreCase, out var value) && visited.Add(name))
				{
					Type type;
					if (memberInfo.MemberInfo is PropertyInfo info)
						type = info.PropertyType;
					else
						type = ((FieldInfo) memberInfo.MemberInfo).FieldType;
					var valueObj = serializer.Deserialize(type, value);
					dict.Add(memberInfo, valueObj);
				}
			}

			return dict;
		}
		private static bool _TryGetKeyValue(JsonValue json, string name, bool ignoreCase, out JsonValue value)
		{
			var key = name;
			value = null;

			// PERF: attempt to read the name AS-IS before searching
			if (!json.Object.TryGetValue(name, out value))
			{
				key = null;

				// PERF: if we didn't have an exact match, we must be 'ignore case'.
				//       Otherwise the key does not exist and we should not go
				//       looking for the key.
				if (ignoreCase)
				{
					foreach (var kvp in json.Object)
					{
						if (string.Compare(kvp.Key, name, StringComparison.OrdinalIgnoreCase) != 0) continue;

							key = kvp.Key;
							value = kvp.Value;
							break;
						}
					}
				}

			return key != null;
		}
		private static void _AssignObjectProperties(ref object obj, Dictionary<SerializationInfo, object> memberMap)
		{
			foreach (var entry in memberMap)
			{
				var memberInfo = entry.Key;
				if (memberInfo.MemberInfo is PropertyInfo info)
				{
					if (info.CanWrite)
						info.SetValue(obj, entry.Value, null);
					else if (typeof(IList).GetTypeInfo().IsAssignableFrom(info.PropertyType.GetTypeInfo()))
					{
						var list = (IList) info.GetValue(obj);
						foreach (var value in (IList)entry.Value)
						{
							list.Add(value);
						}
					}
					else if (typeof(IDictionary).GetTypeInfo().IsAssignableFrom(info.PropertyType.GetTypeInfo()))
					{
						var dictionary = (IDictionary) info.GetValue(obj);
						foreach (DictionaryEntry kvp in (IDictionary)entry.Value)
						{
							dictionary.Add(kvp.Key, kvp.Value);
						}
					}
				}
				else
					((FieldInfo) memberInfo.MemberInfo).SetValue(obj, entry.Value);
			}
		}
		private static void _AddSample(Type type, JsonArray json, JsonSerializer serializer)
		{
			var elementType = _GetElementType(type);
			var buildMethod = TemplateGenerator.GetBuildMethod(elementType);
			var value = buildMethod.Invoke(null, new object[] {serializer.Options});
			json.Add(serializer.Serialize(elementType, value));
		}
		private static Type _GetElementType(Type collectionType)
		{
			if (collectionType.IsArray)
				return collectionType.GetElementType();
			if (collectionType.GetTypeInfo().IsGenericType && collectionType.GetGenericTypeDefinition().InheritsFrom(typeof(IEnumerable<>)))
				return collectionType.GetTypeArguments().First();
			return typeof (object);
		}
	}
}