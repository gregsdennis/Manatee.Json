using System;

namespace Manatee.Json.Tests.Test_References
{
	public struct Mass : IEquatable<Mass>
	{
		public double Value { get; set; }

		public bool Equals(Mass other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Value.Equals(other.Value);
		}
		public override bool Equals(object obj)
		{
			return obj is Mass mass && Equals(mass);
		}
		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}
	}
}