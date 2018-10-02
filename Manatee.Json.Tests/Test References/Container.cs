using System;

namespace Manatee.Json.Tests.Test_References
{
	public class Container : IEquatable<Container>
	{
		public Mass? Value { get; set; }

		public bool Equals(Container other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Value.Equals(other.Value);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as Container);
		}
		public override int GetHashCode()
		{
			return Value?.GetHashCode() ?? 0;
		}
	}
}