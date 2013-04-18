using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manatee.Tests.Test_References
{
	public class ImplementationClass : Interface
	{
		public string RequiredProp { get; set; }

		public override bool Equals(object obj)
		{
			if (!(obj is ImplementationClass)) return false;
			return Equals((ImplementationClass) obj);
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
	}
}
