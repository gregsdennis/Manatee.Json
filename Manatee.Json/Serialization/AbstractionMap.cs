﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Manatee.Json.Internal;
using Manatee.Json.Serialization.Internal;

namespace Manatee.Json.Serialization
{
	/// <summary>
	/// Provides an interface to map abstract and interface types to
	/// concrete types for object instantiation during deserialization.
	/// </summary>
	public class AbstractionMap
	{
		private readonly Dictionary<Type, Type> _registry;

		public static AbstractionMap Default { get; }

		static AbstractionMap()
		{
			Default = new AbstractionMap();
			Default.MapGeneric(typeof(IEnumerable<>), typeof(List<>));
			Default.MapGeneric(typeof(ICollection<>), typeof(List<>));
			Default.MapGeneric(typeof(IList<>), typeof(List<>));
			Default.MapGeneric(typeof(IDictionary<,>), typeof(Dictionary<,>));
		}
		public AbstractionMap()
		{
			_registry = new Dictionary<Type, Type>();
		}
		public AbstractionMap(AbstractionMap other)
		{
			_registry = other._registry.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
		}

		/// <summary>
		/// Applies a mapping from an abstraction to a concrete type.
		/// </summary>
		/// <typeparam name="TAbstract">The abstract type.</typeparam>
		/// <typeparam name="TConcrete">The concrete type.</typeparam>
		/// <param name="mappingBehavior">The mapping behavior.</param>
		/// <exception cref="JsonTypeMapException{TAbstract, TConcrete}">Thrown if TConcrete is an
		/// abstract class or an interface.</exception>
		public void Map<TAbstract, TConcrete>(MapBaseAbstractionBehavior mappingBehavior = MapBaseAbstractionBehavior.Unmapped)
			where TConcrete : TAbstract, new()
		{
			if (typeof(TConcrete).GetTypeInfo().IsAbstract || typeof(TConcrete).GetTypeInfo().IsInterface)
				throw new JsonTypeMapException<TAbstract, TConcrete>();
			var tAbstract = typeof (TAbstract);
			var tConcrete = typeof (TConcrete);
			_MapTypes(tAbstract, tConcrete, mappingBehavior);
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
		public void MapGeneric(Type tAbstract, Type tConcrete, MapBaseAbstractionBehavior mappingBehavior = MapBaseAbstractionBehavior.Unmapped)
		{
			if (tConcrete.GetTypeInfo().IsAbstract || tConcrete.GetTypeInfo().IsInterface)
				throw new JsonTypeMapException(tAbstract, tConcrete);
			if (!tConcrete.InheritsFrom(tAbstract))
				throw new JsonTypeMapException(tAbstract, tConcrete);
			_MapTypes(tAbstract, tConcrete, mappingBehavior);
		}
		/// <summary>
		/// Removes a previously-assigned mapping.
		/// </summary>
		/// <typeparam name="TAbstract">The type to remove.</typeparam>
		/// <param name="removeRelated">Optionally removes mappings of base and related interface types.</param>
		public void RemoveMap<TAbstract>(bool removeRelated = true)
		{
			var tAbstract = typeof (TAbstract);
			if (!_registry.TryGetValue(tAbstract, out Type tConcrete)) return;

			_registry.Remove(tAbstract);
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
		public Type GetMap(Type type)
		{
			if (!type.GetTypeInfo().IsAbstract && !type.GetTypeInfo().IsInterface) return type;
			if (_registry.TryGetValue(type, out Type tConcrete)) return tConcrete;

			if (type.GetTypeInfo().IsGenericType)
			{
				var genericDefinition = type.GetGenericTypeDefinition();
				var genericMatches = _registry.Where(t => t.Key.GetTypeInfo().IsGenericTypeDefinition && t.Key.GetGenericTypeDefinition() == genericDefinition).ToList();
				if (genericMatches.Any())
				{
					var typeArguments = type.GetTypeArguments();
					return genericMatches.First().Value.MakeGenericType(typeArguments);
				}
			}
			return type;
		}

		internal T CreateInstance<T>(JsonValue json, IResolver resolver)
		{
			var type = typeof (T);
			if (type.GetTypeInfo().IsAbstract || type.GetTypeInfo().IsInterface || type.GetTypeInfo().IsGenericType)
			{
				if ((json != null) && (json.Type == JsonValueType.Object) && (json.Object.ContainsKey(Constants.TypeKey)))
				{
					var concrete = Type.GetType(json.Object[Constants.TypeKey].String);
					return (T) resolver.Resolve(concrete);
				}
				if (!_registry.TryGetValue(type, out Type tConcrete))
				{
					if (type.GetTypeInfo().IsGenericType)
						type = type.GetGenericTypeDefinition();
					_registry.TryGetValue(type, out tConcrete);
				}

				if (tConcrete != null)
				{
					if (tConcrete.GetTypeInfo().IsGenericTypeDefinition)
						tConcrete = tConcrete.MakeGenericType(typeof(T).GetTypeArguments());
					return (T) resolver.Resolve(tConcrete);
				}

				if (type.GetTypeInfo().IsInterface)
					return TypeGenerator.Generate<T>();
			}
			return resolver.Resolve<T>();
		}

		private void _MapTypes(Type tAbstract, Type tConcrete, MapBaseAbstractionBehavior mappingBehavior)
		{
			_registry[tAbstract] = tConcrete;
			switch (mappingBehavior)
			{
				case MapBaseAbstractionBehavior.Unmapped:
					_MapBaseTypes(tAbstract, tConcrete, false);
					break;
				case MapBaseAbstractionBehavior.Override:
					_MapBaseTypes(tAbstract, tConcrete, true);
					break;
			}
		}
		private void _MapBaseTypes(Type tAbstract, Type tConcrete, bool overwrite)
		{
			if (tAbstract == null) return;
			var tBase = tAbstract.GetTypeInfo().BaseType;
			if (tBase != null && (overwrite || !_registry.ContainsKey(tBase)))
				_registry[tBase] = tConcrete;
			_MapBaseTypes(tBase, tConcrete, overwrite);
			foreach (var tInterface in tAbstract.GetTypeInfo().ImplementedInterfaces)
			{
				if (overwrite || !_registry.ContainsKey(tInterface))
				{
					_registry[tInterface] = tConcrete;
				}
				_MapBaseTypes(tInterface, tConcrete, overwrite);
			}
		}
	}
}
