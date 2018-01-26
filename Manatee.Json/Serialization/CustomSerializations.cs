using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Manatee.Json.Serialization.Internal;

namespace Manatee.Json.Serialization
{
	/// <summary>
	/// Manages methods for serializing object types which do not implement <see cref="IJsonSerializable"/> and
	/// cannot be automatically serialized.
	/// </summary>
	public class CustomSerializations
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

		/// <summary>
		/// Provides a default custom serialization set for new <see cref="JsonSerializer"/> instances.
		/// </summary>
		public static CustomSerializations Default { get; } = new CustomSerializations();

		private static readonly List<ISerializationDelegateProvider> _delegateProviders;
		private static readonly MethodInfo _autoregistrationMethod;
		private readonly Dictionary<Type, Delegate> _toJsonConverters;
		private readonly Dictionary<Type, Delegate> _fromJsonConverters;
		private readonly object _lockHolder = new object();

		static CustomSerializations()
		{
			_delegateProviders = typeof(CustomSerializations).GetTypeInfo().Assembly.DefinedTypes
			                                                 .Where(t => typeof(ISerializationDelegateProvider).GetTypeInfo().IsAssignableFrom(t) &&
			                                                             !t.IsAbstract &&
			                                                             t.IsClass)
			                                                 .Select(ti => Activator.CreateInstance(ti.AsType()))
			                                                 .Cast<ISerializationDelegateProvider>()
			                                                 .ToList();
			_autoregistrationMethod = typeof(CustomSerializations).GetTypeInfo().GetDeclaredMethod(nameof(CustomSerializations._RegisterProviderDelegates));
		}
		/// <summary>
		/// Creates a new <see cref="CustomSerializations"/> instance.
		/// </summary>
		public CustomSerializations()
		{
			_toJsonConverters = new Dictionary<Type, Delegate>();
			_fromJsonConverters = new Dictionary<Type, Delegate>();
		}
		/// <summary>
		/// Creates a new <see cref="CustomSerializations"/> instance using another as a basis.
		/// </summary>
		public CustomSerializations(CustomSerializations other)
		{
			_toJsonConverters = other._toJsonConverters.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
			_fromJsonConverters = other._fromJsonConverters.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
		}

		/// <summary>
		/// Registers an encode/decode method pair for a specific type.
		/// </summary>
		/// <typeparam name="T">The type.</typeparam>
		/// <param name="toJson">The serializer method.</param>
		/// <param name="fromJson">The deserializer method.</param>
		/// <exception cref="TypeRegistrationException">Thrown if either, but not both, <paramref name="toJson"/>
		/// or <paramref name="fromJson"/> is null.</exception>
		public void RegisterType<T>(ToJsonDelegate<T> toJson, FromJsonDelegate<T> fromJson)
		{
			if ((toJson == null && fromJson != null) ||
				(toJson != null && fromJson == null))
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
		public bool IsRegistered<T>()
		{
			return IsRegistered(typeof (T));
		}
		/// <summary>
		/// Gets whether a given type has been entered into the registry.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns>True if an entry exists for the type; otherwise false.</returns>
		public bool IsRegistered(Type type)
		{
			if (_toJsonConverters.ContainsKey(type)) return true;
			if (type.GetTypeInfo().IsGenericTypeDefinition) return false;

			foreach (var delegateProvider in _delegateProviders)
			{
				if (delegateProvider.CanHandle(type))
				{
					var registerMethod = _autoregistrationMethod.MakeGenericMethod(type);
					registerMethod.Invoke(this, new object[] { delegateProvider });
					return true;
				}
			}

			return false;
		}

		internal void Encode<T>(JsonSerializer serializer, T obj, out JsonValue json)
		{
			var converter = _GetToJsonConverter(serializer, obj?.GetType() ?? typeof(T));
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
		internal void Decode<T>(JsonSerializer serializer, JsonValue json, out T obj)
		{
			var converter = _GetFromJsonConverter<T>(serializer);
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

		private Delegate _GetToJsonConverter(JsonSerializer serializer, Type requestedType)
		{
			var type = serializer.AbstractionMap.GetMap(requestedType);
			return _toJsonConverters.ContainsKey(type) ? _toJsonConverters[type] : null;
		}
		private FromJsonDelegate<T> _GetFromJsonConverter<T>(JsonSerializer serializer)
		{
			var type = serializer.AbstractionMap.GetMap(typeof (T));
			return _fromJsonConverters.ContainsKey(type) ? (FromJsonDelegate<T>) _fromJsonConverters[type] : null;
		}
		// ReSharper disable once UnusedMember.Local
		private void _RegisterProviderDelegates<T>(ISerializationDelegateProvider provider)
		{
			var type = typeof (T);
			_toJsonConverters[type] = provider.GetEncoder<T>();
			_fromJsonConverters[type] = provider.GetDecoder<T>();
		}
	}
}
