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

		public void Push(Type inferredType, Type requestedType, string? propertyName, object? source)
		{
			Push(inferredType, requestedType, propertyName);

			_sources.Push(Source);
			Source = source;
		}
		public void Pop()
		{
			base.Pop();

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