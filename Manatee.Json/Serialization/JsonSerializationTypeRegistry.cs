using System;

namespace Manatee.Json.Serialization
{
	/// <summary>
	/// Manages methods for serializing object types which do not implement <see cref="IJsonSerializable"/> and
	/// cannot be automatically serialized.
	/// </summary>
	[Obsolete("Please use SerializerFactory.UserSerializers instead.")]
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
			CustomSerializations.Default.RegisterType<T>((o, s) => toJson(o, s), (j, s) => fromJson(j, s));
		}
		/// <summary>
		/// Gets whether a given type has been entered into the registry.
		/// </summary>
		/// <typeparam name="T">The type.</typeparam>
		/// <returns>True if an entry exists for the type; otherwise false.</returns>
		public static bool IsRegistered<T>()
		{
			return CustomSerializations.Default.IsRegistered<T>();
		}
		/// <summary>
		/// Gets whether a given type has been entered into the registry.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns>True if an entry exists for the type; otherwise false.</returns>
		public static bool IsRegistered(Type type)
		{
			return CustomSerializations.Default.IsRegistered(type);
		}
	}
}
