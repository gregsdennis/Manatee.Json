using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Manatee.Json.Enumerations;
using Manatee.Json.Exceptions;

namespace Manatee.Json.Serialization
{
	/// <summary>
	/// Provides an interface to map abstract and interface types to
	/// concrete types for object instantiation during deserialization.
	/// </summary>
	public static class JsonSerializationAbstractionMap
	{
		private static readonly Dictionary<Type, Type> _registry = new Dictionary<Type, Type>();

		/// <summary>
		/// Applies a mapping from an abstraction to a concrete type.
		/// </summary>
		/// <typeparam name="TAbstract">The abstract type.</typeparam>
		/// <typeparam name="TConcrete">The concrete type.</typeparam>
		public static void Map<TAbstract, TConcrete>()
			where TConcrete : TAbstract, new()
		{
			if (typeof(TConcrete).IsAbstract || typeof(TConcrete).IsInterface)
				throw new JsonTypeMapException<TAbstract, TConcrete>();
			_registry[typeof (TAbstract)] = typeof (TConcrete);
		}
		/// <summary>
		/// Removes a previously-assigned mapping.
		/// </summary>
		/// <typeparam name="TAbstract">The type to remove.</typeparam>
		public static void RemoveMap<TAbstract>()
		{
			_registry.Remove(typeof (TAbstract));
		}

		internal static T CreateInstance<T>(JsonValue json)
		{
			var type = typeof (T);
			if (type.IsAbstract || type.IsInterface)
			{
				if ((json.Type == JsonValueType.Object) && (json.Object.ContainsKey(JsonSerializer.TypeKey)))
				{
					var concrete = Type.GetType(json.Object[JsonSerializer.TypeKey].String);
					return (T) Activator.CreateInstance(concrete);
				}
				if (_registry.ContainsKey(type))
				{
					var concrete = _registry[type];
					return (T) Activator.CreateInstance(concrete);
				}
			}
			return Activator.CreateInstance<T>();
		}
	}
}
