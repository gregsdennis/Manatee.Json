using System;

namespace Manatee.Json.Tests.Test_References
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