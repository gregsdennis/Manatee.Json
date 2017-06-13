﻿using Manatee.Json.Serialization.Internal;

namespace Manatee.Json.Serialization
{
	/// <summary>
	/// Serializes and deserializes objects and types to and from JSON structures.
	/// </summary>
	public class JsonSerializer
	{
		private int _callCount;
		private JsonSerializerOptions _options;

		/// <summary>
		/// Gets or sets a set of options for the serializer.
		/// </summary>
		public JsonSerializerOptions Options
		{
			get { return _options ?? (_options = new JsonSerializerOptions(JsonSerializerOptions.Default)); }
			set { _options = value ?? new JsonSerializerOptions(JsonSerializerOptions.Default); }
		}
		internal SerializationPairCache SerializationMap { get; } = new SerializationPairCache();

		#region Public Methods
		/// <summary>
		/// Serializes an object to a JSON structure.
		/// </summary>
		/// <typeparam name="T">The type of the object to serialize.</typeparam>
		/// <param name="obj">The object to serialize.</param>
		/// <returns>The JSON representation of the object.</returns>
		public JsonValue Serialize<T>(T obj)
		{
			_callCount++;
			var serializer = SerializerFactory.GetSerializer<T>(Options);
			var json = serializer.Serialize(obj, this);
			if (--_callCount == 0)
			{
				SerializationMap.Clear();
			}
			return json;
		}
		/// <summary>
		/// Serializes the public static properties of a type to a JSON structure.
		/// </summary>
		/// <typeparam name="T">The type to serialize.</typeparam>
		/// <returns>The JSON representation of the type.</returns>
		public JsonValue SerializeType<T>()
		{
			var serializer = SerializerFactory.GetTypeSerializer<T>(Options);
			var json = serializer.SerializeType<T>(this);
			SerializationMap.Clear();
			return json;
		}
		/// <summary>
		/// Generates a template JSON inserting default values.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public JsonValue GenerateTemplate<T>()
		{
			return TemplateGenerator.FromType<T>(this);
		}
		/// <summary>
		/// Deserializes a JSON structure to an object of the appropriate type.
		/// </summary>
		/// <typeparam name="T">The type of the object that the JSON structure represents.</typeparam>
		/// <param name="json">The JSON representation of the object.</param>
		/// <returns>The deserialized object.</returns>
		/// <exception cref="TypeDoesNotContainPropertyException">Optionally thrown during automatic
		/// deserialization when the JSON contains a property which is not defined by the requested
		/// type.</exception>
		public T Deserialize<T>(JsonValue json)
		{
			_callCount++;
			var serializer = SerializerFactory.GetSerializer<T>(Options, json);
			var obj = serializer.Deserialize<T>(json, this);
			if (--_callCount == 0)
			{
				SerializationMap.Clear();
			}
			return obj;
		}
		/// <summary>
		/// Deserializes a JSON structure to the public static properties of a type.
		/// </summary>
		/// <typeparam name="T">The type to deserialize.</typeparam>
		/// <param name="json">The JSON representation of the type.</param>
		/// <exception cref="TypeDoesNotContainPropertyException">Optionally thrown during automatic
		/// deserialization when the JSON contains a property which is not defined by the requested
		/// type.</exception>
		public void DeserializeType<T>(JsonValue json)
		{
			var serializer = SerializerFactory.GetTypeSerializer<T>(Options);
			serializer.DeserializeType<T>(json, this);
		}
		#endregion
	}
}
