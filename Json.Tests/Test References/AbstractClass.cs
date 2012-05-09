using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manatee.Tests.Test_References
{
	abstract class AbstractClass
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
