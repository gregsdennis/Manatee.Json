/***************************************************************************************

	Copyright 2012 Greg Dennis

	   Licensed under the Apache License, Version 2.0 (the "License");
	   you may not use this file except in compliance with the License.
	   You may obtain a copy of the License at

		 http://www.apache.org/licenses/LICENSE-2.0

	   Unless required by applicable law or agreed to in writing, software
	   distributed under the License is distributed on an "AS IS" BASIS,
	   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	   See the License for the specific language governing permissions and
	   limitations under the License.
 
	File Name:		JsonSerializationTypeRegistry.cs
	Namespace:		Manatee.Json.Serialization
	Class Name:		JsonSerializationTypeRegistry
	Purpose:		Manages methods for serializing object types which do not
					implement IJsonCompatible and cannot be automatically serialized.

***************************************************************************************/

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

		private static readonly List<ISerializationDelegateProvider> DelegateProviders;
		private static readonly Dictionary<Type, Delegate> ToJsonConverters;
		private static readonly Dictionary<Type, Delegate> FromJsonConverters;
		private static readonly object LockHolder = new object();
		private static readonly MethodInfo AutoregistrationMethod;

		static JsonSerializationTypeRegistry()
		{
			DelegateProviders = typeof (JsonSerializationTypeRegistry).Assembly.GetTypes()
																	  .Where(t => typeof (ISerializationDelegateProvider).IsAssignableFrom(t) &&
																				  !t.IsAbstract &&
																				  t.IsClass)
																	  .Select(Activator.CreateInstance)
																	  .Cast<ISerializationDelegateProvider>()
																	  .ToList();
			ToJsonConverters = new Dictionary<Type, Delegate>();
			FromJsonConverters = new Dictionary<Type, Delegate>();
			AutoregistrationMethod = typeof (JsonSerializationTypeRegistry).GetMethod("RegisterProviderDelegates", BindingFlags.Static | BindingFlags.NonPublic);
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
				ToJsonConverters.Remove(type);
				FromJsonConverters.Remove(type);
				return;
			}
			ToJsonConverters[type] = toJson;
			FromJsonConverters[type] = fromJson;
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
			if (ToJsonConverters.ContainsKey(type)) return true;
			if (type.IsGenericTypeDefinition) return false;

			var delegateProvider = DelegateProviders.FirstOrDefault(p => p.CanHandle(type));
			if (delegateProvider == null) return false;

			var registerMethod = AutoregistrationMethod.MakeGenericMethod(type);
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
			lock (LockHolder)
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
			lock (LockHolder)
			{
				obj = converter(json, serializer);
			}
		}

		private static Delegate GetToJsonConverter<T>()
		{
			var type = JsonSerializationAbstractionMap.GetMap(typeof(T));
			return ToJsonConverters.ContainsKey(type) ? ToJsonConverters[type] : null;
		}
		private static FromJsonDelegate<T> GetFromJsonConverter<T>()
		{
			var type = JsonSerializationAbstractionMap.GetMap(typeof (T));
			return FromJsonConverters.ContainsKey(type) ? (FromJsonDelegate<T>) FromJsonConverters[type] : null;
		}
		// ReSharper disable once UnusedMember.Local
		private static void RegisterProviderDelegates<T>(ISerializationDelegateProvider provider)
		{
			var type = typeof (T);
			ToJsonConverters[type] = provider.GetEncoder<T>();
			FromJsonConverters[type] = provider.GetDecoder<T>();
		}
	}
}
