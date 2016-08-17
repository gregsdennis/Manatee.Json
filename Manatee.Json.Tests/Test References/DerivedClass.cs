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
 
	File Name:		DerivedClass.cs
	Namespace:		Manatee.Tests.Test_References
	Class Name:		DerivedClass
	Purpose:		Derived class to be used in testing the Manatee.Json
					library.

***************************************************************************************/

using Manatee.Json.Schema;

namespace Manatee.Json.Tests.Test_References
{
	public class DerivedClass : AbstractClass
	{
		public string NewProp { get; set; }

		public override bool Equals(object obj)
		{
			if (!(obj is DerivedClass)) return false;
			return Equals((DerivedClass) obj);
		}
		public bool Equals(DerivedClass other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return base.Equals(other) && Equals(other.NewProp, NewProp);
		}
		public override int GetHashCode()
		{
			unchecked
			{
				return (base.GetHashCode()*397) ^ (NewProp != null ? NewProp.GetHashCode() : 0);
			}
		}
	}
}
