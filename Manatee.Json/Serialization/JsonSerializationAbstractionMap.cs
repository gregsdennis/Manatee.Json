/***************************************************************************************

	Copyright 2012 Greg Dennis

	   Licensed under the Apache License, Version 2.0 (the "License");
	   you may not use this file except in compliance with the License.
	   You may obtain a copy of the License at

		 http://www.apache.org/licenses/LICENSE-2.0

	   Unless required by applicable law or agreed to in writing, software
	   distributed under the License is distributed on an "AS IS" BASIS,
	   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	   See the License for the specific language governing permissions and
	   limitations under the License.
 
	File Name:		JsonSerializationAbstractionMap.cs
	Namespace:		Manatee.Json.Serialization
	Class Name:		JsonSerializationAbstractionMap
	Purpose:		Maps interfaces and abstract classes to concrete classes and
					in order to provide instances during instantia.

***************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Internal;
using Manatee.Json.Serialization.Internal;

namespace Manatee.Json.Serialization
{
	/// <summary>
	/// Provides an interface to map abstract and interface types to
	/// concrete types for object instantiation during deserialization.
	/// </summary>
	public static class JsonSerializationAbstractionMap
	{
		private static readonly Dictionary<Type, Type> _registry;
		private static readonly IResolver _defaultResolver;

		static JsonSerializationAbstractionMap()
		{
			_registry = new Dictionary<Type, Type>();
			_defaultResolver = new ActivatorResolver();
		}

		/// <summary>
		/// Applies a mapping from an abstraction to a concrete type.
		/// </summary>
		/// <typeparam name="TAbstract">The abstract type.</typeparam>
		/// <typeparam name="TConcrete">The concrete type.</typeparam>
		/// <exception cref="JsonTypeMapException{TAbstract, TConcrete}">Thrown if TConcrete is an
		/// abstract class or an interface.</exception>
		public static void Map<TAbstract, TConcrete>(MapBaseAbstractionBehavior mappingBehavior = MapBaseAbstractionBehavior.Unmapped)
			where TConcrete : TAbstract, new()
		{
			if (typeof(TConcrete).IsAbstract || typeof(TConcrete).IsInterface)
				throw new JsonTypeMapException<TAbstract, TConcrete>();
			var tAbstract = typeof (TAbstract);
			var tConcrete = typeof (TConcrete);
			MapTypes(tAbstract, tConcrete, mappingBehavior);
		}
		public static void MapGeneric(Type tAbstract, Type tConcrete, MapBaseAbstractionBehavior mappingBehavior = MapBaseAbstractionBehavior.Unmapped)
		{
			if (tConcrete.IsAbstract || tConcrete.IsInterface)
				throw new Exception();
			if (!tConcrete.InheritsFrom(tAbstract))
				throw new Exception();
			MapTypes(tAbstract, tConcrete, mappingBehavior);
		}
		/// <summary>
		/// Removes a previously-assigned mapping.
		/// </summary>
		/// <typeparam name="TAbstract">The type to remove.</typeparam>
		public static void RemoveMap<TAbstract>(bool removeRelated = true)
		{
			var tAbstract = typeof (TAbstract);
			if (!_registry.ContainsKey(tAbstract)) return;
			var tConcrete = _registry[tAbstract];
			_registry.Remove(typeof (TAbstract));
			if (!removeRelated) return;
			var removeTypes = _registry.Where(kvp => kvp.Value == tConcrete).Select(kvp => kvp.Key).ToList();
			foreach (var type in removeTypes)
			{
				_registry.Remove(type);
			}
		}
		/// <summary>
		/// Retrieves the map setting for an abstraction type.
		/// </summary>
		/// <param name="type">The abstraction type.</param>
		/// <returns>The mapped type if a mapping exists; otherwise the abstraction type.</returns>
		public static Type GetMap(Type type)
		{
			return _registry.ContainsKey(type) ? _registry[type] : type;
		}

		internal static T CreateInstance<T>(JsonValue json, IResolver resolver)
		{
			var type = typeof (T);
			resolver = resolver ?? _defaultResolver;
			if (type.IsAbstract || type.IsInterface)
			{
				if ((json.Type == JsonValueType.Object) && (json.Object.ContainsKey(Constants.TypeKey)))
				{
					var concrete = Type.GetType(json.Object[Constants.TypeKey].String);
					return (T) resolver.Resolve(concrete);
				}
				if (_registry.ContainsKey(type))
				{
					var concrete = _registry[type];
					return (T) resolver.Resolve(concrete);
				}
#if !IOS
				if (type.IsInterface)
					return TypeGenerator.Generate<T>();
#endif
			}
			return resolver.Resolve<T>();
		}

		private static void MapTypes(Type tAbstract, Type tConcrete, MapBaseAbstractionBehavior mappingBehavior)
		{
			_registry[tAbstract] = tConcrete;
			switch (mappingBehavior)
			{
				case MapBaseAbstractionBehavior.Unmapped:
					MapBaseTypes(tAbstract, tConcrete, false);
					break;
				case MapBaseAbstractionBehavior.Override:
					MapBaseTypes(tAbstract, tConcrete, true);
					break;
			}
		}
		private static void MapBaseTypes(Type tAbstract, Type tConcrete, bool overwrite)
		{
			if (tAbstract == null) return;
			var tBase = tAbstract.BaseType;
			if ((tBase != null) && (overwrite || !_registry.ContainsKey(tBase)))
			{
				_registry[tBase] = tConcrete;
			}
			MapBaseTypes(tBase, tConcrete, overwrite);
			foreach (var tInterface in tAbstract.GetInterfaces())
			{
				if (overwrite || !_registry.ContainsKey(tInterface))
				{
					_registry[tInterface] = tConcrete;
				}
				MapBaseTypes(tInterface, tConcrete, overwrite);
			}
		}
	}
}
