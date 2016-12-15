/***************************************************************************************

	Copyright 2016 Greg Dennis

	   Licensed under the Apache License, Version 2.0 (the "License");
	   you may not use this file except in compliance with the License.
	   You may obtain a copy of the License at

		 http://www.apache.org/licenses/LICENSE-2.0

	   Unless required by applicable law or agreed to in writing, software
	   distributed under the License is distributed on an "AS IS" BASIS,
	   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	   See the License for the specific language governing permissions and
	   limitations under the License.
 
	File Name:		ConstructorResolver.cs
	Namespace:		Manatee.Json.Serialization.Internal
	Class Name:		ConstructorResolver
	Purpose:		Implements IResolver using the Activator.

***************************************************************************************/

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