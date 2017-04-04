using System;

namespace Manatee.Json.Serialization
{
	/// <summary>
	/// Thrown when a type cannot be instantiated.
	/// </summary>
	public class TypeInstantiationException : Exception
	{
		/// <summary>
		/// Creates a new instance of the <see cref="TypeInstantiationException"/> class.
		/// </summary>
		/// <param name="type">The type which could not be instantiated.</param>
		public TypeInstantiationException(Type type)
			: base($"Manatee.Json cannot create an instance of type '{type}' through the default resolver." +
			       " You may need to implement your own IResolver to instantiate this type."){}
	}
}