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
 
	File Name:		JsonTypeMapException.cs
	Namespace:		Manatee.Json.Serialization
	Class Name:		JsonTypeMapException
	Purpose:		Thrown when an abstract or interface type is mapped to another
					abstract or interface type.

***************************************************************************************/

using System;

namespace Manatee.Json.Serialization
{
	///<summary>
	/// Thrown when an abstract or interface type is mapped to another abstract or interface type.
	///</summary>
	public class JsonTypeMapException : Exception
	{
		internal JsonTypeMapException(Type abstractType, Type concreteType)
			: base(string.Format("Cannot create map from type '{0}' to type '{1}' because the destination type is either abstract or an interface.",
								 abstractType,
								 concreteType)) {}
	}

	///<summary>
	/// Thrown when an abstract or interface type is mapped to another abstract or interface type.
	///</summary>
	///<typeparam name="TAbstract">The type being mapped from.</typeparam>
	///<typeparam name="TConcrete">The type being mapped to.</typeparam>
	public class JsonTypeMapException<TAbstract, TConcrete> : JsonTypeMapException
	{
		/// <summary>
		/// Creates a new instance of the <see cref="JsonTypeMapException&lt;TAbstract, TConcrete&gt;"/> object.
		/// </summary>
		internal JsonTypeMapException()
			: base(typeof (TAbstract), typeof (TConcrete)) {}
	}
}
