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
 
	File Name:		AbstractClass.cs
	Namespace:		Manatee.Tests.Test_References
	Class Name:		AbstractClass
	Purpose:		Abstract class to be used in testing the Manatee.Json
					library.

***************************************************************************************/
namespace Manatee.Json.Tests.Test_References
{
	public abstract class AbstractClass
	{
		public int SomeProp { get; set; }

		public override bool Equals(object obj)
		{
			if (obj is AbstractClass)
				return Equals((AbstractClass) obj);
			return base.Equals(obj);
		}

		public bool Equals(AbstractClass other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return other.SomeProp == SomeProp;
		}

		public override int GetHashCode()
		{
			return SomeProp;
		}
	}
}
