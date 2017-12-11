using System;
using System.Linq;
using System.Reflection;

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
				ConstructorInfo shortestCtor = null;
				foreach (var constructor in type.GetTypeInfo().DeclaredConstructors)
				{
					if (constructor.GetParameters().Length == 0)
					{
						return constructor.Invoke(null);
					}

					if (shortestCtor == null)
					{
						shortestCtor = constructor;
					}
					else if (constructor.GetParameters().Length < shortestCtor.GetParameters().Length)
					{
						shortestCtor = constructor;
					}
				}

				if (shortestCtor != null)
				{
					var parameters = shortestCtor.GetParameters().Select(p => Resolve(p.ParameterType)).ToArray();
					return shortestCtor.Invoke(parameters);
				}

				return Activator.CreateInstance(type);
			}
			catch
			{
				throw new TypeInstantiationException(type);
			}
		}
	}
}