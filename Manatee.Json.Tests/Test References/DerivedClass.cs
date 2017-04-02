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
