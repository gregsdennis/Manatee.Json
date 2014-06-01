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
 
	File Name:		IResolver.cs
	Namespace:		Manatee.Json.Serialization
	Class Name:		IResolver
	Purpose:		Defines methods required to resolved instances for
					deserialization.

***************************************************************************************/

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