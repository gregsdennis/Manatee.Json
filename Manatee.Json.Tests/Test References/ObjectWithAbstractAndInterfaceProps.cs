﻿namespace Manatee.Json.Tests.Test_References
{
	public class ObjectWithAbstractAndInterfaceProps
	{
		public IInterface InterfaceProp { get; set; }
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
