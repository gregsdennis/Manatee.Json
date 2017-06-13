using System;

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
		/// <typeparam name="T">The type to resolve.</typeparam>
		/// <returns>An instance of the type requested.</returns>
		T Resolve<T>();
		/// <summary>
		/// Resolves an instance of the given type.
		/// </summary>
		/// <param name="type">The type to resolve.</param>
		/// <returns>An instance of the type requested.</returns>
		object Resolve(Type type);
	}
}