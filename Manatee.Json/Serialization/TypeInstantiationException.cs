/***************************************************************************************

	Copyright 2014 Greg Dennis

	   Licensed under the Apache License, Version 2.0 (the "License");
	   you may not use this file except in compliance with the License.
	   You may obtain a copy of the License at

		 http://www.apache.org/licenses/LICENSE-2.0

	   Unless required by applicable law or agreed to in writing, software
	   distributed under the License is distributed on an "AS IS" BASIS,
	   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	   See the License for the specific language governing permissions and
	   limitations under the License.
 
	File Name:		TypeInstantiationException.cs
	Namespace:		Manatee.Json.Serialization
	Class Name:		TypeInstantiationException
	Purpose:		Thrown when a type cannot be instantiated.

***************************************************************************************/

using System;

namespace Manatee.Json.Serialization
{
	public class TypeInstantiationException : Exception
	{
		public TypeInstantiationException(Type type)
			: base(string.Format("Manatee.Json cannot create an instance of type '{0}' through the default resolver." +
								 " You may need to implement your own IResolver to instantiate this type.", type)) {}
	}
}