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
 
	File Name:		MemberReference.cs
	Namespace:		Manatee.Json.Serialization.Internal
	Class Name:		MemberReference
	Purpose:		Base classes for defining a field or property on an object
					for the purpose of cataloging references.

***************************************************************************************/
using System.Reflection;

namespace Manatee.Json.Serialization.Internal
{
	internal abstract class MemberReference
	{
		public object Owner { get; set; }

		public abstract object GetValue(object instance);
		public abstract void SetValue(object instance, object value);
	}

	internal abstract class MemberReference<T> : MemberReference
		where T : MemberInfo
	{
		public T Info { get; set; }
	}
}