using System;

namespace Manatee.Tests.Test_References
{
	public interface IInterface : IComparable
	{
		string this[int a] { get; set; }
		string RequiredProp { get; set; }
		T RequiredMethod<T, U>(U str) where T : U;
	}
}