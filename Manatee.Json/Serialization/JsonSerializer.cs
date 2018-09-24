using System;
using Manatee.Json.Pointer;
using Manatee.Json.Serialization.Internal;

namespace Manatee.Json.Serialization
{
	/// <summary>
	/// Serializes and deserializes objects and types to and from JSON structures.
	/// </summary>
	public class JsonSerializer
	{
		private int _callCount;
		private JsonSerializerOptions _options;
		private AbstractionMap _abstractionMap;

		/// <summary>
		/// Gets or sets a set of options for this serializer.
		/// </summary>
		public JsonSerializerOptions Options
		{
			get { return _options ?? (_options = new JsonSerializerOptions(JsonSerializerOptions.Default)); }
			set { _options = value; }
		}
		/// <summary>
		/// Gets or sets the abstraction map used by this serializer.
		/// </summary>
		public AbstractionMap AbstractionMap
		{
			get { return _abstractionMap ?? (_abstractionMap = new AbstractionMap(AbstractionMap.Default)); }
			set { _abstractionMap = value; }
		}

		/// <summary>
		/// Serializes an object to a JSON structure.
		/// </summary>
		/// <typeparam name="T">The type of the object to serialize.</typeparam>
		/// <param name="obj">The object to serialize.</param>
		/// <returns>The JSON representation of the object.</returns>
		public JsonValue Serialize<T>(T obj)
		{
			var context = new SerializationContext(this)
				{
					InferredType = obj?.GetType() ?? typeof(T),
					RequestedType = typeof(T),
					CurrentLocation = new JsonPointer("#"),
					Source = obj
				};
			return Serialize(context);
		}
		/// <summary>
		/// Serializes an object to a JSON structure.
		/// </summary>
		/// <param name="type">The type of the object to serialize.</param>
		/// <param name="obj">The object to serialize.</param>
		/// <returns>The JSON representation of the object.</returns>
		public JsonValue Serialize(Type type, object obj)
		{
			var context = new SerializationContext(this)
				{
					InferredType = obj?.GetType() ?? type,
					RequestedType = type,
					CurrentLocation = new JsonPointer("#"),
					Source = obj
				};
			return Serialize(context);
		}

		internal JsonValue Serialize(SerializationContext context)
		{
			_callCount++;
			var serializer = SerializerFactory.GetSerializer(context);
			var json = serializer.Serialize(context);
			if (--_callCount == 0)
			{
				context.SerializationMap.Clear();
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
			var serializer = SerializerFactory.GetTypeSerializer();
			var json = serializer.SerializeType<T>(this);
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
			return (T) Deserialize(typeof(T), json);
		}
		/// <summary>
		/// Deserializes a JSON structure to an object of the appropriate type.
		/// </summary>
		/// <param name="type">The type of the object that the JSON structure represents.</param>
		/// <param name="json">The JSON representation of the object.</param>
		/// <returns>The deserialized object.</returns>
		/// <exception cref="TypeDoesNotContainPropertyException">Optionally thrown during automatic
		/// deserialization when the JSON contains a property which is not defined by the requested
		/// type.</exception>
		public object Deserialize(Type type, JsonValue json)
		{
			var context = new SerializationContext(this, json)
				{
					InferredType = type,
					RequestedType = type,
					CurrentLocation = new JsonPointer("#"),
					LocalValue = json
				};

			return Deserialize(context);
		}

		internal object Deserialize(SerializationContext context)
		{
			_callCount++;
			var serializer = SerializerFactory.GetSerializer(context);
			var obj = serializer.Deserialize(context);
			if (--_callCount == 0)
			{
				context.SerializationMap.Complete(obj);
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
			var serializer = SerializerFactory.GetTypeSerializer();
			serializer.DeserializeType<T>(json, this);
		}
	}
}
