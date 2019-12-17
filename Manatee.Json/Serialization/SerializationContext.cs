using System;
using System.Collections.Generic;

namespace Manatee.Json.Serialization
{
	/// <summary>
	/// Encapsulates all data required to serialize an object to JSON.
	/// </summary>
	public class SerializationContext : SerializationContextBase
	{
		private readonly Stack<object?> _sources = new Stack<object?>();

		/// <summary>
		/// The current value in the object for serialization.
		/// </summary>
		public object? Source { get; private set; }

		internal SerializationContext(JsonSerializer rootSerializer)
			: base(rootSerializer)
		{
		}

		/// <summary>
		/// Pushes new details onto the context to allow for recursive serialization.
		/// </summary>
		/// <param name="inferredType">The type to be serialized as inferred by the system.</param>
		/// <param name="requestedType">The type to be serialized as requested by the caller.</param>
		/// <param name="propertyName">The property name or index.  Will be appended to the location as a JSON Path segment.</param>
		/// <param name="source">The object being serialized.</param>
		public void Push(Type inferredType, Type requestedType, string propertyName, object? source)
		{
			PushDetails(inferredType, requestedType, propertyName);

			_sources.Push(Source);
			Source = source;
		}
		/// <summary>
		/// Pops details from the context to keep the context in sync with the serialization process.
		/// </summary>
		public void Pop()
		{
			PopDetails();

			Source = _sources.Pop();
		}

		internal void OverrideSource(object source)
		{
			_sources.Pop();
			Source = source;
			_sources.Push(source);
		}
	}
}