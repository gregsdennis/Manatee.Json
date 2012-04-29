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
 
	File Name:		NotPrimitiveTypeException.cs
	Namespace:		Manatee.Json.Serialization.Exceptions
	Class Name:		NotPrimitiveTypeException
	Purpose:		Thrown when a request is send to the PrimitiveMapper to map
					a non-primitive type.

***************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manatee.Json.Serialization.Exceptions
{
	class NotPrimitiveTypeException : Exception
	{
		public Type RequestedType { get; private set; }
		public NotPrimitiveTypeException(Type type)
			: base(string.Format("Type {0} is not primitive.", type)) {}
	}
}
