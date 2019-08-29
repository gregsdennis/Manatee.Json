using System;
using System.Collections.Generic;
using Manatee.Json.Pointer;
using Manatee.Json.Serialization.Internal;
using Manatee.Json.Serialization.Internal.Serializers;

namespace Manatee.Json.Serialization
{
	/// <summary>
	/// Encapsulates all data required to serialize and deserialize object to and from JSON.
	/// </summary>
	public class SerializationContext
	{
		/// <summary>
		/// The inferred type.  This will usually be the actual type of the object during serialization
		/// or the <see cref="RequestedType"/> during deserialization.
		/// </summary>
		public Type InferredType { get; set; }
		/// <summary>
		/// The type requested by the serialize/deserialize call.
		/// </summary>
		public Type RequestedType { get; set; }
		/// <summary>
		/// The current location in the JSON instance or object.
		/// </summary>
		public JsonPointer CurrentLocation { get; set; }
		/// <summary>
		/// The current value in the JSON instance for deserialization.
		/// </summary>
		public JsonValue LocalValue { get; set; }
		/// <summary>
		/// The current value in the object for serialization.
		/// </summary>
		public object Source { get; set; }

		/// <summary>
		/// The root of the JSON instance.
		/// </summary>
		public JsonValue JsonRoot { get; }
		/// <summary>
		/// The original serializer called by the client.
		/// </summary>
		public JsonSerializer RootSerializer { get; }
		/// <summary>
		/// A mapping of the deserialized values to the type's property information.
		/// </summary>
		public Dictionary<SerializationInfo, object> ValueMap { get; set; }

		internal SerializationReferenceCache SerializationMap { get; }

		/// <summary>
		/// Creates a new instance of the <see cref="SerializationContext"/> class.
		/// </summary>
		/// <param name="other"></param>
		public SerializationContext(SerializationContext other)
		{
			SerializationMap = other.SerializationMap;
			RootSerializer = other.RootSerializer;
			JsonRoot = other.JsonRoot;
		}
		internal SerializationContext(JsonSerializer rootSerializer, JsonValue jsonRoot = null)
		{
			SerializationMap = new SerializationReferenceCache();
			RootSerializer = rootSerializer;
			JsonRoot = jsonRoot;
		}
	}
}