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
 
	File Name:		ObjectWithAbstractAndInterfaceProps.cs
	Namespace:		Manatee.Tests.Test_References
	Class Name:		ObjectWithAbstractAndInterfaceProps
	Purpose:		Basic class that contains abstract-typed and interface-
					typed properties to be used in testing the Manatee.Json
					library.

***************************************************************************************/
using System;

namespace Manatee.Tests.Test_References
{
	public class ObjectWithAbstractAndInterfaceProps
	{
		public IComparable InterfaceProp { get; set; }
		public AbstractClass AbstractProp { get; set; }

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof (ObjectWithAbstractAndInterfaceProps)) return false;
			return Equals((ObjectWithAbstractAndInterfaceProps) obj);
		}

		public bool Equals(ObjectWithAbstractAndInterfaceProps other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.InterfaceProp, InterfaceProp) && Equals(other.AbstractProp, AbstractProp);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((InterfaceProp != null ? InterfaceProp.GetHashCode() : 0)*397) ^ (AbstractProp != null ? AbstractProp.GetHashCode() : 0);
			}
		}
	}
}
