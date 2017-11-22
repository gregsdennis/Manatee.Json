using System;

namespace Manatee.Json.Serialization
{
	/// <summary>
	/// Provides an interface to map abstract and interface types to
	/// concrete types for object instantiation during deserialization.
	/// </summary>
	[Obsolete("Please use the AbstrationMap class instead.")]
	public static class JsonSerializationAbstractionMap
	{
		/// <summary>
		/// Applies a mapping from an abstraction to a concrete type.
		/// </summary>
		/// <typeparam name="TAbstract">The abstract type.</typeparam>
		/// <typeparam name="TConcrete">The concrete type.</typeparam>
		/// <param name="mappingBehavior">The mapping behavior.</param>
		/// <exception cref="JsonTypeMapException{TAbstract, TConcrete}">Thrown if TConcrete is an
		/// abstract class or an interface.</exception>
		public static void Map<TAbstract, TConcrete>(MapBaseAbstractionBehavior mappingBehavior = MapBaseAbstractionBehavior.Unmapped)
			where TConcrete : TAbstract, new()
		{
			AbstractionMap.Default.Map<TAbstract, TConcrete>(mappingBehavior);
		}
		/// <summary>
		/// Applies a mapping from an open generic abstraction to an open generic concrete type.
		/// </summary>
		/// <param name="tAbstract">The abstract type.</param>
		/// <param name="tConcrete">The concrete type.</param>
		/// <param name="mappingBehavior">The mapping behavior.</param>
		/// <exception cref="JsonTypeMapException">Thrown if <paramref name="tConcrete"/> is an
		/// abstract class or an interface or if <paramref name="tConcrete"/> does not inherit
		/// from <paramref name="tAbstract"/>.</exception>
		public static void MapGeneric(Type tAbstract, Type tConcrete, MapBaseAbstractionBehavior mappingBehavior = MapBaseAbstractionBehavior.Unmapped)
		{
			AbstractionMap.Default.MapGeneric(tAbstract,  tConcrete, mappingBehavior);
		}
		/// <summary>
		/// Removes a previously-assigned mapping.
		/// </summary>
		/// <typeparam name="TAbstract">The type to remove.</typeparam>
		/// <param name="removeRelated">Optionally removes mappings of base and related interface types.</param>
		public static void RemoveMap<TAbstract>(bool removeRelated = true)
		{
			AbstractionMap.Default.RemoveMap<TAbstract>(removeRelated);
		}
		/// <summary>
		/// Retrieves the map setting for an abstraction type.
		/// </summary>
		/// <param name="type">The abstraction type.</param>
		/// <returns>The mapped type if a mapping exists; otherwise the abstraction type.</returns>
		public static Type GetMap(Type type)
		{
			return AbstractionMap.Default.GetMap(type);
		}
	}
}
