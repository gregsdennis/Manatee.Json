using System;
using System.Collections.Generic;
using Manatee.Json.Pointer;
using Manatee.Json.Serialization.Internal;

namespace Manatee.Json.Serialization
{
	/// <summary>
	/// Base class for <see cref="SerializationContext"/> and <see cref="DeserializationContext"/>.
	/// </summary>
	public class SerializationContextBase
	{
		private readonly Stack<Type> _requestedTypes = new Stack<Type>();
		private readonly Stack<Type> _inferredTypes = new Stack<Type>();

		/// <summary>
		/// The inferred type.  This will usually be the actual type of the object during serialization.
		/// </summary>
		public Type InferredType { get; private set; } = null!;
		/// <summary>
		/// The type requested by the serialize/deserialize call.
		/// </summary>
		public Type RequestedType { get; private set; } = null!;
		/// <summary>
		/// The current location in the JSON instance or object.
		/// </summary>
		public JsonPointer CurrentLocation { get; } = new JsonPointer("#");
		/// <summary>
		/// The original serializer called by the client.
		/// </summary>
		public JsonSerializer RootSerializer { get; }

		internal SerializationReferenceCache SerializationMap { get; }

		internal SerializationContextBase(JsonSerializer rootSerializer)
		{
			SerializationMap = new SerializationReferenceCache();
			RootSerializer = rootSerializer;
		}

		internal void OverrideInferredType(Type type)
		{
			_inferredTypes.Pop();
			_inferredTypes.Push(InferredType);
			InferredType = type;
		}

		private protected void Push(Type inferredType, Type requestedType, string? propertyName)
		{
			_inferredTypes.Push(InferredType);
			InferredType = inferredType;

			_requestedTypes.Push(RequestedType);
			RequestedType = requestedType;

			CurrentLocation.Add(propertyName);
		}

		private protected void Pop()
		{
			InferredType = _inferredTypes.Pop();
			RequestedType = _requestedTypes.Pop();
			CurrentLocation.RemoveAt(CurrentLocation.Count - 1);
		}
	}
}