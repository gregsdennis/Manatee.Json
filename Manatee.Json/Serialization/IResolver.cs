using System;
using System.Collections.Generic;

namespace Manatee.Json.Serialization
{
	/// <summary>
	/// Defines methods required to resolved instances for deserialization.
	/// </summary>
	public interface IResolver
	{
		/// <summary>
		/// Resolves an instance of the given type.
		/// </summary>
		/// <param name="type">The type to resolve.</param>
		/// <param name="parameters">Parameters to use for construction of the object.</param>
		/// <returns>An instance of the type requested.</returns>
		object? Resolve(Type type, Dictionary<SerializationInfo, object> parameters);
	}
}