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
 
	File Name:		TypeRegistrationException.cs
	Namespace:		Manatee.Json.Serialization.Exceptions
	Class Name:		TypeRegistrationException
	Purpose:		Thrown when incorrectly attempting to register a type for the
					serializer.

***************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manatee.Json.Serialization.Exceptions
{
	/// <summary>
	/// Thrown when JsonSerializationTypeRegistry.RegisterType&lt;T&gt;(ToJsonDelegate&lt;T&gt; toJson, FromJsonDelegate&lt;T&gt; fromJson)
	/// is passed one method and a null.
	/// </summary>
	[Serializable]
	public class TypeRegistrationException : Exception
	{
		/// <summary>
		/// Gets the type.
		/// </summary>
		public Type Type { get; private set; }

		/// <summary>
		/// Initializes a new instance of the TypeRegistrationException class.
		/// </summary>
		/// <param name="type">The type.</param>
		public TypeRegistrationException(Type type)
			: base(string.Format("Attempted to register type {0} without supplying both an encoder and decoder.", type)) {}
	}
}
