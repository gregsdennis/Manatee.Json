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
 
	File Name:		TypeRegistrationException.cs
	Namespace:		Manatee.Json.Serialization
	Class Name:		TypeRegistrationException
	Purpose:		Thrown when incorrectly attempting to register a type for the
					serializer.

***************************************************************************************/

using System;

namespace Manatee.Json.Serialization
{
	/// <summary>
	/// Thrown when <see cref="JsonSerializationTypeRegistry.RegisterType&lt;T&gt;(JsonSerializationTypeRegistry.ToJsonDelegate&lt;T&gt;, JsonSerializationTypeRegistry.FromJsonDelegate&lt;T&gt;)"/>
	/// is passed one method and a null.
	/// </summary>
	public class TypeRegistrationException : Exception
	{
		/// <summary>
		/// Gets the type.
		/// </summary>
		public Type Type { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="TypeRegistrationException"/> class.
		/// </summary>
		/// <param name="type">The type.</param>
		internal TypeRegistrationException(Type type)
			: base($"Attempted to register type {type} without supplying both an encoder and decoder.")
		{
			Type = type;
		}
	}
}
