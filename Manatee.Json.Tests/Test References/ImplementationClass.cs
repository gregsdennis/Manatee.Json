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
