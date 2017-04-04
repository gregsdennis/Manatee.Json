using System;
using System.Linq;
using System.Reflection;
using Manatee.Json.Internal;

namespace Manatee.Json.Serialization.Internal
{
	internal class ConstructorResolver : IResolver
	{
		public T Resolve<T>()
		{
			return (T) Resolve(typeof (T));
		}
		public object Resolve(Type type)
		{
			try
			{
#if IOS
				var constructors = type.TypeInfo().DeclaredConstructors.ToList();
#else
				var constructors = type.TypeInfo().GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public).ToList();
#endif
				if (!constructors.Any())
					return Activator.CreateInstance(type);
				var parameterless = constructors.FirstOrDefault(c => !c.GetParameters().Any());
				if (parameterless != null)
					return parameterless.Invoke(null);
				var constructor = constructors.OrderBy(c => c.GetParameters().Count()).First();
				var parameters = constructor.GetParameters().Select(p => Resolve(p.ParameterType)).ToArray();
				return constructor.Invoke(parameters);
			}
			catch
			{
				throw new TypeInstantiationException(type);
			}
		}
	}
}