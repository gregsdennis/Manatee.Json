namespace Manatee.Json.Tests.Test_References
{
	internal struct CustomStruct
	{
		public string A;
		public int B;

		public override string ToString()
		{
			return $"A: '{A}'; B: {B}";
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}
		public bool Equals(CustomStruct other)
		{
			return string.Equals(A, other.A) && B == other.B;
		}
		public override int GetHashCode()
		{
			unchecked
			{
				return ((A != null ? A.GetHashCode() : 0) * 397) ^ B;
			}
		}
	}
}