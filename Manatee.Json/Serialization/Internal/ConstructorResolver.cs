using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Manatee.Json.Serialization.Internal
{
	internal class ConstructorResolver : IResolver
	{
		public object? Resolve(Type type, Dictionary<SerializationInfo, object>? parameters)
		{
			try
			{
				if (parameters == null)
					return _ResolveSimple(type);
				return _Resolve(type, parameters);
			}
			catch (Exception e)
			{
				throw new TypeInstantiationException(type, e);
			}
		}

		private static object? _Resolve(Type type, Dictionary<SerializationInfo, object>? parameters)
		{
			var ctors = type.GetTypeInfo().DeclaredConstructors
				.Select(c => new {Method = c, Parameters = c.GetParameters()})
				.OrderBy(c => c.Parameters.Length)
				.ToList();

			if (ctors.Count == 0)
				return Activator.CreateInstance(type);

			if (ctors[0].Parameters.Length == 0)
				return ctors[0].Method.Invoke(null);

			var scored = ctors.Select(c =>
					{
						var matched = c.Parameters.Join(parameters,
						                                cp => cp.Name!.ToLowerInvariant(),
						                                p => (p.Key.SerializationName ?? p.Key.MemberInfo.Name).ToLowerInvariant(),
						                                (cp, p) => new {ConstructorParameter = cp, JsonParameter = p}).ToList();
						var score = (double) matched.Count / c.Parameters.Length;

						return new
							{
								Constructor = c.Method,
								Matched = matched,
								Score = score
							};
					})
				.OrderByDescending(c => c.Score)
				.ThenByDescending(c => c.Matched.Count);

			var bestMatch = scored.First();

			return bestMatch.Constructor.Invoke(bestMatch.Matched.Select(m => m.JsonParameter.Value).ToArray());
		}

		private static object? _ResolveSimple(Type type)
		{
			ConstructorInfo? shortestCtor = null;
			foreach (var constructor in type.GetTypeInfo().DeclaredConstructors)
			{
				if (constructor.GetParameters().Length == 0)
					return constructor.Invoke(null);

				if (shortestCtor == null)
					shortestCtor = constructor;
				else if (constructor.GetParameters().Length < shortestCtor.GetParameters().Length)
					shortestCtor = constructor;
			}

			if (shortestCtor != null)
			{
				var ctorParameters = shortestCtor.GetParameters().Select(p => _ResolveSimple(p.ParameterType)).ToArray();
				return shortestCtor.Invoke(ctorParameters);
			}

			return Activator.CreateInstance(type);
		}
	}
}