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
 
	File Name:		ImplementationClass.cs
	Namespace:		Manatee.Tests.Test_References
	Class Name:		ImplementationClass
	Purpose:		Basic implementation of IInterface.

***************************************************************************************/

using System;

namespace Manatee.Json.Tests.Test_References
{
	public class ImplementationClass : IInterface
	{
		public string this[int a]
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}
		public string RequiredProp { get; set; }

		public T RequiredMethod<T, U>(U str) where T : U
		{
			return default(T);
		}
		public event EventHandler RequiredEvent
		{
			add { }
			remove { }
		}

		public override bool Equals(object obj)
		{
			if (!(obj is ImplementationClass)) return false;
			return Equals((ImplementationClass)obj);
		}
		public bool Equals(ImplementationClass other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.RequiredProp, RequiredProp);
		}
		public override int GetHashCode()
		{
			return (RequiredProp != null ? RequiredProp.GetHashCode() : 0);
		}
		public int CompareTo(object obj)
		{
			throw new NotImplementedException();
		}
		public override string ToString()
		{
			return string.Format("RequiredProp: {0}", RequiredProp);
		}
	}
}
