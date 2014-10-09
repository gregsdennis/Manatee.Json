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
 
	File Name:		IInterface.cs
	Namespace:		Manatee.Tests.Test_References
	Class Name:		IInterface
	Purpose:		Simple interface to be used in testing the Manatee.Json
					library.

***************************************************************************************/
using System;

namespace Manatee.Tests.Test_References
{
	public interface IInterface : IComparable
	{
// ReSharper disable UnusedMember.Global
		string this[int a] { get; set; }
// ReSharper restore UnusedMember.Global
		string RequiredProp { get; set; }
		T RequiredMethod<T, U>(U str) where T : U;
// ReSharper disable EventNeverInvoked.Global
		event EventHandler RequiredEvent;
// ReSharper restore EventNeverInvoked.Global
	}

	public interface IFace<T>
	{
		int Value { get; set; }
	}

	public class Impl<T> : IFace<T>
	{
		public int Value { get; set; }
	}

	public class Derived<T> : Impl<T> { }


}