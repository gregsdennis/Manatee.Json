using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Manatee.Json.Internal;
using Manatee.Json.Serialization.Internal;

namespace Manatee.Json.Serialization
{
	/// <summary>
	/// Manages methods for serializing object types which do not implement <see cref="IJsonSerializable"/> and
	/// cannot be automatically serialized.
	/// </summary>
	public static class JsonSerializationTypeRegistry
	{
		/// <summary>
		/// Declares the required signature for a serializer method.
		/// </summary>
		/// <typeparam name="T">The type which the method serializes.</typeparam>
		/// <param name="input">The object to be serialized.</param>
		/// <param name="serializer">The serializer to be used.</param>
		/// <returns>The JSON representation of the object.</returns>
		public delegate JsonValue ToJsonDelegate<in T>(T input, JsonSerializer serializer);

		/// <summary>
		/// Declares the required signature for a deserializer method.
		/// </summary>
		/// <typeparam name="T">The type which the method deserializes.</typeparam>
		/// <param name="json">The JSON representation of the object.</param>
		/// <param name="serializer">The serializer to be used.</param>
		/// <returns>The deserialized object.</returns>
		public delegate T FromJsonDelegate<out T>(JsonValue json, JsonSerializer serializer);

		private static readonly List<ISerializationDelegateProvider> _delegateProviders;
		private static readonly Dictionary<Type, Delegate> _toJsonConverters;
		private static readonly Dictionary<Type, Delegate> _fromJsonConverters;
		private static readonly object _lockHolder = new object();
		private static readonly MethodInfo _autoregistrationMethod;

		static JsonSerializationTypeRegistry()
		{
			_delegateProviders = typeof(JsonSerializationTypeRegistry).TypeInfo().Assembly.GetTypes()
																	  .Where(t => typeof(ISerializationDelegateProvider).IsAssignableFrom(t) &&
																				  !t.TypeInfo().IsAbstract &&
																				  t.TypeInfo().IsClass)
																	  .Select(Activator.CreateInstance)
																	  .Cast<ISerializationDelegateProvider>()
																	  .ToList();
			_toJsonConverters = new Dictionary<Type, Delegate>();
			_fromJsonConverters = new Dictionary<Type, Delegate>();
#if IOS
			_autoregistrationMethod = typeof (JsonSerializationTypeRegistry).GetMethod("RegisterProviderDelegates");
#else
			_autoregistrationMethod = typeof (JsonSerializationTypeRegistry).GetMethod("RegisterProviderDelegates", BindingFlags.Static | BindingFlags.NonPublic);
#endif
		}

		/// <summary>
		/// Registers an encode/decode method pair for a specific type.
		/// </summary>
		/// <typeparam name="T">The type.</typeparam>
		/// <param name="toJson">The serializer method.</param>
		/// <param name="fromJson">The deserializer method.</param>
		/// <exception cref="TypeRegistrationException">Thrown if either, but not both, <paramref name="toJson"/>
		/// or <paramref name="fromJson"/> is null.</exception>
		public static void RegisterType<T>(ToJsonDelegate<T> toJson, FromJsonDelegate<T> fromJson)
		{
			if (((toJson == null) && (fromJson != null)) ||
				((toJson != null) && (fromJson == null)))
				throw new TypeRegistrationException(typeof(T));
			var type = typeof(T);
			if (toJson == null)
			{
				_toJsonConverters.Remove(type);
				_fromJsonConverters.Remove(type);
				return;
			}
			_toJsonConverters[type] = toJson;
			_fromJsonConverters[type] = fromJson;
		}
		/// <summary>
		/// Gets whether a given type has been entered into the registry.
		/// </summary>
		/// <typeparam name="T">The type.</typeparam>
		/// <returns>True if an entry exists for the type; otherwise false.</returns>
		public static bool IsRegistered<T>()
		{
			return IsRegistered(typeof (T));
		}
		/// <summary>
		/// Gets whether a given type has been entered into the registry.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns>True if an entry exists for the type; otherwise false.</returns>
		public static bool IsRegistered(Type type)
		{
			if (_toJsonConverters.ContainsKey(type)) return true;
			if (type.TypeInfo().IsGenericTypeDefinition) return false;

			var delegateProvider = _delegateProviders.FirstOrDefault(p => p.CanHandle(type));
			if (delegateProvider == null) return false;

			var registerMethod = _autoregistrationMethod.MakeGenericMethod(type);
			registerMethod.Invoke(null, new object[] { delegateProvider });
			return true;
		}

		internal static void Encode<T>(this JsonSerializer serializer, T obj, out JsonValue json)
		{
			var converter = GetToJsonConverter<T>();
			if (converter == null)
			{
				json = null;
				return;
			}
			lock (_lockHolder)
			{
				json = (JsonValue) converter.DynamicInvoke(obj, serializer);
			}
		}
		internal static void Decode<T>(this JsonSerializer serializer, JsonValue json, out T obj)
		{
			var converter = GetFromJsonConverter<T>();
			if (converter == null)
			{
				obj = default(T);
				return;
			}
			lock (_lockHolder)
			{
				obj = converter(json, serializer);
			}
		}

		private static Delegate GetToJsonConverter<T>()
		{
			var type = JsonSerializationAbstractionMap.GetMap(typeof(T));
			return _toJsonConverters.ContainsKey(type) ? _toJsonConverters[type] : null;
		}
		private static FromJsonDelegate<T> GetFromJsonConverter<T>()
		{
			var type = JsonSerializationAbstractionMap.GetMap(typeof (T));
			return _fromJsonConverters.ContainsKey(type) ? (FromJsonDelegate<T>) _fromJsonConverters[type] : null;
		}
		// ReSharper disable once UnusedMember.Local
		private static void RegisterProviderDelegates<T>(ISerializationDelegateProvider provider)
		{
			var type = typeof (T);
			_toJsonConverters[type] = provider.GetEncoder<T>();
			_fromJsonConverters[type] = provider.GetDecoder<T>();
		}
	}
}
